using Godot;
using System;

public partial class Player : Node3D
{

	private Board _board;
	private int playerID;

	public void SetPlayerID(int id)
	{
		playerID = id;
	}
	public int GetPlayerID()
	{
		return playerID;
	}

	// Called when the node enters the scene tree for the first time.
	/*public override void _Ready()
	{
		Name = playerID.ToString();
		//Rpc("SyncPlayerID", playerID);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}*/
}
