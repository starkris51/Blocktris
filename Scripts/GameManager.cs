using Godot;
using System;
using System.Collections.Generic;



public partial class GameManager : Node
{
	private BagSystem _bagSystem;
	private Control _MainMenu;
	private MultiplayerManager multiplayer_Manager;
	private CanvasLayer _canvasLayer;
	//private List<Player> _players = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_bagSystem = GetNode<BagSystem>("BagSystem");
		multiplayer_Manager = GetNode<MultiplayerManager>("/root/MultiplayerManager");
		_canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
		_MainMenu = _canvasLayer.GetNode<Control>("MainMenu");

		//AddPlayer(1);
	}

	public void Host()
	{
		multiplayer_Manager.Host();
		_MainMenu.Hide();
	}

	public void Join()
	{
		multiplayer_Manager.Join();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*public override void _Process(double delta)
	{
	}*/

	/*public int NextPiece(int id)
	{
		return _bagSystem.GetNextPiece(id);
	}*/

	/*public void AddPlayer(int id)
	{
		PackedScene playerScene = ResourceLoader.Load<PackedScene>("res://Objects/player.tscn");
		Player player = playerScene.Instantiate<Player>();
		player.SetPlayerID(id);
		_bagSystem.InitializePlayerBag(id);
		_players.Add(player);

		AddChild(player);
	}*/
}
