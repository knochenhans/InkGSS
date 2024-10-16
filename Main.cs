using Godot;
using System;
using GodotInk;
using Godot.Collections;

public partial class Main : Node2D
{
	[Export]
	public InkStory InkStory { get; set; }

	public ScriptManager ScriptManager { get; set; }
	public Array<ScriptObjectController> ScriptObjects { get; set; } = new Array<ScriptObjectController>();


	Button Button => GetNode<Button>("%Button");
	Label Output => GetNode<Label>("%Output");

	public override void _Ready()
	{
		ScriptManager = new ScriptManager(InkStory);

		ScriptManager.BindExternalFunction("wait", new Callable(this, MethodName.WaitFunction));
		ScriptManager.BindExternalFunction("print_error", new Callable(this, MethodName.PrintErrorFunction));
		ScriptManager.BindExternalFunction("print", new Callable(this, MethodName.PrintFunction));
		ScriptManager.BindExternalFunction("move_object_by", new Callable(this, MethodName.MoveObjectByFunction));
		ScriptManager.BindExternalFunction("move_object_to", new Callable(this, MethodName.MoveObjectToFunction));

		Button.Pressed += OnButtonPressed;

		// Find and register all nodes of type ScriptObjectController
		foreach (Node node in GetTree().GetNodesInGroup("controller"))
			if (node is ScriptObjectController scriptObjectController)
				ScriptObjects.Add(scriptObjectController);
	}

	public override void _ExitTree()
	{
		ScriptManager.UnbindExternalFunctions();
	}

	private async void OnButtonPressed()
	{
		while (ScriptManager.InkStory.CanContinue)
		{
			string storyText = ScriptManager.InkStory.Continue();
		}

		await ScriptManager.RunActionQueue();
	}

	public void WaitFunction(float seconds)
	{
		ScriptManager.AddAction(new ScriptActionWait(seconds));
	}

	public void PrintErrorFunction(string message)
	{
		GD.PrintErr(message);
	}

	public void PrintFunction(string objectControllerID, string message)
	{
		ScriptManager.AddAction(new ScriptActionPrint(ScriptObjects, objectControllerID, message));
	}

	public void MoveObjectByFunction(string objectControllerID, float posX, float posY)
	{
		ScriptManager.AddAction(new ScriptActionMoveObjectBy(ScriptObjects, objectControllerID, new Vector2(posX, posY)));
	}

	public void MoveObjectToFunction(string objectControllerID, float posX, float posY)
	{
		ScriptManager.AddAction(new ScriptActionMoveObjectTo(ScriptObjects, objectControllerID, new Vector2(posX, posY)));
	}

}