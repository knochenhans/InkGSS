using Godot;
using System;
using GodotInk;

public partial class Main : Node2D
{
	[Export]
	public InkStory InkStory { get; set; }

	ScriptManager ScriptManager { get; set; }

	Button Button => GetNode<Button>("%Button");
	Label Output => GetNode<Label>("%Output");

	public override void _Ready()
	{
		ScriptManager = new ScriptManager(InkStory);

		Button.Pressed += OnButtonPressed;

		// Find and register all nodes of type ScriptObjectController
		foreach (Node node in GetTree().GetNodesInGroup("controller"))
			if (node is ScriptObjectController scriptObjectController)
				ScriptManager.RegisterScriptObjectController(scriptObjectController);
	}

	private async void OnButtonPressed()
	{
		while (ScriptManager.InkStory.CanContinue)
		{
			string storyText = ScriptManager.InkStory.Continue();
		}

		await ScriptManager.RunActionQueue();
	}
}