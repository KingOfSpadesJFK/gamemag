using Godot;
using System;

/// <summary>
///  The Singleton for the game. This should be used for keeping track of paths to important nodes
///   such as the TimeKeeper and the world root.
/// </summary>
public partial class Gamemag : Node
{
	public static NodePath TimeKeeperPath = new NodePath("/root/Gamemag/TimeKeeper");
	public static NodePath WorldRootPath = new NodePath("/root/Gamemag/SubViewportContainer/SubViewport/Node2D");
	public static NodePath RootPath = new NodePath("/root/Gamemag");
}
