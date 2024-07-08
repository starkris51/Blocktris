using Godot;
using System;
using System.Collections.Generic;

public partial class BagSystem : Node
{
	private List<int> sharedBag = new();
	private readonly Dictionary<int, Queue<int>> playerBags = new();
	private readonly Dictionary<int, int> playerIndices = new();
	private readonly Random random = new();

	public void ResetBag()
	{
		sharedBag = new List<int>();
	}

	public void InitializePlayerBag(int playerId)
	{
		if (!playerBags.ContainsKey(playerId))
		{
			playerBags[playerId] = new Queue<int>();
			playerIndices[playerId] = 0;
			RefillPlayerBag(playerId);
		}
	}

	public Queue<int> GetPlayerBags(int playerId)
	{
		return playerBags[playerId];
	}

	public int GetNextPiece(int playerId)
	{
		Queue<int> playerBag = playerBags[playerId];
		if (playerBag.Count < 7)
		{
			RefillPlayerBag(playerId);
		}

		return playerBag.Dequeue();
	}

	public List<int> GetUpcomingPieces(int playerId, int count)
	{
		Queue<int> playerBag = playerBags[playerId];
		return new List<int>(playerBag).GetRange(0, Math.Min(count, playerBag.Count));
	}

	private void RefillPlayerBag(int playerId)
	{
		Queue<int> playerBag = playerBags[playerId];
		int bagCount = sharedBag.Count;
		int playerIndex = playerIndices[playerId];

		while (playerBag.Count < 14)
		{
			if (playerIndex >= bagCount)
			{
				sharedBag.AddRange(GenerateNewBag());
			}

			playerBag.Enqueue(sharedBag[playerIndex]);
			playerIndex++;
		}

		playerIndices[playerId] = playerIndex;
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

