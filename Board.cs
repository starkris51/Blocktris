using Godot;
using System;

public partial class Board : GridMap
{
	struct Cell
	{
		public bool isFilled;
		public int item;

		public int orientation;

	}

	Cell[,] BoardData = new Cell[10, 20];

	public void NewGame() {

		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 20; j++)
			{
				BoardData[i, j] = new Cell { isFilled = false, item = -1, orientation = 4 };
			}
		}

		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 20; j++)
			{
				SetCellItem(new Vector3I(i, j, 0), -1, 4);
			}
		}

	}

	public override void _Ready()
	{
		NewGame();
	}

	public override void _Process(double delta)
	{
		for (int i = 0; i < BoardData.GetLength(0); i++)
		{
			for (int j = 0; j < BoardData.GetLength(1); j++)
			{
				Vector3I Position = new(i, j, 0);
				SetCellItem(Position, BoardData[i, j].item, BoardData[i, j].orientation);
			}
		}
	}
}
