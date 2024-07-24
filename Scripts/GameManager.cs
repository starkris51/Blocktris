using Godot;
using System;
using System.Collections.Generic;
using System.Linq;



public partial class GameManager : Node
{
	private BagSystem _bagSystem;
	private Control _MainMenu;
	private MultiplayerManager multiplayer_Manager;
	private Node3D _mainScene;
	private Node3D PlayerSpawner;

	private int offset = -12;
	//private CanvasLayer _canvasLayer;
	//private List<Player> _players = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_bagSystem = GetNode<BagSystem>("BagSystem");
		multiplayer_Manager = GetNode<MultiplayerManager>("/root/MultiplayerManager");
		multiplayer_Manager.StopGame += QuitGame;
		_mainScene = GetParent<Node3D>();
		PlayerSpawner = _mainScene.GetNode<Node3D>("Players");

		foreach (KeyValuePair<int, string> player in multiplayer_Manager.GetPlayer())
		{
			AddPlayer(player.Key, player.Value);
		}
	}


	public void QuitGame()
	{
		GD.Print("Game is stopping...");

		// Ensure all player nodes are removed safely
		if (PlayerSpawner != null && PlayerSpawner.IsInsideTree())
		{
			PlayerSpawner.CallDeferred("queue_free");
		}

		// Handle scene change with deferred call to avoid timing issues
		GetTree().CallDeferred("change_scene_to_packed", ResourceLoader.Load<PackedScene>("res://Scenes/main_menu.tscn"));
	}

	public void AddPlayer(int id, string playername)
	{
		PackedScene BoardScene = ResourceLoader.Load<PackedScene>("res://Objects/board.tscn");
		Board Board = BoardScene.Instantiate<Board>();
		Board.Name = id.ToString();
		_bagSystem.InitializePlayerBag(id);
		Board.Position += new Vector3(offset, 0, 0);
		offset += 20;
		PlayerSpawner.CallDeferred("add_child", Board);
		Board.CallDeferred("SetPlayerName", playername);
		Board.CallDeferred("SetTargetingPlayer", multiplayer_Manager.Multiplayer.GetPeers()[0]);
		Board.CallDeferred("NewGame");
	}
}
