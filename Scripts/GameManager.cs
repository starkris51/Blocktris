using Godot;
using System;
using System.Collections.Generic;



public partial class GameManager : Node
{
	private BagSystem _bagSystem;
	private List<Player> _players = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_bagSystem = GetNode<BagSystem>("BagSystem");

		AddPlayer(1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*public override void _Process(double delta)
	{
	}*/

	/*public int NextPiece(int id)
	{
		return _bagSystem.GetNextPiece(id);
	}*/

	public void AddPlayer(int id)
	{
		PackedScene playerScene = ResourceLoader.Load<PackedScene>("res://Objects/player.tscn");
		Player player = playerScene.Instantiate<Player>();
		player.SetPlayerID(id);
		_bagSystem.InitializePlayerBag(id);
		_players.Add(player);

		AddChild(player);
	}
}
