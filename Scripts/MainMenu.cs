using Godot;
using System;

public partial class MainMenu : Control
{
	private MultiplayerManager multiplayer_Manager;
	private string playerName;

	public override void _Ready()
	{
		multiplayer_Manager = GetNode<MultiplayerManager>("/root/MultiplayerManager");
	}


	private void _on_host_pressed()
	{
		// Replace with function body.
		multiplayer_Manager.Host();
		PackedScene LobbyScene = ResourceLoader.Load<PackedScene>("res://Scenes/lobby.tscn");
		GetTree().ChangeSceneToPacked(LobbyScene);
	}

	private void _on_join_pressed()
	{
		// Replace with function body.
		multiplayer_Manager.Join();
		PackedScene LobbyScene = ResourceLoader.Load<PackedScene>("res://Scenes/lobby.tscn");
		GetTree().ChangeSceneToPacked(LobbyScene);
	}


	private void _on_name_field_text_changed(string new_text)
	{
		playerName = new_text;
	}

}






