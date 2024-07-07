using Godot;
using System;



public partial class GameManager : Node3D
{

	Godot.Node Player = ResourceLoader.Load<PackedScene>("res://Objects/Player.tscn").Instantiate();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddChild(Player);
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
