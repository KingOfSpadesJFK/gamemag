using Godot;
using System;

public partial class Collectable : Node2D
{
	private Schedule<bool> _visibilitySchedule = new Schedule<bool>();
	private bool _recording = true;
	private TimeKeeper _timeKeeper;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timeKeeper = GetNode<TimeKeeper>(Gamemag.TimeKeeperPath);
		_timeKeeper.InvertTime += OnTimeInversion;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (Visible) {
			Position = new Vector2(0, Mathf.Sin(Time.GetTicksMsec() / 1000.0f) * 2);
		}

		if (_recording) {
			_visibilitySchedule.Append(Visible);
		} else {
			Visible = _visibilitySchedule.Next();
			if (_visibilitySchedule.EndOfRecording || _visibilitySchedule.StartOfRecording) {
				_recording = true;
			}
		}

		// GD.Print("Starting point: " + _visibilitySchedule.StartPoint + 
		// 		 ", End point: " + _visibilitySchedule.EndPoint +
		// 		 ", Current point: " + (_visibilitySchedule.TimeTicks + _visibilitySchedule.TimeMinutes * 3600) +
		// 		 ", Current data: " + _visibilitySchedule.Seek(0));
	}

	private void OnCollected(Node2D body)
	{
		// Replace with function body.
		if (Visible && body is Player and not BackwardsPlayer) {
			GD.Print("Collectible collected!");
			// foreach (var child in GetChildren()) {
			// 	child.QueueFree();
			// }
			// QueueFree();
			Visible = false;
			if (!_recording ) {
				_recording = true;
				_visibilitySchedule.SetEndPoint(_visibilitySchedule.Inverted);
			}
		}
	}

	private void OnTimeInversion()
	{
		if (_recording) {
			_visibilitySchedule.StartPlaybackAtEnding();
		}

		_recording = false;
		_visibilitySchedule.Invert();
	}
}
