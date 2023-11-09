using Godot;
using System;

public partial class MovementAxis : Node
{
	public Vector2 Axis
    {
        get
        {
            float x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
            float y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

            Vector2 coordinates = new Vector2(x, y);
            return coordinates.Normalized();
        }
    }
}
