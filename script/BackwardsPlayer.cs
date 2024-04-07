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
	private TimeKeeper _timeKeeper;

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
		Position = _positionRecording.Next();
	}

	public void SetColor(bool inverted)
	{
		if (inverted) {
			GetNode<MeshInstance2D>("RedShape").Hide();
			GetNode<MeshInstance2D>("BlueShape").Show();
		} else {
			GetNode<MeshInstance2D>("RedShape").Show();
			GetNode<MeshInstance2D>("BlueShape").Hide();
		}
	}

	public void SetRecording(Recording<Vector2> recording)
	{
		_positionRecording = recording;
	}

	private void OnTimeInversion()
	{
		_positionRecording.Invert();
	}
}
