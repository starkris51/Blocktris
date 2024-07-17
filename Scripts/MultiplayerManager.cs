using Godot;
using System;
using System.Threading.Tasks;

public partial class MultiplayerManager : Node
{

	[Export]
	const int serverPort = 8080;
	[Export]
	const string serverIP = "localhost";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*public override void _Process(double delta)
	{
	}*/

	public int Host()
	{

		ENetMultiplayerPeer server_peer = new();

		var error = server_peer.CreateServer(serverPort, 2);
		if (error != Error.Ok)
		{
			GD.Print("Could not host");
			return 0;
		}

		server_peer.Host.Compress(ENetConnection.CompressionMode.Fastlz);

		Multiplayer.MultiplayerPeer = server_peer;

		return Multiplayer.MultiplayerPeer.GetUniqueId();
	}

	public async Task<int> Join()
	{
		ENetMultiplayerPeer client_peer = new();
		client_peer.CreateClient(serverIP, serverPort);

		client_peer.Host.Compress(ENetConnection.CompressionMode.Fastlz);

		Multiplayer.MultiplayerPeer = client_peer;

		int timeout = 5000; // 5 seconds timeout
		int elapsed = 0;
		while (Multiplayer.MultiplayerPeer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connecting)
		{
			await Task.Delay(100);
			elapsed += 100;

			if (elapsed >= timeout)
			{
				GD.Print("Connection timed out");
				Multiplayer.MultiplayerPeer = null;
				return 0;
			}
		}
		if (Multiplayer.MultiplayerPeer.GetConnectionStatus() != MultiplayerPeer.ConnectionStatus.Connected)
		{
			GD.Print("Failed to connect to server");
			return 0;
		}

		return Multiplayer.MultiplayerPeer.GetUniqueId();
	}

	public void AddPlayer(int id)
	{
		GD.Print(id);
	}

}
