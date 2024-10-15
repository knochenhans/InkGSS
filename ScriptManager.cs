using Godot;
using System;
using GodotInk;
using System.Threading.Tasks;
using Godot.Collections;

public partial class AbstractScriptAction : GodotObject
{
	public AbstractScriptAction() { }
	public virtual Task Execute() { return Task.CompletedTask; }
}

public partial class ScriptObjectControllerAction : AbstractScriptAction
{
	public Array<ScriptObjectController> ScriptObjectControllers { get; set; }
	public string ObjectControllerID { get; set; }

	public ScriptObjectControllerAction(Array<ScriptObjectController> scriptObjectControllers, string objectControllerID) : base() { ScriptObjectControllers = scriptObjectControllers; ObjectControllerID = objectControllerID; }

	public ScriptObjectController GetScriptObjectController()
	{
		foreach (var scriptObjectController in ScriptObjectControllers)
			if (scriptObjectController.ID == ObjectControllerID)
				return scriptObjectController;
		return null;
	}

	public override Task Execute()
	{
		GD.Print($"Executing action for {ObjectControllerID}");

		var scriptObjectController = GetScriptObjectController();

		if (scriptObjectController != null)
			GD.Print($"Found script object controller for {ObjectControllerID}");

		return Task.CompletedTask;
	}
}


public partial class ScriptActionWait : AbstractScriptAction
{
	public float Seconds { get; set; }

	public ScriptActionWait(float seconds) : base() { Seconds = seconds; }
	public override Task Execute()
	{
		GD.Print($"Waiting for {Seconds} seconds");
		return Task.Delay(TimeSpan.FromSeconds(Seconds));
	}
}

public partial class ScriptActionPrint : ScriptObjectControllerAction
{
	public string Message { get; set; }

	public ScriptActionPrint(Array<ScriptObjectController> scriptObjectControllers, string objectControllerID, string message) : base(scriptObjectControllers, objectControllerID)
	{
		Message = message;
	}

	public override Task Execute()
	{
		GD.Print($"Printing message: {Message}");

		var scriptObjectController = GetScriptObjectController();

		if (scriptObjectController != null)
			scriptObjectController.Print(Message);

		return Task.CompletedTask;
	}
}

public partial class ScriptActionMoveObjectBy : ScriptObjectControllerAction
{
	public Vector2 TargetPositionDelta { get; set; }

	public ScriptActionMoveObjectBy(Array<ScriptObjectController> scriptObjectControllers, string objectControllerID, Vector2 targetPositionDelta) : base(scriptObjectControllers, objectControllerID) => TargetPositionDelta = targetPositionDelta;

	public async override Task Execute()
	{
		GD.Print($"Moving object with ID {ObjectControllerID} by {TargetPositionDelta}");

		var scriptObjectController = GetScriptObjectController();

		if (scriptObjectController != null)
			await scriptObjectController.MoveBy(TargetPositionDelta);
	}
}

public partial class ScriptActionMoveObjectTo : ScriptObjectControllerAction
{
	public Vector2 TargetPosition { get; set; }

	public ScriptActionMoveObjectTo(Array<ScriptObjectController> scriptObjectControllers, string objectControllerID, Vector2 targetPosition) : base(scriptObjectControllers, objectControllerID) => TargetPosition = targetPosition;

	public async override Task Execute()
	{
		GD.Print($"Moving object with ID {ObjectControllerID} to {TargetPosition}");

		var scriptObjectController = GetScriptObjectController();

		if (scriptObjectController != null)
			await scriptObjectController.MoveTo(TargetPosition);
	}
}

public partial class ScriptManager : GodotObject
{
	public Array<AbstractScriptAction> ActionQueue { get; set; } = new Array<AbstractScriptAction>();
	public Array<ScriptObjectController> ScriptObjects { get; set; } = new Array<ScriptObjectController>();

	public async Task RunActionQueue()
	{
		GD.Print($"Running {ActionQueue.Count} actions in queue");
		foreach (var action in ActionQueue)
		{
			// GD.Print($"Running action: {action.GetType()}");
			await action.Execute();
		}
		ActionQueue.Clear();
	}

	// External Ink functions

	// Func<string, Variant> InkGetVariable;
	// Action<string, bool> InkSetVariable;
	// Func<string, Variant> InkGetScriptVisits;

	public InkStory InkStory { get; set; }

	public ScriptManager(InkStory story)
	{
		InkStory = story;

		Bind();
	}

	private void Bind()
	{
		InkStory.BindExternalFunction("print_error", (string message) => GD.PrintErr(message));
		InkStory.BindExternalFunction("print", (string objectControllerID, string message) => ActionQueue.Add(new ScriptActionPrint(ScriptObjects, objectControllerID, message)));
		InkStory.BindExternalFunction("wait", (float seconds) => ActionQueue.Add(new ScriptActionWait(seconds)));
		InkStory.BindExternalFunction("move_object_by", (string objectControllerID, float posX, float posY) => ActionQueue.Add(new ScriptActionMoveObjectBy(ScriptObjects, objectControllerID, new Vector2(posX, posY))));
		InkStory.BindExternalFunction("move_object_to", (string objectControllerID, float posX, float posY) => ActionQueue.Add(new ScriptActionMoveObjectTo(ScriptObjects, objectControllerID, new Vector2(posX, posY))));
	}

	private void Unbind()
	{
		InkStory.UnbindExternalFunction("move_object_to");
		InkStory.UnbindExternalFunction("move_object_by");
		InkStory.UnbindExternalFunction("wait");
		InkStory.UnbindExternalFunction("print");
		InkStory.UnbindExternalFunction("print_error");
	}

	public void RegisterScriptObjectController(ScriptObjectController scriptObjectController)
	{
		ScriptObjects.Add(scriptObjectController);
	}
}