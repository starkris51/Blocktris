using Godot;
using System;

public partial class InputHandler : MultiplayerSynchronizer
{

	private Board _board;
	private MultiplayerManager multiplayerManager;

	private Tetromino _tetromino;
	private Timer _PieceFallTimer;
	private Timer _DelayAutoShiftTimer;
	private bool IsStart;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		multiplayerManager = GetNode<MultiplayerManager>("/root/MultiplayerManager");
		_board = GetParent<Board>();
		_PieceFallTimer = GetNode<Timer>("PieceFallTimer");
		_DelayAutoShiftTimer = GetNode<Timer>("DelayAutoShiftTimer");
		_board.GameStarted += InputStartup;
		_tetromino = _board.GetNode<Tetromino>("Tetromino");
		SetMultiplayerAuthority(int.Parse(_board.Name));
		_DelayAutoShiftTimer.SetMultiplayerAuthority(int.Parse(_board.Name));

	}

	public void InputStartup()
	{
		IsStart = true;
		if (multiplayerManager.Multiplayer.IsServer())
		{
			_PieceFallTimer.WaitTime = 1.0f;
			_PieceFallTimer.OneShot = false;
			_PieceFallTimer.Timeout += DropPiece;
			_PieceFallTimer.Start();
		}
	}

	public void DropPiece()
	{
		_tetromino.Rpc(nameof(_tetromino.MovePiece), 0, -1);

	}

	public override void _Process(double delta)
	{
		if (!IsStart)
		{
			return;
		}

		if (multiplayerManager.Multiplayer.HasMultiplayerPeer() &&
			GetMultiplayerAuthority() == multiplayerManager.Multiplayer.GetUniqueId())
		{
			HandleInput();
		}
	}

	private void HandleInput()
	{
		/*if (Input.IsActionJustPressed("Left") || Input.IsActionJustPressed("Right"))
		{
			_DelayAutoShiftTimer.WaitTime = 0.1f;
			_DelayAutoShiftTimer.Start();
		}*/

		if (Input.IsActionPressed("Left"))
		{
			if (_DelayAutoShiftTimer.TimeLeft == 0 && !_DelayAutoShiftTimer.IsStopped())
			{
				_tetromino.Rpc(nameof(_tetromino.MovePiece), -1, 0);
			}
		}
		else if (Input.IsActionPressed("Right"))
		{
			_tetromino.Rpc(nameof(_tetromino.MovePiece), 1, 0);
		}
		if (Input.IsActionPressed("Down"))
		{
			_tetromino.Rpc(nameof(_tetromino.MovePiece), 0, -1);
		}
		if (Input.IsActionJustPressed("Clockwise"))
		{
			_tetromino.Rpc(nameof(_tetromino.RotatePiece), true);
		}
		if (Input.IsActionJustPressed("CounterClockwise"))
		{
			_tetromino.Rpc(nameof(_tetromino.RotatePiece), false);
		}
		if (Input.IsActionJustPressed("StorePiece"))
		{
			_board.Rpc(nameof(_board.StorePiece));
		}
		if (Input.IsActionJustPressed("HardDrop"))
		{
			_tetromino.Rpc(nameof(_tetromino.HardDrop));
		}
	}
}
