using Godot;
using System;
using System.Collections.Generic;



public partial class GameManager : Node
{
	private BagSystem _bagSystem;
	private Control _MainMenu;
	private MultiplayerManager multiplayer_Manager;
	private Node3D _mainScene;
	//private Node3D PlayerSpawner;

	private int offset;
	//private CanvasLayer _canvasLayer;
	//private List<Player> _players = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_bagSystem = GetNode<BagSystem>("BagSystem");
		multiplayer_Manager = GetNode<MultiplayerManager>("/root/MultiplayerManager");
		_mainScene = GetParent<Node3D>();
		//AddPlayer(1);
		multiplayer_Manager.Connect("PlayerJoined", new Callable(this, nameof(OnPlayerJoined)));
	}

	public void Host()
	{
		int playerID;
		playerID = multiplayer_Manager.Host();

		if (playerID == 0)
		{
			return;
		}

		AddPlayer(playerID);
	}

	public async void Join()
	{
		int playerID;
		playerID = await multiplayer_Manager.Join();

		if (playerID == 0)
		{
			return;
		}

		AddPlayer(playerID);
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
		//_players.Add(player);

		//PlayerSpawner.AddChild();
		_mainScene.CallDeferred("add_child", player);
	}

	private void OnPlayerJoined(int id)
	{
		GD.Print("PlayerJoiend");
		//AddPlayer(id);
	}

	public void StartGame()
	{
		GD.Print("PlayerJoined");
	}
}
