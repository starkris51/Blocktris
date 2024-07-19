using Godot;
using System;
using System.Threading.Tasks;

public partial class MultiplayerManager : Node
{

	[Export]
	const int serverPort = 8080;
	[Export]
	const string serverIP = "localhost";

	[Signal]
	public delegate void PlayerJoinedEventHandler(int playerId);

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

		ENetMultiplayerPeer server_peer = new();
		GD.Print("Hosted");

		var error = server_peer.CreateServer(serverPort, 2);
		if (error != Error.Ok)
		{
			GD.Print("Could not host");
			return;
		}

		server_peer.Host.Compress(ENetConnection.CompressionMode.Fastlz);

		Multiplayer.MultiplayerPeer = server_peer;
		Multiplayer.MultiplayerPeer.Connect("peer_connected", new Callable(this, nameof(OnPeerConnected)));
	}

	public async void Join()
	{
		GD.Print("Join");

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
				return;
			}
		}
		if (Multiplayer.MultiplayerPeer.GetConnectionStatus() != MultiplayerPeer.ConnectionStatus.Connected)
		{
			GD.Print("Failed to connect to server");
			return;
		}

	}
	private void OnPeerConnected(int id)
	{
		EmitSignal("PlayerJoined", id);
	}

}
