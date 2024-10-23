using Godot;
using System;
using GodotInk;
using Godot.Collections;

public partial class CustomScriptManager : ScriptManager
{
	public CustomScriptManager(InkStory inkStory) : base(inkStory)
	{
		BindExternalFunction("wait", new Callable(this, MethodName.WaitFunction));
		BindExternalFunction("print_error", new Callable(this, MethodName.PrintErrorFunction));
		BindExternalFunction("print", new Callable(this, MethodName.PrintFunction));
		BindExternalFunction("move_object_by", new Callable(this, MethodName.MoveObjectByFunction));
		BindExternalFunction("move_object_to", new Callable(this, MethodName.MoveObjectToFunction));
		BindExternalFunction("destroy_object", new Callable(this, MethodName.DestroyObjectFunction));
		BindExternalFunction("get_game_var", new Callable(this, MethodName.GetGameVarFunction));
	}

	public void WaitFunction(float seconds)
	{
		QueueAction(new ScriptActionWait(seconds));
	}

	public void PrintErrorFunction(string message)
	{
		GD.PrintErr(message);
	}

	public void PrintFunction(string objectControllerID, string message)
	{
		QueueAction(new ScriptActionPrint(ScriptObjects, objectControllerID, message));
	}

	public void MoveObjectByFunction(string objectControllerID, float posX, float posY)
	{
		QueueAction(new ScriptActionMoveObjectBy(ScriptObjects, objectControllerID, new Vector2(posX, posY)));
	}

	public void MoveObjectToFunction(string objectControllerID, float posX, float posY)
	{
		QueueAction(new ScriptActionMoveObjectTo(ScriptObjects, objectControllerID, new Vector2(posX, posY)));
	}

	public void DestroyObjectFunction(string objectControllerID)
	{
		QueueAction(new ScriptActionDestroyObject(ScriptObjects, objectControllerID));
	}

	public string GetGameVarFunction(string varName)
	{
		if (varName == "test_var")
			return "okay";
		return "not okay";
	}
}

public partial class Main : Node2D
{
	[Export] public InkStory InkStory { get; set; }

	public ScriptManager ScriptManager { get; set; }

	Button Button => GetNode<Button>("%Button");
	Label Output => GetNode<Label>("%Output");

	public override void _Ready()
	{
		ScriptManager = new CustomScriptManager(InkStory);

		Button.Pressed += OnButtonPressed;

		// Find and register all nodes of type ScriptObjectController
		foreach (Node node in GetTree().GetNodesInGroup("controller"))
			if (node is ScriptObjectController scriptObjectController)
				ScriptManager.RegisterScriptObject(scriptObjectController);
	}

	public override void _ExitTree()
	{
		ScriptManager.Cleanup();
	}

	private async void OnButtonPressed()
	{
		ScriptManager.InkStory.ResetState();
		while (ScriptManager.InkStory.CanContinue)
		{
			string storyText = ScriptManager.InkStory.Continue();
		}

		await ScriptManager.RunActionQueue();

		GD.Print(ScriptManager.GetVariable("test_string"));
		GD.Print(ScriptManager.GetVariable("test_number"));
	}
}