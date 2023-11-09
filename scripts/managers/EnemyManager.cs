using Godot;
using System;
using OnReadyCs;
using Godot.Collections;

public partial class EnemyManager : Path2D
{
	[ExportGroup("Enemies")]
	[Export]
	public PackedScene BasicEnemy { get; set; }

	[Export]
	public PackedScene SmallAsteroid { get; set; }

	[Export]
	public PackedScene BigAsteroid { get; set; }

	[ExportGroup("Wait Between")]
	[Export]
	public float Init { get; set; }

	[Export]
	public float End { get; set; }

	[OnReady("PathFollow2D")]
	public PathFollow2D PathFollow2DProp { get; set; }

	[OnReady("TimerSpawn")]
	public Timer TimerSpawn { get; set; }

	public Random RandomWaitTime { get; private set; } = new Random();

	public WeightedTable WeightedTableEnemies { get; private set; } = new WeightedTable();

    public override void _Ready()
    {
		this.InitializeOnReadyFields();

		TimerSpawn.Timeout += OnTimeout;

		WeightedTableEnemies.AddItem("basic_enemy", BasicEnemy, 15);
		WeightedTableEnemies.AddItem("small_asteroid", SmallAsteroid, 8);
		WeightedTableEnemies.AddItem("big_asteroid", BigAsteroid, 2);
    }

    public override void _Process(double delta) => PathFollow2DProp.Progress = PathFollow2DProp.Progress + 80 * (float)delta;

	private float SetNewRandomWaitTime()
	{
		float randomNumber = (float)RandomWaitTime.NextDouble();
		float scaledRandomNumber = Init + (randomNumber * (End - Init));

		return scaledRandomNumber;
	}

	private void OnTimeout()
	{
		if (BasicEnemy == null) return;

		var chosenEnemy = (PackedScene)WeightedTableEnemies.PickItem();
		if (chosenEnemy == null) return;

		var enemyInstance = chosenEnemy.Instantiate();
		(enemyInstance as Node2D).GlobalPosition = PathFollow2DProp.GlobalPosition;
		(enemyInstance as Node2D).Scale = new Vector2(-1, 1);
		
		GetTree().GetFirstNodeInGroup("entities").AddChild(enemyInstance);

		TimerSpawn.WaitTime = SetNewRandomWaitTime();
	}

    public override void _ExitTree() => TimerSpawn.Timeout -= OnTimeout;
}
