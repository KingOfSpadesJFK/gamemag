using Godot;
using System;
using System.Linq;

// This class keeps track of the time and records the player's position.
public partial class TimeKeeper : Node
{
	private const int MAX_MINUTE_TICKS = 3600;
	[Export] public int StartingTime = 5;
	[Export] public int TimeLimitLower = 0;
	[Export] public int TimeLimitUpper = 10;

	public bool Inverted { get => _invert; }
	public int RecordingCount { get => _recordingCount; }
	public int Time { get => _time; }
	public int Minutes { get => _time / MAX_MINUTE_TICKS; }
	public int Seconds { get => Ticks / 60; }
	public int Ticks { get => _time % MAX_MINUTE_TICKS; }
	public float TimeProgress { get => (float)_time / (TimeLimitUpper * MAX_MINUTE_TICKS); }

	private int _time = 0;
	private bool _invert = false;
	private bool _paused = false;
	private int _recordingCount = 0;

	[Signal] public delegate void InvertTimeEventHandler();
	[Signal] public delegate void TimeoutEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_time = StartingTime * MAX_MINUTE_TICKS;
	}

    // Every physics tick, update the time.
    public override void _PhysicsProcess(double delta)
	{
		if (!_paused) {
			if (_invert) {
				_time--;
			} else {
				_time++;
			}
		}

		if (_time == TimeLimitLower * MAX_MINUTE_TICKS || _time == TimeLimitUpper * MAX_MINUTE_TICKS) {
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
