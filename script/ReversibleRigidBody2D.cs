using Godot;
using System;

public partial class ReversibleRigidBody2D : RigidBody2D
{
	private Recording<Transform2D> _transformBuffer = new Recording<Transform2D>();
	private bool _recording = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    public override void _Process(double delta)
    {
    }

	// TODO: Fix teleporting bug. Might have something to do with what data is being pulled
	//  from the recording buffer being outdated relative to the player
    public override void _PhysicsProcess(double delta)
	{
		// Check if the rigid body is colliding with something other than the tilemap.
		//  If the rigid body is colliding with the player, start recording the rigid body's transform.
		foreach (var body in GetCollidingBodies()) {
			if (body is Player && body is not BackwardsPlayer) {
				GD.Print("Collided with " + body.Name + " at " + body.GlobalPosition);
				_recording = true;
				_transformBuffer.SetEndPoint(_transformBuffer.Inverted);
				break;
			}
		}

		if (_recording) {
			// Record the player's position every 1/60th of a second.
			_transformBuffer.Append(Transform);
		} else {
			LinearVelocity = Vector2.Zero;
			AngularVelocity = 0;
			Inertia = 0;
			Transform = _transformBuffer.Next();
			MoveAndCollide(Vector2.Zero);
			if (_transformBuffer.EndOfRecording || _transformBuffer.StartOfRecording) {
				GD.Print("Recording ended!");
				_recording = true;
			}
		}
	}

	/// Called when the player inverts time. This will playback the transform buffer in reverse
	//  until the relative end of the buffer is reached or the player touches the rigid body.
	private void OnMirrorEnter(Node2D body)
	{
		if (body is Player and not BackwardsPlayer) {
			_recording = false;
			_transformBuffer.SetEndPoint(_transformBuffer.Inverted);
			_transformBuffer.Invert();
		}
	}


}
