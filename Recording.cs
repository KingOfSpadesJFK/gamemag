using Godot;
using System;

public class Recording<T>
{
	public bool Inverted { get; }
	public bool EndOfRecording { get => _time >= _length; }
	public bool StartOfRecording { get => _time <= 0; }
	public int TimeTicks { get => _time % 3600; }
	public int TimeMinutes { get => _time / 3600; }
	public int LengthTicks { get => _length % 3600; }
	public int LengthMinutes { get => _length / 3600; }

	private int _time = 0;
	private int _length = -1;
	private T[][] _recordingMinute = new T[10][];    // 10 minutes
	private bool _invert = false;

	public Recording()
	{
		_recordingMinute[0] = new T[3600];	// 60 ticks per second for 1 minute
	}

	public void Push(T data) {
		var minutes = TimeMinutes;
		_recordingMinute[TimeMinutes][TimeTicks] = data;
		TickUpdate(_invert);
		if (TimeMinutes != minutes && _recordingMinute[TimeMinutes] is null) {
			_recordingMinute[TimeMinutes] = new T[3600];
		}
		_length++;
	}

	private void TickUpdate(bool inverted) {
		if (inverted) {
			_time--;
		} else {
			_time++;
		}
	}

	public void Invert() {
		_invert = !_invert;
	}

	public T Get(int time) {
		if (EndOfRecording) {
			return _recordingMinute[LengthMinutes][LengthTicks];
		}
		if (StartOfRecording) {
			return _recordingMinute[0][0];
		}
		return _recordingMinute[time / 3600][time % 3600];
	}

	public T Next() {
		TickUpdate(_invert);
		return Get(_time);
	}

	public T Previous() {
		TickUpdate(!_invert);
		return Get(_time);
	}
}
