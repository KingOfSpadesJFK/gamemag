using Godot;
using System;

/* 
 * S O M A R
 * O M I R A
 * M I R I M
 * A R I M O
 * R A M O S
 */

public partial class Player : CharacterBody2D
{
	[Export]
	public const float Speed = 150.0f;
	public const float JumpVelocity = -400.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private Recording<Vector2> _positionRecording = new Recording<Vector2>();
	private static PackedScene _backwardsPlayerScene = ResourceLoader.Load<PackedScene>("res://node/backwards_player.tscn");
	private static Vector2 _direction = new Vector2(0, 0);
	private TimeKeeper _timeKeeper;

	public override void _Ready()
	{
		_timeKeeper = GetNode<TimeKeeper>(Gamemag.TimeKeeperPath);
		if (_timeKeeper is not null) {
			_timeKeeper.InvertTime += OnTimeInversion;
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("debug.invert_time")) {
			_timeKeeper?.Invert();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor()) {
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		_direction.Y = 0;
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			_direction = direction;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, 15);
		}

		// Flip the player sprite based on the direction.
		if (_direction.X > 0)
		{
			GetNode<MeshInstance2D>("MeshInstance2D").Rotation = Mathf.Pi;
		}
		else
		{
			GetNode<MeshInstance2D>("MeshInstance2D").Rotation = 0.0f;
		}

		Velocity = velocity;
		MoveAndSlide();

		// Record the player's position every 1/60th of a second.
		_positionRecording.Append(Position);
	}

	private void OnTimeInversion()
	{
		if (_timeKeeper is not null) {
			// Prepare the recording to pass into the BackwardsPlayer.
			_positionRecording.StartPlaybackAtEnding();
			_positionRecording.Invert();

			// Instantiate the BackwardsPlayer and pass the inverted recording to it.
			var bPlayer = _backwardsPlayerScene.Instantiate<BackwardsPlayer>();
			bPlayer.SetColor(!_timeKeeper.Inverted);
			bPlayer.SetRecording(_positionRecording);
			GetParent().AddChild(bPlayer);
			_positionRecording = new Recording<Vector2>();
			
			Position += 5.0f * _direction;
		}
	}
}
