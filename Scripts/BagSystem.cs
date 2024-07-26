using Godot;
using System;
using System.Collections.Generic;

public partial class BagSystem : Node
{

	private List<int> sharedBag = new();
	private Godot.Collections.Dictionary<int, Godot.Collections.Array<int>> playerBags = new();
	private Godot.Collections.Dictionary<int, int> playerIndices = new();
	private readonly Random random = new();

	public void ResetBag()
	{
		sharedBag = new List<int>();
	}

	public void InitializePlayerBag(int playerId)
	{
		if (!playerBags.ContainsKey(playerId))
		{
			playerBags[playerId] = new Godot.Collections.Array<int>();
			playerIndices[playerId] = 0;
			RefillPlayerBag(playerId);
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SyncAllBags(Godot.Collections.Dictionary<int, Godot.Collections.Array<int>> newBags, Godot.Collections.Dictionary<int, int> newIndices)
	{
		playerBags = newBags;
		playerIndices = newIndices;
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	private void BroadcastBags()
	{
		foreach (int id in Multiplayer.GetPeers())
		{
			RpcId(id, nameof(SyncAllBags), playerBags, playerIndices);
		}
	}

	public Godot.Collections.Array<int> GetPlayerBags(int playerId)
	{
		return playerBags.ContainsKey(playerId) ? playerBags[playerId] : new Godot.Collections.Array<int>();
	}

	public int GetNextPiece(int playerId)
	{
		Godot.Collections.Array<int> playerBag = playerBags[playerId];

		// Refill the player bag if needed
		if (playerBag.Count < 7)
		{
			RefillPlayerBag(playerId);
		}

		int nextPiece = playerBag[0];
		playerBag.RemoveAt(0);

		BroadcastBags();

		return nextPiece;
	}

	public Godot.Collections.Array<int> GetUpcomingPieces(int playerId, int count)
	{
		Godot.Collections.Array<int> playerBag = GetPlayerBags(playerId);
		Godot.Collections.Array<int> upcomingPieces = new();

		for (int i = 0; i < Math.Min(count, playerBag.Count); i++)
		{
			upcomingPieces.Add(playerBag[i]);
		}

		return upcomingPieces;
	}

	private void RefillPlayerBag(int playerId)
	{
		Godot.Collections.Array<int> playerBag = playerBags[playerId];
		int bagCount = sharedBag.Count;
		int playerIndex = playerIndices[playerId];

		while (playerBag.Count < 14)
		{
			if (playerIndex >= bagCount)
			{
				sharedBag.AddRange(GenerateNewBag());
				bagCount = sharedBag.Count;
			}

			playerBag.Add(sharedBag[playerIndex]);
			playerIndex++;
		}

		playerIndices[playerId] = playerIndex;
		BroadcastBags();
	}

	private List<int> GenerateNewBag()
	{
		List<int> pieces = new() { 0, 1, 2, 3, 4, 5, 6 };
		List<int> bag = new();

		while (pieces.Count > 0)
		{
			int index = random.Next(pieces.Count);
			bag.Add(pieces[index]);
			pieces.RemoveAt(index);
		}

		return bag;
	}
}
