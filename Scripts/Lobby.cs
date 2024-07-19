using Godot;
using System;

public partial class Lobby : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	private void _on_start_ready()
	{
		// Replace with function body.
		SetMultiplayerAuthority(1);
	}

	private void _on_start_pressed()
	{
		// Replace with function body.
		if (IsMultiplayerAuthority())
		{
			GD.Print("Start");
		}
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}




