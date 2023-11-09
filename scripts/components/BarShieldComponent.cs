using Godot;
using System;

public partial class BarShieldComponent : ProgressBar
{
	public void SetPercent(float percent) => Value = percent;
}
