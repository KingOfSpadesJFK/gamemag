using Godot;
using System;

public partial class Mirror : Node2D
{
	private TimeKeeper _timeKeeper;

	public override void _Ready()
	{
		_timeKeeper = GetNode<TimeKeeper>(Gamemag.TimeKeeperPath);
	}

	public void OnBodyCrossing(Node2D body)
	{
		if (body is Player and not BackwardsPlayer) {
			_timeKeeper.Invert();
		}
	}
}
