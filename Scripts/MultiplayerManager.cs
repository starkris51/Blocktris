using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;


public partial class MultiplayerManager : Node
{

	[Export]
	const int serverPort = 8080;
	[Export]
	const string serverIP = "localhost";

	[Signal]
	public delegate void UpdateLobbyEventHandler(int playerId);

	[Signal]
	public delegate void ConnectionEstablishedEventHandler();

	[Signal]
	public delegate void PlayerDisconnectedEventHandler(int playerId);

	[Signal]
	public delegate void HostDisconnectedEventHandler();

	[Signal]
	public delegate void LobbyStartGameEventHandler();

	[Signal]
	public delegate void StopGameEventHandler();


	/*[Signal]
	public delegate void PlayerNameSyncedEventHandler();*/
	public enum GameState
	{
		Lobby = 0,
		InGame = 1,
	}

	public Godot.Collections.Dictionary<int, string> playerNames = new();
	public GameState CurrentGameState { get; private set; } = GameState.Lobby;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;
		Multiplayer.ServerDisconnected += OnHostDisconnected;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*public override void _Process(double delta)
	{
	}*/

	public void Host(string playerName)
	{

		ENetMultiplayerPeer server_peer = new();
		GD.Print("Hosted");

		var error = server_peer.CreateServer(serverPort, 2);
		if (error != Error.Ok)
		{
			GD.Print("Could not host");
			return;
		}

		server_peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);

		Multiplayer.MultiplayerPeer = server_peer;

		CurrentGameState = GameState.Lobby;
		LocalSetPlayerName(Multiplayer.GetUniqueId(), playerName);

		EmitSignal("ConnectionEstablished");
	}

	public async void Join(string playerName)
	{
		GD.Print("Join");

		ENetMultiplayerPeer client_peer = new();
		client_peer.CreateClient(serverIP, serverPort);

		client_peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);

		Multiplayer.MultiplayerPeer = client_peer;
		//Multiplayer.MultiplayerPeer.Connect("peer_connected", new Callable(this, nameof(OnPeerConnected)));

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
		GD.Print(Multiplayer.GetPeers().Stringify());
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


	}

	public void OnHostDisconnected()
	{
		EmitSignal(nameof(HostDisconnected));
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
