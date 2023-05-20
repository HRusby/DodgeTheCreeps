using Godot;
using System;

public partial class hud : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();
	
	public void ShowMessage(string text){
		var message = GetNode<Label>("Message");
		message.Text = text;
		message.Show();

		GetNode<Timer>("MessageTimer").Start();
	}

	async public void ShowGameOver()
	{
		ShowMessage("Game Over!");

		var messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, Timer.SignalName.Timeout);
		var message = GetNode<Label>("Message");
		message.Text = "Dodge the\nCreeps!";
		message.Show();

		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		GetNode<Button>("StartButton").Show();
	}

	public void UpdateScore(int score){
		GetNode<Label>("ScoreLabel").Text = score.ToString();
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var timer = GetNode<Timer>("MessageTimer");
		timer.Timeout += OnMessageTimerTimedOut;

		var startButton = GetNode<Button>("StartButton");
		startButton.Pressed += OnStartButtonPressed;
	}

	public void OnMessageTimerTimedOut()
	{
		GetNode<Label>("Message").Hide();
	}
	
	public void OnStartButtonPressed()
	{
		GetNode<Button>("StartButton").Hide();
		EmitSignal(SignalName.StartGame);
	}
}
