using Godot;
using System;

public partial class BarHealthComponent : ProgressBar, IBar
{
	public void SetPercent(float percent) => Value = percent;
}
