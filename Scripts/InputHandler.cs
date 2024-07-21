using Godot;
using System;

public partial class InputHandler : MultiplayerSynchronizer
{

	private Board _board;
	private MultiplayerManager multiplayerManager;

	private Tetromino _tetromino;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		multiplayerManager = GetNode<MultiplayerManager>("/root/MultiplayerManager");
		_board = GetParent<Board>();
		_tetromino = _board.GetNode<Tetromino>("Tetromino");

		SetMultiplayerAuthority(int.Parse(_board.Name));
	}

	public override void _Process(double delta)
	{
		if (multiplayerManager.Multiplayer.HasMultiplayerPeer() &&
			GetMultiplayerAuthority() == multiplayerManager.Multiplayer.GetUniqueId())
		{
			HandleInput();
		}
	}

	private void HandleInput()
	{
		if (Input.IsActionJustPressed("ui_left"))
		{
			_tetromino.Rpc(nameof(_tetromino.MovePiece), -1, 0);
		}
		if (Input.IsActionJustPressed("ui_right"))
		{
			_tetromino.Rpc(nameof(_tetromino.MovePiece), 1, 0);
		}
		if (Input.IsActionJustPressed("ui_down"))
		{
			_tetromino.Rpc(nameof(_tetromino.MovePiece), 0, -1);
		}
		if (Input.IsActionJustPressed("ui_up"))
		{
			_tetromino.Rpc(nameof(_tetromino.RotatePiece), true);
		}
		if (Input.IsActionJustPressed("ui_test"))
		{
			_board.Rpc(nameof(_board.StorePiece));
		}
		if (Input.IsActionJustPressed("ui_space"))
		{
			_tetromino.Rpc(nameof(_tetromino.HardDrop));
		}
	}
}
