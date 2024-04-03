using Godot;
using System;
using System.Linq;

// This class keeps track of the time and records the player's position.
public partial class TimeKeeper : Node
{
	private const int MAX_MINUTE_TICKS = 3600;
	private const int MAX_MINUTES = 10;
	private const int STARTING_MINUTE = 5;
	private const int TIMEOUT_MIN = 0;
	private const int TIMEOUT_MAX = MAX_MINUTES * MAX_MINUTE_TICKS;

	public bool Inverted { get => _invert; }
	public int RecordingCount { get => _recordingCount; }
	public int Time { get => _time; }
	public int Minutes { get => _time / MAX_MINUTE_TICKS; }
	public int Seconds { get => Ticks / 60; }
	public int Ticks { get => _time % MAX_MINUTE_TICKS; }
	public float TimeProgress { get => (float)_time / TIMEOUT_MAX; }
	private int _time = STARTING_MINUTE * MAX_MINUTE_TICKS;
	private bool _invert = false;
	private int _recordingCount = 0;

	[Signal] public delegate void InvertTimeEventHandler();
	[Signal] public delegate void TimeoutEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    // Every physics tick, update the time.
    public override void _PhysicsProcess(double delta)
	{
		if (_invert) {
			_time--;
		} else {
			_time++;
		}

		if (_time == TIMEOUT_MIN || _time == TIMEOUT_MAX) {
			EmitSignal(SignalName.Timeout);
		}
	}

	/// <summary>
	/// Inverts the time direction. Emits a signal to notify other nodes that the time has been inverted.
	/// </summary>
	public void Invert() {
		GD.Print("Inverting time to " + (_invert ? "forward" : "backward") + "!");
		_invert = !_invert;
		EmitSignal(SignalName.InvertTime);
	}
}
