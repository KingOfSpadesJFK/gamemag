using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices; 

/// <summary>
///  A recording buffer that stores data in a sorted dictionary. The recording buffer can be played back in reverse.
///  This is useful for storing data that changes on an interval, like whether an enemy is alive or dead.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Schedule<T> : ITimedData<T>
{
	// Tick Range: 0 to 72000
	private const int MAX_TIME = MAX_BUFFER_TICKS * MAX_BUFFER_MINUTES;
	private const int MIN_TIME = -1;
	private const int MAX_BUFFER_TICKS = MAX_TICKS_PER_SECOND * 60;
	private const int MAX_TICKS_PER_SECOND = 60;
	private const int MAX_BUFFER_MINUTES = 20;
	private const int BUFFER_MID = 10;

	public bool Inverted { get => _invert; }
	public bool EndOfRecording   { get => _time >= _end;  }
	public bool StartOfRecording { get => _time <= _start; }
	public int TimeTicks { get => InTicks(_time); }
	public int TimeMinutes { get => InMinutes(_time); }
	public int LengthTicks { get => InTicks(_end - _start); }
	public int LengthMinutes { get => InMinutes(_end - _start); }
	public int StartPoint { get => _start; }
	public int EndPoint { get => _end; }	

	protected int _time = BUFFER_MID * MAX_BUFFER_TICKS;
	protected int _end = BUFFER_MID * MAX_BUFFER_TICKS;
	protected int _start = BUFFER_MID * MAX_BUFFER_TICKS - 1;
	protected bool _invert = false;

	// The schedule is a sorted dictionary that stores the starting time and the data
	// Key: Starting time
	// Value: Data
	private SortedDictionary<int, T> _schedule = new SortedDictionary<int, T>();
    
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int InTicks(int time) {
		return time % MAX_BUFFER_TICKS;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int InMinutes(int time) {
		return time / MAX_BUFFER_TICKS;
	}

	/// <summary>
	///  Append data to the start of the recording buffer
	/// </summary>
	/// <param name="data"></param>
	public void AppendAtStart(T data) 
	{
		if (_start > MIN_TIME) {
			// If the schedule is empty, add the data
			if (_schedule.Count == 0) {
				_schedule.Add(_start, data);
			} else {
				var nearest = FindNearestKey(_start);
				// If the input data is different from the current start data, 
				//  move the current start data to the next time index
				if (!_schedule[nearest].Equals(data)) {
					var backup = _schedule[nearest];
					if (nearest == _start+1 && _schedule.ContainsKey(_start+1)) _schedule.Remove(_start+1);
					if (nearest == _start   && _schedule.ContainsKey(_start)  ) _schedule.Remove(_start);
					_schedule.Add(_start+1, backup);
					_schedule.Add(_start, data);
				}
			}
			_start--;
		}
	}

	/// <summary>
	///  Append data to the end of the recording buffer
	/// </summary>
	/// <param name="data"></param>
	public void AppendAtEnd(T data) 
	{
		if (_end < MAX_TIME) {
			// If the schedule is empty, add the data
			if (_schedule.Count == 0) {
				_schedule.Add(_end, data);
			} else {
				var nearest = FindNearestKey(_end);
				if (!_schedule[nearest].Equals(data)) {
					if (nearest == _end && _schedule.ContainsKey(_end)) _schedule.Remove(_end);
					_schedule.Add(_end, data);
				}
			}
			_end++;
		}
	}

	/// <summary>
	///   Append data to the (relative) end of the recording buffer
	/// </summary>
	/// <param name="data">The data to append to the buffer</param>
	public void Append(T data) 
	{
		if (_invert) {
			AppendAtStart(data);
		} else {
			AppendAtEnd(data);
		}
	}

	/// <summary>
	///  Reset the recording buffer. The data will remain, but all time indices will be reset.
	/// </summary>
	public void Reset() 
	{
		_time = BUFFER_MID * MAX_BUFFER_TICKS;
		_end = BUFFER_MID * MAX_BUFFER_TICKS;
		_start = BUFFER_MID * MAX_BUFFER_TICKS - 1;
		_invert = false;
	}

	private void TickUpdate(bool inverted) 
	{
		if (inverted) {
			_time--;
		} else {
			_time++;
		}
	}

	/// <summary>
	///   Invert the playback direction of the recording
	/// </summary>
	public void Invert() 
	{
		_invert = !_invert;
	}

	/// <summary>
	///  Set the (relative) end point of the recording buffer to the current time
	/// </summary>
	/// <param name="inverted"> 
	///  If set, this will set the starting index to the current time.
	///  Otherwise, this will set the ending index to the current time.
	/// </param>
	public void SetEndPoint(bool inverted) 
	{
		GD.Print("Setting end point to " + _time + "!");
		if (!inverted) {
			_end = _time;
		} else {
			_start = _time;
		}
	}

	/// <summary>
	///  Set the (relative) starting point of the recording buffer to the current time
	/// </summary>
	/// <param name="inverted"> 
	///  If set, this will set the ending index to the current time.
	///  Otherwise, this will set the starting index to the current time.
	/// </param>
	public void SetStartPoint(bool inverted) 
	{
		SetEndPoint(!inverted);
	}

	/// <summary>
	///  Set the playback time to the (relative) start of the recording buffer
	/// </summary>
	public void StartPlaybackAtBeginning() 
	{
		if (!_invert) {
			_time = _start;
		} else {
			_time = _end;
		}
	}

	/// <summary>
	///  Set the playback time to the (relative) end of the recording buffer
	/// </summary>
	public void StartPlaybackAtEnding() 
	{
		if (!_invert) {
			_time = _end;
		} else {
			_time = _start;
		}
	}

	private int FindNearestKey(int searchKey)
	{
		int index = Array.BinarySearch(_schedule.Keys.ToArray(), searchKey);

		// If exact key is found, return it
		if (index >= 0) {
			return _schedule.Keys.ToArray()[index];
		} else {
			// If exact key is not found, find the nearest key
			index = ~index;		// Bitwise complement of the index. It's a C# thing
			index--;

			// Handle edge cases
			if (index < 0)
				return _schedule.Keys.First();
			else if (index >= _schedule.Count)
				return _schedule.Keys.Last();
			else
                return _schedule.Keys.ToArray()[index];
		}
	}

	/// <summary>
	///  Get the data at the specified time
	/// </summary>
	/// <param name="minute">Minute of the specified time</param>
	/// <param name="second">Second of the specified time</param>
	/// <param name="tick">Subsecond tick of the specified time (60 ticks per minute)</param>
	/// <returns></returns>
	public T Get(int minute, int second, int tick) 
	{
		return Get(minute * MAX_BUFFER_TICKS + second * MAX_TICKS_PER_SECOND + tick);
	}

	/// <summary>
	///  Get the data at the specified time
	/// </summary>
	/// <param name="time">The index to where the time is</param>
	/// <returns></returns>
	public T Get(int time) 
	{
		if (time >= _end   || time >= MAX_TIME) return _schedule[FindNearestKey(_end-1)];
		if (time <= _start || time <= MIN_TIME) return _schedule[FindNearestKey(_start+1)];
		return _schedule[FindNearestKey(time)];
	}

    public T Next()
    {
		var data = Get(_time);
		TickUpdate(_invert);
		return data;
    }

    public T Previous()
    {
		var data = Get(_time);
		TickUpdate(!_invert);
		return data;
    }

	public T Seek(int time) 
	{
		return Get(_time + time);
	}
}