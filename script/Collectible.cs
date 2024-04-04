using Godot;
using System;

public partial class Collectible : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnCollected(Node2D body)
	{
		// Replace with function body.
		GD.Print("Collectible collected!");
		if (body is Player and not BackwardsPlayer) {
			foreach (var child in GetChildren()) {
				child.QueueFree();
			}
			QueueFree();
		}
	}
}
