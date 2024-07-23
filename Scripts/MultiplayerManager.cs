using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class MultiplayerManager : Node
{
	[Export] const int serverPort = 8080;
	[Export] const string serverIP = "localhost";

	[Signal] public delegate void UpdateLobbyEventHandler(int playerId);
	[Signal] public delegate void ConnectionEstablishedEventHandler();
	[Signal] public delegate void PlayerDisconnectedEventHandler(int playerId);
	[Signal] public delegate void HostDisconnectedEventHandler();
	[Signal] public delegate void LobbyStartGameEventHandler();
	[Signal] public delegate void StopGameEventHandler();

	public enum GameState
	{
		Lobby = 0,
		InGame = 1,
	}

	public Godot.Collections.Dictionary<int, string> playerNames = new();
	public GameState CurrentGameState { get; private set; } = GameState.Lobby;

	public override void _Ready()
	{
		Multiplayer.PeerDisconnected += OnPeerDisconnected;
		Multiplayer.ServerDisconnected += OnHostDisconnected;
	}

	public void Host(string playerName)
	{
		GD.Print("Hosting game");

		ENetMultiplayerPeer serverPeer = new();
		var error = serverPeer.CreateServer(serverPort, 2);
		if (error != Error.Ok)
		{
			GD.Print("Could not host");
			return;
		}

		serverPeer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = serverPeer;

		CurrentGameState = GameState.Lobby;
		LocalSetPlayerName(Multiplayer.GetUniqueId(), playerName);
		EmitSignal("ConnectionEstablished");
	}

	public async void Join(string playerName)
	{
		GD.Print("Joining game");

		ENetMultiplayerPeer clientPeer = new();
		clientPeer.CreateClient(serverIP, serverPort);
		clientPeer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);

		Multiplayer.MultiplayerPeer = clientPeer;

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

		RpcId(1, nameof(RemoteSetPlayerName), Multiplayer.GetUniqueId(), playerName);
		EmitSignal("ConnectionEstablished");
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	private void BroadcastExistingPlayers()
	{
		foreach (var id in Multiplayer.GetPeers())
		{
			RpcId(id, nameof(SyncPlayerName), playerNames);
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void RemoteSetPlayerName(int playerId, string name)
	{
		LocalSetPlayerName(playerId, name);

		if (Multiplayer.IsServer())
		{
			BroadcastExistingPlayers();
		}
	}

	private void LocalSetPlayerName(int playerId, string name)
	{
		if (playerNames.ContainsKey(playerId))
		{
			playerNames[playerId] = name;
		}
		else
		{
			playerNames.Add(playerId, name);
		}

		EmitSignal(nameof(UpdateLobby));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void SyncPlayerName(Godot.Collections.Dictionary<int, string> names)
	{
		playerNames = names;
		EmitSignal(nameof(UpdateLobby));
	}

	public Godot.Collections.Dictionary<int, string> GetPlayer()
	{
		return playerNames;
	}

	public void OnPeerDisconnected(long id)
	{
		playerNames.Remove((int)id);

		if (Multiplayer.IsServer())
		{
			BroadcastExistingPlayers();
			EmitSignal(nameof(UpdateLobby));
		}

		EmitSignal(nameof(StopGame));
	}

	public void OnHostDisconnected()
	{
		GD.Print("Host disconnected");

		// Clear player names and reset game state
		playerNames.Clear();
		CurrentGameState = GameState.Lobby;

		// Cleanup server peer if necessary
		if (Multiplayer.IsServer())
		{
			EmitSignal(nameof(StopGame));
			// Cleanup server peer
			Multiplayer.MultiplayerPeer?.CallDeferred("queue_free");
			Multiplayer.MultiplayerPeer = null;
		}
		EmitSignal(nameof(HostDisconnected));
		EmitSignal(nameof(StopGame));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void StartGame()
	{
		CurrentGameState = GameState.InGame;
		EmitSignal(nameof(LobbyStartGame));
	}

	public string GetPlayerName(int playerId)
	{
		return playerNames.ContainsKey(playerId) ? playerNames[playerId] : "Unknown";
	}
}
