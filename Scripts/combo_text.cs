using Godot;
using System;

public partial class combo_text : Node3D
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
	}

	public void DisplayCombo(int Combo)
	{

		_comboText.Text = Combo.ToString();
		//_comboText.Scale += new Vector3(Combo, Combo, Combo);

		_comboAnimation.Advance(0);
		_comboAnimation.Play("RESET");
		_comboAnimation.Play("Combo");
		_comboAnimation.Play("ComboNumberText");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*public override void _Process(double delta)
	{
	}*/
}
