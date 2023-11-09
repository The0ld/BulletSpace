using Godot;
using System;

public partial class BarHealthComponent : ProgressBar
{
	public void SetPercent(float percent) => Value = percent;
}
