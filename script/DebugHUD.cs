using Godot;
using System;

public partial class DebugHUD : RichTextLabel
{
	private TimeKeeper _timeKeeper;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timeKeeper = GetNode<TimeKeeper>("/root/Control/TimeKeeper");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var seconds = String.Format("{0:00}", _timeKeeper.Seconds);
		var ticks = String.Format("{0:00}", _timeKeeper.Ticks % 60);
		Text  = $"Time: {_timeKeeper.Minutes}:{seconds}.{ticks}";
		Text += "\n" + (_timeKeeper.Inverted ? "<<" : ">>");
	}
}
