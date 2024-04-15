using Godot;
using System;

// Reverse yourself.
// Witness time reflecting!
// While crossed self 
// Finds
// Self crossed while
// reflecting time, witness
// yourself reverse.
public partial class BackwardsPlayer : Player
{
	private Recording<Vector2> _positionRecording;	// 60 ticks per second for 10 minutes
	private Schedule<bool> _directionSchedule = new Schedule<bool>();
	private TimeKeeper _timeKeeper;
	private bool _facingLeft = false;
	private bool _invert = false;

	public override void _Ready()
	{
		_timeKeeper = GetNode<TimeKeeper>(Gamemag.TimeKeeperPath);
		_timeKeeper.InvertTime += OnTimeInversion;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_positionRecording.StartOfRecording || _positionRecording.EndOfRecording) {
			Hide();
		} else {
			Show();
		}

		// Flip the player sprite based on the direction.
		if (_facingLeft)
		{
			if (_invert)  GetNode<MeshInstance2D>("BlueShape").Rotation = 0.0f;
			if (!_invert) GetNode<MeshInstance2D>("RedShape").Rotation = 0.0f;
		}
		else
		{
			if (_invert)  GetNode<MeshInstance2D>("BlueShape").Rotation = Mathf.Pi;
			if (!_invert) GetNode<MeshInstance2D>("RedShape").Rotation = Mathf.Pi;
		}

		Position = _positionRecording.Next();
		_facingLeft = _directionSchedule.Next();
	}

	public void SetColor(bool inverted)
	{
		_invert = inverted;
		if (inverted) {
			GetNode<MeshInstance2D>("RedShape").Hide();
			GetNode<MeshInstance2D>("BlueShape").Show();
		} else {
			GetNode<MeshInstance2D>("RedShape").Show();
			GetNode<MeshInstance2D>("BlueShape").Hide();
		}
	}

	public void SetRecording(Recording<Vector2> positionRecording, Schedule<bool> directionSchedule)
	{
		_positionRecording = positionRecording;
		_directionSchedule = directionSchedule;
	}

	private void OnTimeInversion()
	{
		_positionRecording.Invert();
		_directionSchedule.Invert();
	}
}
