using Godot;
using System;

public partial class InputHandler : Node
{

	private Board _board;
	private Tetromino _tetromino;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_board = GetParent().GetNode<Board>("TetrisBoard");
		_tetromino = _board.GetNode<Tetromino>("Tetromino");
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event.IsActionPressed("ui_left"))
		{
			_tetromino.MovePiece(-1, 0);
		}
		else if (@event.IsAction("ui_right"))
		{
			_tetromino.MovePiece(1, 0);
		}
		if (@event.IsAction("ui_down"))
		{
			_tetromino.MovePiece(0, -1);
		}
		if (@event.IsActionPressed("ui_up"))
		{
			_tetromino.RotatePiece();
		}
	}
}
