using Godot;
using System;
using System.Runtime.CompilerServices; 

/// <summary>
///  A recording buffer that stores data in a 2D array. The recording buffer can be played back in reverse.
///  This is useful for storing data that's updated on a per-frame basis, like a node's position.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Recording<T> : ITimedData<T>
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
	private T[][] _recordingBuffer = new T[MAX_BUFFER_MINUTES][];

	public Recording()
	{
		_recordingBuffer[BUFFER_MID] = new T[MAX_BUFFER_TICKS];	// 60 ticks per second for 1 minute
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int InTicks(int time) {
		return time % MAX_BUFFER_TICKS;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int InMinutes(int time) {
		return time / MAX_BUFFER_TICKS;
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
	///  Append data to the start of the recording buffer
	/// </summary>
	/// <param name="data"></param>
	public void AppendAtStart(T data) 
	{
		if (_start > MIN_TIME) {
			if (_recordingBuffer[InMinutes(_start)] is null) {
				_recordingBuffer[InMinutes(_start)] = new T[MAX_BUFFER_TICKS];
			}
			_recordingBuffer[InMinutes(_start)][InTicks(_start)] = data;
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
			if (_recordingBuffer[InMinutes(_end)] is null) {
				_recordingBuffer[InMinutes(_end)] = new T[MAX_BUFFER_TICKS];
			}
			_recordingBuffer[InMinutes(_end)][InTicks(_end)] = data;
			_end++;
		}
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
		if (time >= _end   || time >= MAX_TIME) return _recordingBuffer[InMinutes(_end-1)][InTicks(_end-1)];
		if (time <= _start || time <= MIN_TIME) return _recordingBuffer[InMinutes(_start+1)][InTicks(_start+1)];
		return _recordingBuffer[InMinutes(time)][InTicks(time)];
	}

	/// <summary>
	///   Get the next piece of data relative to the current time direction
	/// </summary>
	/// <returns>The element at the next index</returns>
	public T Next() 
	{
		var data = Get(_time);
		TickUpdate(_invert);
		return data;
	}

	/// <summary>
	///   Get the previous piece of data relative to current the time direction
	/// </summary>
	/// <returns>The element at the previous index</returns>
	public T Previous() 
	{
		var data = Get(_time);
		TickUpdate(!_invert);
		return data;
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

	public T Seek(int time) 
	{
		return Get(_time + time);
	}

	public override string ToString()
	{
		return _recordingBuffer.ToString();
	}
}
