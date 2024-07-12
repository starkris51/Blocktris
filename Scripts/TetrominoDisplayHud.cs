using Godot;
using System;

public partial class TetrominoDisplayHud : GridMap
{

	private readonly int[,,] TetrominoHudData = new int[,,]
	{
		{
			{1,1,0,0},
			{1,1,0,0}
		},
		{
			{0,0,0,0},
			{1,1,1,1}
		},
		{
			{0,1,0,0},
			{1,1,1,0}
		},
		{
			{0,0,1,0},
			{1,1,1,0}
		},
		{
			{1,0,0,0},
			{1,1,1,0}
		},
		{
			{0,1,1,0},
			{1,1,0,0}
		},
		{
			{1,1,0,0},
			{0,1,1,0}
		},
	};

	private readonly int[,] matrix = new int[4, 2];

	private Vector3 orginalPosition;

	public override void _Ready()
	{
		orginalPosition = Position;
	}

	public void ClearPiece()
	{
		for (int i = 0; i < matrix.GetLength(0); i++)
		{
			for (int j = 0; j < matrix.GetLength(1); j++)
			{
				matrix[i, j] = 0;
				SetCellItem(new Vector3I(i, j, 0), -1, 4);

			}
		}
	}

	public void RenderPiece(int piece)
	{
		Position = orginalPosition;
		ClearPiece();

		if (piece == 1)
		{
			float newY = Position.Y + 0.35f;
			Position = new Vector3(Position.X, newY, Position.Z);
		}
		else if (piece == 0)
		{
			float newX = Position.X + 0.75f;
			Position = new Vector3(newX, Position.Y, Position.Z);
		}
		else
		{
			float newX = Position.X + 0.35f;
			Position = new Vector3(newX, Position.Y, Position.Z);
		}

		for (int i = 0; i < matrix.GetLength(0); i++)
		{
			for (int j = 0; j < matrix.GetLength(1); j++)
			{
				matrix[i, j] = TetrominoHudData[piece, j, i];

				if (matrix[i, j] == 1)
				{
					SetCellItem(new Vector3I(i, 3 - j - 2, 0), piece, 4);
				}
			}
		}
	}

}
