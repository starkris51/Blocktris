using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class Tetromino : GridMap
{

	public enum TetrisPiece
	{
		O = 0,
		I = 1,
		T = 2,
		L = 3,
		J = 4,
		S = 5,
		Z = 6
	}

	public enum TSpinType
	{
		None,
		Normal,
		Mini
	}

	private Board _board;

	private readonly int[,,,] TetrominoData = new int[7, 4, 4, 4]
	{
		{ // O-Tetromino
			{
				{ 0, 1, 1, 0 },
				{ 0, 1, 1, 0 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 1, 1, 0 },
				{ 0, 1, 1, 0 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 1, 1, 0 },
				{ 0, 1, 1, 0 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 1, 1, 0 },
				{ 0, 1, 1, 0 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 }
			}
		},
		{ // I-Tetromino
			{
				{ 0, 0, 0, 0 },
				{ 1, 1, 1, 1 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 0, 1, 0 },
				{ 0, 0, 1, 0 },
				{ 0, 0, 1, 0 },
				{ 0, 0, 1, 0 }
			},
			{
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 },
				{ 1, 1, 1, 1 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 1, 0, 0 },
				{ 0, 1, 0, 0 },
				{ 0, 1, 0, 0 },
				{ 0, 1, 0, 0 }
			}
		},
		{ // T-Tetromino
			{
				{ 0, 1, 0, 0 },
				{ 1, 1, 1, 0 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 1, 0, 0 },
				{ 0, 1, 1, 0 },
				{ 0, 1, 0, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 0, 0, 0 },
				{ 1, 1, 1, 0 },
				{ 0, 1, 0, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 1, 0, 0 },
				{ 1, 1, 0, 0 },
				{ 0, 1, 0, 0 },
				{ 0, 0, 0, 0 }
			}
		},
		{ // L-Tetromino
			{
				{ 0, 0, 1, 0 },
				{ 1, 1, 1, 0 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 1, 0, 0 },
				{ 0, 1, 0, 0 },
				{ 0, 1, 1, 0 },
				{ 0, 0, 0, 0 }
				},
			{
				{ 0, 0, 0, 0 },
				{ 1, 1, 1, 0 },
				{ 1, 0, 0, 0 },
				{ 0, 0, 0, 0 }
				},
			{
				{ 1, 1, 0, 0 },
				{ 0, 1, 0, 0 },
				{ 0, 1, 0, 0 },
				{ 0, 0, 0, 0 }
			}
		},
		{ // J-Tetromino
			{
				{ 1, 0, 0, 0 },
				{ 1, 1, 1, 0 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 1, 1, 0 },
				{ 0, 1, 0, 0 },
				{ 0, 1, 0, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 0, 0, 0 },
				{ 1, 1, 1, 0 },
				{ 0, 0, 1, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 1, 0, 0 },
				{ 0, 1, 0, 0 },
				{ 1, 1, 0, 0 },
				{ 0, 0, 0, 0 }
			}
		},
		{ // S-Tetromino
			{
				{ 0, 1, 1, 0 },
				{ 1, 1, 0, 0 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 1, 0, 0 },
				{ 0, 1, 1, 0 },
				{ 0, 0, 1, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 0, 0, 0 },
				{ 0, 1, 1, 0 },
				{ 1, 1, 0, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 1, 0, 0, 0 },
				{ 1, 1, 0, 0 },
				{ 0, 1, 0, 0 },
				{ 0, 0, 0, 0 }
			}
		},
		{ // Z-Tetromino
			{
				{ 1, 1, 0, 0 },
				{ 0, 1, 1, 0 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 0, 1, 0 },
				{ 0, 1, 1, 0 },
				{ 0, 1, 0, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 0, 0, 0 },
				{ 1, 1, 0, 0 },
				{ 0, 1, 1, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 1, 0, 0 },
				{ 1, 1, 0, 0 },
				{ 1, 0, 0, 0 },
				{ 0, 0, 0, 0 }
			}
		},
	};

	private readonly int[,,] srsKickNormal = new int[8, 5, 2] {
		{{0, 0}, {-1, 0}, {-1, 1}, {0, -2}, {-1, -2}}, // 0 -> 1
		{{0, 0}, {1, 0}, {1, -1}, {0, 2}, {1, 2}},    // 1 -> 0
		{{0, 0}, {1, 0}, {1, -1}, {0, 2}, {1, 2}},    // 1 -> 2
		{{0, 0}, {-1, 0}, {-1, 1}, {0, -2}, {-1, -2}},// 2 -> 1
		{{0, 0}, {1, 0}, {1, 1}, {0, -2}, {1, -2}},   // 2 -> 3
		{{0, 0}, {-1, 0}, {-1, -1}, {0, 2}, {-1, 2}}, // 3 -> 2
		{{0, 0}, {-1, 0}, {-1, -1}, {0, 2}, {-1, 2}},// 3 -> 0
		{{0, 0}, {1, 0}, {1, 1}, {0, -2}, {1, -2}}     // 0 -> 3
	};

	private readonly int[,,] srsKickI = new int[8, 5, 2] {
		{{0, 0}, {-2, 0}, {1, 0}, {-2, -1}, {1, 2}},  // 0 -> 1
		{{0, 0}, {2, 0}, {-1, 0}, {2, 1}, {-1, -2}},  // 1 -> 0
		{{0, 0}, {-1, 0}, {2, 0}, {-1, 2}, {2, -1}},  // 1 -> 2
		{{0, 0}, {1, 0}, {-2, 0}, {1, -2}, {-2, 1}},  // 2 -> 1
		{{0, 0}, {2, 0}, {-1, 0}, {2, 1}, {-1, -2}},  // 2 -> 3
		{{0, 0}, {-2, 0}, {1, 0}, {-2, -1}, {1, 2}},  // 3 -> 2
		{{0, 0}, {1, 0}, {-2, 0}, {1, -2}, {-2, 1}},  // 3 -> 0
		{{0, 0}, {-1, 0}, {2, 0}, {-1, 2}, {2, -1}}   // 0 -> 3
	};

	private readonly int[,] transitionTable = new int[4, 2] {
		{0, 7}, // 0 -> 1 (clockwise), 0 -> 3 (counterclockwise)
		{2, 5}, // 1 -> 2 (clockwise), 1 -> 0 (counterclockwise)
		{4, 3}, // 2 -> 3 (clockwise), 2 -> 1 (counterclockwise)
		{6, 1}  // 3 -> 0 (clockwise), 3 -> 2 (counterclockwise)
	};

	private readonly int[,] matrix = new int[4, 4];

	private int x = 3;
	private int y = 22;

	private int ghostY = 22;
	private int ghostX = 3;

	private int rotationState = 0;

	private bool canUseTetromino = false;
	private OmniLight3D GhostLight;

	public TSpinType currentTSpin;

	TetrisPiece currentPiece;

	public void UpdatePiece()
	{
		Clear();

		if (!canUseTetromino)
		{
			return;
		}

		for (int i = 0; i < matrix.GetLength(0); i++)
		{
			for (int j = 0; j < matrix.GetLength(1); j++)
			{
				matrix[i, j] = TetrominoData[(int)currentPiece, rotationState, j, i];
			}
		}

		int oldPieceY = y;
		ghostY = y;
		ghostX = x;

		while (!_board.CheckCollision())
		{
			y -= 1;
		}

		y++;
		ghostY = y;

		y = oldPieceY;

		GhostLight.Position = MapToLocal(new Vector3I(ghostX + 1, ghostY - 1, 0));

		for (int i = 0; i < matrix.GetLength(0); i++)
		{
			for (int j = 0; j < matrix.GetLength(1); j++)
			{
				if (matrix[i, j] == 1)
				{
					Vector3I positon = new(x + i, y - j, 0);
					SetCellItem(positon, (int)currentPiece, 4);

					if (ghostX + i == x + i && ghostY - j == y - j)
					{
						continue;
					}

					Vector3I ghostPosition = new(ghostX + i, ghostY - j, 0);
					SetCellItem(ghostPosition, 9, 0);

				}
			}
		}

	}
	public int[,] GetMatrix()
	{
		return matrix;
	}

	public int GetX()
	{
		return x;
	}

	public int GetY()
	{
		return y;
	}

	public void RotatePiece(bool clockwise)
	{
		int oldrotationstate = rotationState;

		if (!canUseTetromino)
		{
			return;
		}

		if (clockwise)
		{
			rotationState = (rotationState + 1) % 4;
		}
		else
		{
			rotationState = (rotationState - 1 + 4) % 4;
		}

		UpdatePiece();

		int transition = transitionTable[oldrotationstate, clockwise ? 0 : 1];

		for (int i = 0; i < 5; i++)
		{

			int dx = (currentPiece == TetrisPiece.I) ? srsKickI[transition, i, 0] : srsKickNormal[transition, i, 0];
			int dy = (currentPiece == TetrisPiece.I) ? srsKickI[transition, i, 1] : srsKickNormal[transition, i, 1];

			x += dx;
			y += dy;

			if (_board.CheckCollision())
			{
				x -= dx;
				y -= dy;
				continue;
			}
			else
			{
				currentTSpin = IsTSpin();
				UpdatePiece();
				return;
			}
		}

		rotationState = oldrotationstate;
		UpdatePiece();
	}

	public void MovePiece(int dx, int dy)
	{
		if (!canUseTetromino)
		{
			return;
		}

		x += dx;
		y += dy;

		if (_board.CheckCollision())
		{
			x -= dx;
			y -= dy;

			if (dy < 0)
			{
				_board.PlacePiece((int)currentPiece);
			}
		}
		else
		{
			UpdatePiece();
		}
	}

	public TSpinType IsTSpin()
	{
		if (currentPiece != TetrisPiece.T) return TSpinType.None;

		int filledcorners = 0;

		int[,] TPieceCorners = new int[4, 2]
		{
			{ x, y },
			{ x + 2, y },
			{ x, y - 2 },
			{ x + 2, y - 2 }
		};

		for (int i = 0; i < 4; i++)
		{
			if (_board.IsOccupied(TPieceCorners[i, 0], TPieceCorners[i, 1]))
			{
				filledcorners++;
			}
		}

		if (filledcorners >= 3)
		{
			return TSpinType.Normal;

		}
		else if (filledcorners == 2)
		{
			return TSpinType.Mini;
		}

		return TSpinType.None;
	}

	public void HardDrop()
	{
		while (!_board.CheckCollision())
		{
			y -= 1;
		}
		y += 1;
		_board.PlacePiece((int)currentPiece);
	}

	public TetrisPiece GetPiece()
	{
		return currentPiece;
	}

	public void NewPiece(int Piece)
	{
		if (!canUseTetromino)
		{
			canUseTetromino = true;
		}

		currentPiece = (TetrisPiece)Piece;
		y = 22;
		x = 3;
		rotationState = 0;
		UpdatePiece();
	}

	public override void _Ready()
	{
		_board = GetParent<Board>();
		GhostLight = GetChild<OmniLight3D>(0);
	}


}
