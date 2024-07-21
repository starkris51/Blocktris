using Godot;
using System;

public partial class Lobby : Control
{
	private MultiplayerManager multiplayerManager;
	private VBoxContainer vBoxContainer;

	private int playerID;

	public override void _Ready()
	{
		multiplayerManager = GetNode<MultiplayerManager>("/root/MultiplayerManager");
		vBoxContainer = GetNode<VBoxContainer>("%PlayerList");
		multiplayerManager.Connect("UpdateLobby", new Callable(this, nameof(UpdateLobby)));
		//multiplayerManager.Connect("PlayerDisconnected", new Callable(this, nameof(UpdateLobby)));
		multiplayerManager.Connect("HostDisconnected", new Callable(this, nameof(ReturnMainMenu)));
		multiplayerManager.Connect("LobbyStartGame", new Callable(this, nameof(GameStarted)));
		UpdateLobby();
	}

	public void UpdateLobby()
	{
		PackedScene playerNameBoxScene = ResourceLoader.Load<PackedScene>("res://Ui/Lobby/PlayerLobbyName.tscn");

		foreach (Node node in vBoxContainer.GetChildren())
		{
			node.QueueFree();
		}

		foreach (System.Collections.Generic.KeyValuePair<int, string> player in multiplayerManager.GetPlayer())
		{
			Label playerLabel = playerNameBoxScene.Instantiate<Label>();
			playerLabel.Text = player.Value;
			vBoxContainer.CallDeferred("add_child", playerLabel);
		}
	}

	public void ReturnMainMenu()
	{
		PackedScene MainMenuScene = ResourceLoader.Load<PackedScene>("res://Scenes/main_menu.tscn");
		GetTree().ChangeSceneToPacked(MainMenuScene);
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
			multiplayerManager.Rpc(nameof(multiplayerManager.StartGame));
		}
	}

	public void GameStarted()
	{
		PackedScene MainScene = ResourceLoader.Load<PackedScene>("res://Scenes/main_scene.tscn");
		GetTree().ChangeSceneToPacked(MainScene);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
