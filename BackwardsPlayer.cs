using Godot;
using System;

// Reverse yourself.
// Witness time reflecting!
// While crossed self 
// Finds
// Self crossed while
// reflecting time, witness
// yourself reverse.
public partial class BackwardsPlayer : CharacterBody2D
{
	private Recording<Vector2> _recording;	// 60 ticks per second for 10 minutes

	public override void _Ready()
	{
		// GD.Print("BackwardsPlayer is ready!");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_recording.StartOfRecording || _recording.EndOfRecording) {
			Hide();
		} else {
			Show();
		}
		Position = _recording.Next();
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
		_recording = recording;
	}

	private void OnMirrorEnter(Node2D body)
	{
		GD.Print("BackwardsPlayer entered the mirror!");
		QueueFree();
	}
}
