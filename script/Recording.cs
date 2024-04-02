using Godot;
using System;
using System.Runtime.CompilerServices; 

public class Recording<T>
{
	// Tick Range: -36000 to 36000
	private const int MAX_TIME = 36000;
	private const int MIN_TIME = -36000;
	private const int MAX_BUFFER_TICKS = 3600;
	private const int MAX_BUFFER_MINUTES = 20;
	private const int BUFFER_MID = 10;

	public bool Inverted { get; }
	public bool EndOfRecording { get => _time >= _end; }
	public bool StartOfRecording { get => _time < _start; }
	public int TimeTicks { get => InTicks(_time); }
	public int TimeMinutes { get => InMinutes(_time); }
	public int LengthTicks { get => InTicks(_end - _start); }
	public int LengthMinutes { get => InMinutes(_end - _start); }

	private int _time = 0;
	private int _end = -1;
	private int _start = 0;
	private T[][] _recordingBuffer = new T[MAX_BUFFER_MINUTES][];    // BUFFER_MID minutes
	private bool _invert = false;

	public Recording()
	{
		_recordingBuffer[BUFFER_MID] = new T[MAX_BUFFER_TICKS];	// 60 ticks per second for 1 minute
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int InTicks(int time) {
		return (time % MAX_BUFFER_TICKS + MAX_BUFFER_TICKS) % MAX_BUFFER_TICKS;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int InMinutes(int time) {
		return time / MAX_BUFFER_TICKS;
	}

	/// <summary>
	///   Append data to the recording buffer
	/// </summary>
	/// <param name="data">The data to append to the buffer</param>
	public void Append(T data) {
		var minutes = TimeMinutes;
		_recordingBuffer[TimeMinutes+BUFFER_MID][TimeTicks] = data;
		TickUpdate(_invert);
		if (TimeMinutes != minutes && _recordingBuffer[TimeMinutes+BUFFER_MID] is null) {
			_recordingBuffer[TimeMinutes+BUFFER_MID] = new T[MAX_BUFFER_TICKS];
		}
		SetEndPoint(_invert);
	}

	private void TickUpdate(bool inverted) {
		if (inverted) {
			_time--;
		} else {
			_time++;
		}
	}

	/// <summary>
	///   Invert the time direction of the recording
	/// </summary>
	public void Invert() {
		_invert = !_invert;
	}

	public T Get(int time) {
		if (EndOfRecording) {
			return _recordingBuffer[InMinutes(_end)+BUFFER_MID][InTicks(_end)];
		}
		if (StartOfRecording) {
			return _recordingBuffer[InMinutes(_start)+BUFFER_MID][InTicks(_start)];
		}
		return _recordingBuffer[InMinutes(time)+BUFFER_MID][InTicks(time)];
	}

	/// <summary>
	///   Get the next piece of data relative to the current time direction
	/// </summary>
	/// <returns>The element at the next index</returns>
	public T Next() {
		var ret = Get(_time);
		TickUpdate(_invert);
		return ret;
	}

	/// <summary>
	///   Get the previous piece of data relative to current the time direction
	/// </summary>
	/// <returns>The element at the previous index</returns>
	public T Previous() {
		var ret = Get(_time);
		TickUpdate(!_invert);
		return ret;
	}

	public void Reset() {
		_time = 0;
		_end = -1;
		_start = 0;
		_invert = false;
	}

	/// <summary>
	/// Set the (relative) end point of the recording buffer to the current time
	/// </summary>
	/// <param name="inverted"> 
	/// If set, this will set the starting index to the current time.
	/// Otherwise, this will set the ending index to the current time.
	/// </param>
	public void SetEndPoint(bool inverted) {
		if (!inverted) {
			if (_end < _time && _time <= MAX_TIME) _end = _time;
		} else {
			if (_start > _time && _time > MIN_TIME) _start = _time;
		}
	}

    public override string ToString()
    {
		return _recordingBuffer.ToString();
    }
}
