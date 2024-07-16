using Godot;
using System;

public partial class MultiplayerManager : Node
{

	[Export]
	const int serverPort = 8080;
	[Export]
	const string serverIP = "127.0.0.1";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*public override void _Process(double delta)
	{
	}*/

	public void Host()
	{
		GD.Print("NewHost");

		ENetMultiplayerPeer server_peer = new();

		var error = server_peer.CreateServer(serverPort, 2);
		if (error != Error.Ok)
		{
			GD.Print("Could not host");
			return;
		}

		server_peer.Host.Compress(ENetConnection.CompressionMode.Fastlz);

		Multiplayer.MultiplayerPeer = server_peer;
	}

	public void Join()
	{
		GD.Print("Join game");
		ENetMultiplayerPeer client_peer = new();
		client_peer.CreateClient(serverIP, serverPort);
		client_peer.Host.Compress(ENetConnection.CompressionMode.Fastlz);
		Multiplayer.MultiplayerPeer = client_peer;

	}

	public void AddPlayer(int id)
	{
		GD.Print(id);
	}

}
