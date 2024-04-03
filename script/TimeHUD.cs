using Godot;
using System;

public partial class TimeHUD : TextureProgressBar
{
	private TimeKeeper _timeKeeper;
	private RichTextLabel _numberLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timeKeeper = GetNode<TimeKeeper>(Gamemag.TimeKeeperPath);
		_numberLabel = GetChild<RichTextLabel>(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Value = _timeKeeper.TimeProgress * 1000;
		if (_timeKeeper.Inverted) {
			_numberLabel.Text = $"<<\n{_timeKeeper.Minutes}:{String.Format("{0:00}", _timeKeeper.Seconds)}";
		} else {
			_numberLabel.Text = $">>\n{_timeKeeper.Minutes}:{String.Format("{0:00}", _timeKeeper.Seconds)}";
		}
		_numberLabel.Position = new Vector2(_timeKeeper.TimeProgress * 320 - 3, _numberLabel.Position.Y);
	}
}
