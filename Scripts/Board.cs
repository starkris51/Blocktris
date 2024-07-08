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

	private readonly Cell[,] BoardData = new Cell[10, 20];

	private Tetromino _tetromino;
	private Player _player;
	private int PlayerID;
	private GameManager _gameManager;

	private BagSystem _bagSystem;

	private int BoardHeight = 20;
	private int BoardWidth = 10;

	public void UpdateBoard()
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

	public void NewGame()
	{

		for (int i = 0; i < BoardWidth; i++)
		{
			for (int j = 0; j < BoardHeight; j++)
			{
				BoardData[i, j] = new Cell { isFilled = false, item = -1, orientation = 4 };
			}
		}

		UpdateBoard();

		_tetromino.NewPiece(_bagSystem.GetNextPiece(PlayerID));
	}

	public bool CheckCollision()
	{
		int[,] matrix = _tetromino.GetMatrix();
		int x = _tetromino.GetX();
		int y = _tetromino.GetY();

		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				if (matrix[i, j] == 1)
				{
					int boardX = x + i;
					int boardY = y - j;

					if (boardY < 0 || boardY >= BoardHeight)
					{
						return true;
					}

					if (boardX < 0 || boardX >= BoardWidth)
					{
						return true;
					}

					if (BoardData[boardX, boardY].isFilled)
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	private void DeleteRow(int row)
	{
		for (int i = row; i < BoardData.GetLength(1) - 1; i++)
		{
			for (int j = 0; j < BoardData.GetLength(0); j++)
			{
				BoardData[j, i] = BoardData[j, i + 1];
			}
		}

		/*for (int j = 0; j < BoardWidth; j++)
		{
			BoardData[j, BoardHeight - 1] = new Cell { isFilled = false, item = -1, orientation = 4 };
		}*/
	}

	private void CheckLines()
	{
		for (int i = 0; i < BoardData.GetLength(1); i++)
		{
			bool isRowComplete = true;

			for (int j = 0; j < BoardData.GetLength(0); j++)
			{
				if (BoardData[j, i].isFilled == false)
				{
					isRowComplete = false;
					break;
				}
			}

			if (isRowComplete)
			{
				DeleteRow(i);
				i--;
			}

		}
	}

	public void PlacePiece(int item)
	{
		int[,] matrix = _tetromino.GetMatrix();
		int x = _tetromino.GetX();
		int y = _tetromino.GetY();

		for (int i = 0; i < matrix.GetLength(0); i++)
		{
			for (int j = 0; j < matrix.GetLength(1); j++)
			{
				if (matrix[i, j] == 1)
				{
					int boardX = x + i;
					int boardY = y - j;

					if (boardX >= 0 && boardX < BoardWidth && boardY >= 0 && boardY < BoardHeight)
					{
						BoardData[boardX, boardY] = new Cell { isFilled = true, item = item, orientation = 4 };
					}
				}
			}
		}

		CheckLines();
		UpdateBoard();
		_tetromino.NewPiece(_bagSystem.GetNextPiece(PlayerID));
	}

	public override void _Ready()
	{
		_tetromino = GetNode<Tetromino>("Tetromino");
		_player = GetParent<Player>();
		_gameManager = _player.GetParent<GameManager>();
		_bagSystem = _gameManager.GetNode<BagSystem>("BagSystem");

		PlayerID = _player.GetPlayerID();
	}


	/*public override void _Process(double delta)
	{

	}*/
}
