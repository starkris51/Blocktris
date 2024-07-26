using Godot;
using System;

public partial class ComboText : Node3D
{

	private Label3D _comboText;
	private Label3D _comboNumberText;
	private AnimationPlayer _comboAnimation;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_comboText = GetNode<Label3D>("ComboText");
		_comboNumberText = GetNode<Label3D>("ComboNumberText");
		_comboAnimation = GetNode<AnimationPlayer>("AnimationPlayer");

		_comboText.Visible = false;
		_comboNumberText.Visible = false;
		_comboNumberText.Text = "";
	}

	public void DisplayCombo(int Combo)
	{
		if (Combo <= 0)
		{
			return;
		}

		_comboNumberText.Text = Combo.ToString();
		//_comboText.Scale += new Vector3(Combo, Combo, Combo);

		_comboAnimation.Play("RESET");
		_comboAnimation.Play("Combo");
		_comboAnimation.Advance(0);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*public override void _Process(double delta)
	{
	}*/
}
