using Godot;
using System;

public partial class InputComponent : LineEdit
{
	public override void _Ready() => TextChanged += OnTextChanged;

	public void OnTextChanged(string name)
	{
		var caretPos = CaretColumn;
		Text = name.ToUpper();
		CaretColumn = caretPos;
	}

	public override void _ExitTree() => TextChanged -= OnTextChanged;
}
