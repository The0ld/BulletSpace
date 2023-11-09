using Godot;
using Godot.Collections;
using System;

public partial class DropComponent : Node
{
    [Export]
	public Node HealthComponentProp { get; set; }

    [Export]
    public int NoDropProbability { get; set; } = 0;

    public WeightedTable WeightedTableEnemies { get; private set; } = new WeightedTable();

    public override void _Ready()
    {
        WeightedTableEnemies.AddItem("none", "EMPTY_VARIANT", NoDropProbability);

        (HealthComponentProp as HealthComponent).Died += OnDied;
    }

    public void AddDrop(string id, Variant item, int weight)
    {
        WeightedTableEnemies.AddItem(id, item, weight);
    }

    public void RemoveDrop(string id)
    {
        WeightedTableEnemies.RemoveItem(id);
    }

    public void OnDied()
	{
        var chosenItem = WeightedTableEnemies.PickItem();

		if (chosenItem == null || (string)chosenItem == "EMPTY_VARIANT") return;
		if (Owner is not Node2D) return;

		var spawnPosition = (Owner as Node2D).GlobalPosition;
        var drop = (PackedScene)chosenItem;
		var dropInstance = drop.Instantiate() as Node2D;
		
		Node entitiesLayer = GetTree().GetFirstNodeInGroup("entities");
		entitiesLayer.AddChild(dropInstance);
		
		dropInstance.GlobalPosition = spawnPosition;
	}

    public override void _ExitTree() => (HealthComponentProp as HealthComponent).Died -= OnDied;
}
