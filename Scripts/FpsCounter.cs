using Godot;
using System;

public partial class FPSCounter : Label
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		Text = (Engine.GetFramesPerSecond() * 5).ToString();
	}
}
