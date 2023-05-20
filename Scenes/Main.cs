using Godot;
using System;

public partial class Main : Node
{
	
	[Export]
	public PackedScene MobScene { get; set; }

	private int _score;
	private hud _hud;
	private Player _player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_hud = GetNode<hud>("HUD");
		_hud.StartGame += NewGame;
		_player = GetNode<Player>("Player");
		_player.Hit += GameOver;
	}
	
	private void GameOver()
	{
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		_hud.ShowGameOver();
		GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("DeathSound").Play();
	}
	
	private void NewGame()
	{
		GetTree().CallGroup("Enemies", Node.MethodName.QueueFree);
		_score = 0;
		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();
		
		_hud.UpdateScore(_score);
		_hud.ShowMessage("Get Ready!");
		GetNode<AudioStreamPlayer>("Music").Play();
	}
	
	private void _on_score_timer_timeout()
	{
		_score++;
		_hud.UpdateScore(_score);
	}
	
	private void _on_start_timer_timeout()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}
	private void _on_mob_timer_timeout()
	{
		// Note: Normally it is best to use explicit types rather than the `var`
		// keyword. However, var is acceptable to use here because the types are
		// obviously Mob and PathFollow2D, since they appear later on the line.

		// Create a new instance of the Mob scene.
		Enemy mob = MobScene.Instantiate<Enemy>();

		// Choose a random location on Path2D.
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();

		// Set the mob's direction perpendicular to the path direction.
		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

		// Set the mob's position to a random location.
		mob.Position = mobSpawnLocation.Position;

		// Add some randomness to the direction.
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;

		// Choose the velocity.
		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);

		// Spawn the mob by adding it to the Main scene.
		AddChild(mob);
	}
}
