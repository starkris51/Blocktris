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

	private int offset = -10;
	//private CanvasLayer _canvasLayer;
	//private List<Player> _players = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_bagSystem = GetNode<BagSystem>("BagSystem");
		multiplayer_Manager = GetNode<MultiplayerManager>("/root/MultiplayerManager");
		_mainScene = GetParent<Node3D>();
		PlayerSpawner = _mainScene.GetNode<Node3D>("Players");

		foreach (KeyValuePair<int, string> player in multiplayer_Manager.GetPlayer())
		{
			AddPlayer(player.Key, player.Value);
		}
	}

	public void AddPlayer(int id, string _)
	{
		PackedScene BoardScene = ResourceLoader.Load<PackedScene>("res://Objects/board.tscn");
		Board Board = BoardScene.Instantiate<Board>();
		Board.Name = id.ToString();
		_bagSystem.InitializePlayerBag(id);
		Board.Position += new Vector3(offset, 0, 0);
		offset += 20;
		PlayerSpawner.CallDeferred("add_child", Board);
		Board.CallDeferred("NewGame");
	}
}
