using Godot;
using System;
using System.Linq;

// This class keeps track of the time and records the player's position.
public partial class TimeKeeper : Node
{
	public bool Inverted { get => _invert; }
	public int RecordingCount { get => _recordingCount; }
	public int Time { get => _time; }
	public int Minutes { get => _time / 3600; }
	public int Seconds { get => Ticks / 60; }
	public int Ticks { get => _time % 3600; }
	private Recording<Vector2>[] _positionRecordings;
	private int _time = 0;
	private bool _invert = false;
	private int _recordingCount = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_positionRecordings = new Recording<Vector2>[0];
	}

	// Every physics tick, update the time.
	public override void _PhysicsProcess(double delta)
	{
		if (_invert) {
			_time--;
		} else {
			_time++;
		}
	}

	// Inverts the recordings and the time direction.
	public void Invert() {
		GD.Print("Inverting time to " + (_invert ? "forward" : "backward") + "!");
		foreach (var recording in _positionRecordings) {
			recording.Invert();
		}
		_invert = !_invert;
	}

	// Add the passed recording to the list of recordings.
	public void AddRecording(Recording<Vector2> recording)
	{
		_positionRecordings = _positionRecordings.Concat(new Recording<Vector2>[] { recording }).ToArray();
		_recordingCount++;
	}
}
