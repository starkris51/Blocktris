using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Board : GridMap
{
	[Signal]
	public delegate void GameStartedEventHandler();
	struct Cell
	{
		public bool isFilled;
		public int item;
		public int orientation;
	}


	private Cell[,] BoardData = new Cell[10, 40];

	private Tetromino _tetromino;
	private int PlayerID;
	private GameManager _gameManager;

	private BagSystem _bagSystem;

	private Node3D _boardHUD;
	private Node3D _mainScene;
	private Node3D _upcomingPieces;
	private TetrominoDisplayHud _storeTetromino;
	private LineClearText _LineClearText;

	private ComboText _ComboText;

	private MultiplayerManager multiplayerManager;

	private int BoardHeight = 40;
	private int BoardWidth = 10;

	private int StoredPiece = -1;
	private int Combo = 0;
	private bool IsCombo = false;
	private int BackToBackCombo = 0;
	private bool IsBackToBack = false;
	private bool CanStore = true;

	private int currentlyTargetingPlayer;

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

		EmitSignal(nameof(GameStarted));

		UpdateBoard();
		Combo = 0;
		RequestNextPiece();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void RequestNextPiece()
	{
		if (multiplayerManager.Multiplayer.IsServer())
		{
			// Server-side logic
			int nextPiece = _bagSystem.GetNextPiece(PlayerID);
			Rpc(nameof(HandleNextPiece), nextPiece);
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void RequestSyncBoard()
	{
		if (multiplayerManager.Multiplayer.IsServer())
		{
			BroadcastBoard(ConvertBoardToVariant());
		}
		else
		{
			RpcId(1, nameof(RequestSyncBoardFromServer)); // Assume 1 is the server ID
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void RequestSyncBoardFromServer()
	{
		BroadcastBoard(ConvertBoardToVariant());
	}


	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void HandleNextPiece(int nextPiece)
	{
		if (nextPiece != -1) // Check for valid piece
		{
			_tetromino.NewPiece(nextPiece);
			UpdateUpcomingPieces();
		}
		else
		{
			GD.PrintErr("Failed to get next piece. The bag may not have been properly refilled.");
		}
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
	}

	private void CheckLines()
	{
		int rowsCleared = 0;

		for (int i = BoardData.GetLength(1) - 1; i >= 0; i--)
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
				rowsCleared++;
			}
		}

		if (rowsCleared >= 4 || _tetromino.currentTSpin != Tetromino.TSpinType.None)
		{
			if (IsBackToBack)
			{
				BackToBackCombo++;
			}

			IsBackToBack = true;
		}
		else if (rowsCleared < 4 && rowsCleared > 0 && _tetromino.currentTSpin == Tetromino.TSpinType.None && IsBackToBack)
		{
			BackToBackCombo = 0;
			IsBackToBack = false;
		}

		if (rowsCleared > 0 || _tetromino.currentTSpin != Tetromino.TSpinType.None)
		{
			_LineClearText.RenderLineClearText(rowsCleared, _tetromino.currentTSpin, BackToBackCombo);
			BroadcastLineClear(rowsCleared, _tetromino.currentTSpin, BackToBackCombo);
		}

		_tetromino.currentTSpin = Tetromino.TSpinType.None;

		if (rowsCleared > 0 && IsCombo)
		{
			Combo++;
			_ComboText.DisplayCombo(Combo);
			BroadcastCombo(Combo);
		}
		else if (Combo <= 0 && rowsCleared > 0 && !IsCombo)
		{
			IsCombo = true;
		}
		else
		{
			IsCombo = false;
			Combo = 0;
		}

		//RpcId(currentlyTargetingPlayer, nameof(AttackPlayer), currentlyTargetingPlayer, rowsCleared);
	}

	public void BroadcastLineClear(int lineClears, Tetromino.TSpinType tspin, int BackToBackCombo)
	{
		foreach (int id in multiplayerManager.Multiplayer.GetPeers())
		{
			RpcId(id, nameof(SyncLineClear), lineClears, (int)tspin, BackToBackCombo);
		}
	}


	public void BroadcastCombo(int Combo)
	{
		foreach (int id in multiplayerManager.Multiplayer.GetPeers())
		{
			RpcId(id, nameof(SyncCombo), Combo);
		}
	}


	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SyncLineClear(int lineClears, int tspin, int BackToBackCombo)
	{
		_LineClearText.RenderLineClearText(lineClears, (Tetromino.TSpinType)tspin, BackToBackCombo);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SyncCombo(int Combo)
	{
		_ComboText.DisplayCombo(Combo);
	}


	public bool IsOccupied(int x, int y)
	{
		if (x < 0 || x >= BoardWidth || y < 0 || y >= BoardHeight)
		{
			return true;
		}

		return BoardData[x, y].isFilled;
	}

	public void PlacePiece(int piece)
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
						BoardData[boardX, boardY] = new Cell { isFilled = true, item = piece, orientation = 4 };
					}
				}
			}
		}


		CheckLines();
		Rpc(nameof(RequestSyncBoard));
		UpdateBoard();
		CanStore = true;
		RequestNextPiece();
		UpdateUpcomingPieces();

	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void BroadcastBoard(Godot.Collections.Array boardArray)
	{
		foreach (int id in multiplayerManager.Multiplayer.GetPeers())
		{
			RpcId(id, nameof(SyncBoard), boardArray);
		}
	}


	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SyncBoard(Godot.Collections.Array boardArray)
	{
		ConvertVariantToBoard(boardArray);
		UpdateBoard();
	}

	private Godot.Collections.Array ConvertBoardToVariant()
	{
		var boardArray = new Godot.Collections.Array();
		for (int i = 0; i < BoardWidth; i++)
		{
			var column = new Godot.Collections.Array();
			for (int j = 0; j < BoardHeight; j++)
			{
				var cell = new Godot.Collections.Dictionary
			{
				{ "isFilled", BoardData[i, j].isFilled },
				{ "item", BoardData[i, j].item },
				{ "orientation", BoardData[i, j].orientation }
			};
				column.Add(cell);
			}
			boardArray.Add(column);
		}
		return boardArray;
	}

	private void ConvertVariantToBoard(Godot.Collections.Array boardArray)
	{
		for (int i = 0; i < BoardWidth; i++)
		{
			var column = (Godot.Collections.Array)boardArray[i];
			for (int j = 0; j < BoardHeight; j++)
			{
				var cell = (Godot.Collections.Dictionary)column[j];
				BoardData[i, j].isFilled = (bool)cell["isFilled"];
				BoardData[i, j].item = (int)cell["item"];
				BoardData[i, j].orientation = (int)cell["orientation"];
			}
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void AttackPlayer(int Id, int amount)
	{
		foreach (Board Player in GetParent().GetChildren().Cast<Board>())
		{
			if (Player.Name == Id.ToString())
			{
				Player.CallDeferred("SendGarbage", amount);
			}
		}
	}

	public void SendGarbage(int amount)
	{
		Random random = new();
		for (int i = 0; i < amount; i++)
		{
			int openColumn = random.Next(BoardWidth);

			for (int y = 0; y < BoardHeight - 1; y++)
			{
				for (int x = 0; x < BoardWidth; x++)
				{
					BoardData[x, y] = BoardData[x, y + 1];
				}
			}

			for (int x = 0; x < BoardWidth; x++)
			{
				if (x == openColumn)
				{
					BoardData[x, BoardHeight - 1] = new Cell { isFilled = false, item = -1, orientation = 4 };
				}
				else
				{
					BoardData[x, BoardHeight - 1] = new Cell { isFilled = true, item = 8, orientation = 4 };
				}
			}
		}
		UpdateBoard();
	}

	public void UpdateUpcomingPieces()
	{
		Godot.Collections.Array<int> PlayerBag = _bagSystem.GetUpcomingPieces(PlayerID, 5);
		int index = 0;

		foreach (Node3D PieceDisplay in _upcomingPieces.GetChildren().Cast<Node3D>())
		{
			PieceDisplay.GetNode<TetrominoDisplayHud>("PieceDisplay").RenderPiece(PlayerBag[index]);
			index++;
		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void StorePiece()
	{
		if (!CanStore)
		{
			return;
		}

		int currentPiece = (int)_tetromino.GetPiece();

		if (StoredPiece == -1)
		{
			StoredPiece = currentPiece;
			_tetromino.NewPiece(_bagSystem.GetNextPiece(PlayerID));
		}
		else
		{
			_tetromino.NewPiece(StoredPiece);
			StoredPiece = currentPiece;
		}

		_storeTetromino.RenderPiece(StoredPiece);
		UpdateUpcomingPieces();
		CanStore = false;

		foreach (int id in multiplayerManager.Multiplayer.GetPeers())
		{
			RpcId(id, nameof(SyncStoredPiece), StoredPiece);
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void RequestStorePiece()
	{
		if (multiplayerManager.Multiplayer.IsServer())
		{
			StorePiece();
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SyncStoredPiece(int storedPiece)
	{
		StoredPiece = storedPiece;
		_storeTetromino.RenderPiece(StoredPiece);
		UpdateUpcomingPieces();
	}

	/*public override void _EnterTree()
	{
		Connect(nameof(BoardChanged), new Callable(this, nameof(BoardChanged)));
	}

	public override void _ExitTree()
	{
		Disconnect(nameof(BoardChanged), new Callable(this, nameof(BoardChanged)));
	}*/

	public void SetPlayerName(string name)
	{
		_boardHUD.GetNode<Label3D>("PlayerName").Text = name;
	}

	public void SetTargetingPlayer(int Target)
	{
		currentlyTargetingPlayer = Target;
	}

	public override void _Ready()
	{
		PlayerID = int.Parse(Name); ;

		_tetromino = GetNode<Tetromino>("Tetromino");
		_mainScene = GetParent().GetParent<Node3D>();
		_gameManager = _mainScene.GetNode<GameManager>("GameManager");
		_bagSystem = _gameManager.GetNode<BagSystem>("BagSystem");
		_boardHUD = GetNode<Node3D>("BoardHUD");
		_upcomingPieces = _boardHUD.GetNode<Node3D>("UpcomingPieces");
		_storeTetromino = _boardHUD.GetNode<Node3D>("StoreTetromino").GetChild<TetrominoDisplayHud>(0);
		_LineClearText = _boardHUD.GetNode<LineClearText>("LineClearTextPlacement");
		_ComboText = _boardHUD.GetNode<ComboText>("ComboTextPlacement");
		multiplayerManager = GetNode<MultiplayerManager>("/root/MultiplayerManager");
		//GetNode<MultiplayerSynchronizer>("ServerSynchronizer").SetMultiplayerAuthority(PlayerID);
		//GetNode<MultiplayerSynchronizer>("InputSynchronizer").SetMultiplayerAuthority(PlayerID);
		//_bagSystem.GetPlayerBags(PlayerID);

	}
	public override void _Process(double delta)
	{

	}

}
