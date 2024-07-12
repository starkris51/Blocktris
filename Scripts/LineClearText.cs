using Godot;
using System;

public partial class LineClearText : Node3D
{

	private Label3D _lineClearText;
	private Label3D _TSpinText;
	private Label3D _B2BText;
	private AnimationPlayer _LineClearAnimation;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_lineClearText = GetNode<Label3D>("LineClearText");
		_TSpinText = _lineClearText.GetNode<Label3D>("TSpinText");
		_B2BText = _lineClearText.GetNode<Label3D>("B2BText");
		_LineClearAnimation = GetNode<AnimationPlayer>("LineClear");

		_lineClearText.Text = "";
		_TSpinText.Text = "";
		_B2BText.Text = "";
	}

	public void RenderLineClearText(int lineClears, Tetromino.TSpinType TSpinType, int b2b)
	{
		_lineClearText.Text = lineClears switch
		{
			0 => "",
			1 => "Single",
			2 => "Double",
			3 => "Triple",
			4 => "Quad",
			_ => "Skibidi Toilet",
		};

		_TSpinText.Text = TSpinType switch
		{
			Tetromino.TSpinType.None => "",
			Tetromino.TSpinType.Normal => "T Spin",
			Tetromino.TSpinType.Mini => "T Spin Mini",
			_ => "Retard Spin"
		};

		if (b2b > 0)
		{
			_B2BText.Text = "Back 2 Back " + b2b.ToString() + "x";
		}
		else
		{
			_B2BText.Text = "";
		}

		_LineClearAnimation.Play("RESET");
		_LineClearAnimation.Play("LineClear");
		_LineClearAnimation.Advance(0);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*public override void _Process(double delta)
	{
	}*/
}
