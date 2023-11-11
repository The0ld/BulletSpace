using Godot;
using System;

public partial class BarShieldComponent : ProgressBar, IBar
{
	public void SetPercent(float percent) => Value = percent;
}
