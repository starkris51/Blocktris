using Godot;
using System;

public partial class InputHandler : MultiplayerSynchronizer
{

	private Board _board;
	private MultiplayerManager multiplayerManager;

	private Tetromino _tetromino;
	private Timer _PieceFallTimer;
	private Timer _DelayAutoShiftTimer;
	private Timer _InputDelayTimer;
	private Timer _DropDelayTimer;

	private bool IsStart;
	private bool LeftPressed;
	private bool RightPressed;
	private bool DownPressed;

	private const float DASInitialDelay = 0.17f; // Initial delay before auto-shifting
	private const float DASInterval = 0.04f; // Interval between auto-shift actions
	private const float DropInterval = 0.04f; // Interval between drop-shift actions


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		multiplayerManager = GetNode<MultiplayerManager>("/root/MultiplayerManager");
		_board = GetParent<Board>();
		_PieceFallTimer = GetNode<Timer>("PieceFallTimer");
		_DelayAutoShiftTimer = GetNode<Timer>("DelayAutoShiftTimer");
		_InputDelayTimer = GetNode<Timer>("InputDelayTimer");
		_DropDelayTimer = GetNode<Timer>("DropDelayTimer");
		_board.GameStarted += InputStartup;
		_tetromino = _board.GetNode<Tetromino>("Tetromino");
		SetMultiplayerAuthority(int.Parse(_board.Name));

		_InputDelayTimer.SetMultiplayerAuthority(int.Parse(_board.Name));
		_InputDelayTimer.OneShot = true;
		_InputDelayTimer.Timeout += DASDelay;

		_DelayAutoShiftTimer.SetMultiplayerAuthority(int.Parse(_board.Name));
		_DelayAutoShiftTimer.OneShot = false;
		_DelayAutoShiftTimer.Timeout += OnDASTimeout;

		_DropDelayTimer.SetMultiplayerAuthority(int.Parse(_board.Name));
		_DropDelayTimer.OneShot = true;
		_DropDelayTimer.Timeout += OnDropTimeout;

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

		if (Input.IsActionJustPressed("Left"))
		{
			_tetromino.Rpc(nameof(_tetromino.MovePiece), -1, 0);

			LeftPressed = true;
			RightPressed = false;
			StartDAS();
		}
		else if (Input.IsActionJustReleased("Left"))
		{
			LeftPressed = false;
			if (!RightPressed)
			{
				StopDAS();
			}
		}
		if (Input.IsActionJustPressed("Right"))
		{
			_tetromino.Rpc(nameof(_tetromino.MovePiece), 1, 0);

			LeftPressed = false;
			RightPressed = true;
			StartDAS();
		}
		else if (Input.IsActionJustReleased("Right"))
		{
			RightPressed = false;
			if (!LeftPressed)
			{
				StopDAS();
			}
		}


		if (Input.IsActionJustPressed("Down"))
		{
			_tetromino.Rpc(nameof(_tetromino.MovePiece), 0, -1);
			DownPressed = true;
			StartDropShift();
		}
		else if (Input.IsActionJustReleased("Down"))
		{
			DownPressed = false;
			StopDropShift();
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
			_board.Rpc(nameof(_board.RequestStorePiece));
		}
		if (Input.IsActionJustPressed("HardDrop"))
		{
			_tetromino.Rpc(nameof(_tetromino.HardDrop));
		}
	}

	private void StartDAS()
	{
		_InputDelayTimer.WaitTime = DASInitialDelay;
		_InputDelayTimer.Start();
	}

	private void StopDAS()
	{
		_InputDelayTimer.Stop();
		_DelayAutoShiftTimer.Stop();
	}

	private void DASDelay()
	{
		_DelayAutoShiftTimer.WaitTime = DASInterval;
		_DelayAutoShiftTimer.Start();
	}

	private void OnDASTimeout()
	{
		if (LeftPressed)
		{
			_tetromino.Rpc(nameof(_tetromino.MovePiece), -1, 0);
		}
		else if (RightPressed)
		{
			_tetromino.Rpc(nameof(_tetromino.MovePiece), 1, 0);
		}
	}

	private void StartDropShift()
	{
		_DropDelayTimer.WaitTime = DASInitialDelay;
		_DropDelayTimer.Start();
	}

	private void StopDropShift()
	{
		_DropDelayTimer.Stop();
	}

	private void OnDropTimeout()
	{
		if (DownPressed)
		{
			_tetromino.Rpc(nameof(_tetromino.MovePiece), 0, -1);
			_DropDelayTimer.WaitTime = DropInterval;
			_DropDelayTimer.Start();
		}
	}
}
