using Godot;
using System;

public partial class Camera : Camera2D
{
	private Node _parent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_parent = GetParent<Node>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 direction = Input.GetVector("debug.camera_left",
											"debug.camera_right",
											"debug.camera_up",
											"debug.camera_down");
		if (direction != Vector2.Zero) {
			Reparent(_parent.GetParent());
			Position += direction * (Input.IsActionPressed("debug.camera_speed") ? 5.0f : 2.5f);
		}
		if (Input.IsActionJustPressed("debug.camera_reset")) {
			Reparent(_parent);
			Position = Vector2.Zero;
		}
		Position = Position.Round();
	}
}
