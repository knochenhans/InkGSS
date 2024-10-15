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

public partial class ScriptActionPrint : AbstractScriptAction
{
	[Signal]
	public delegate void PrintEventHandler(string text);

	public string Text { get; set; }

	public ScriptActionPrint(string text) : base() { Text = text; }
	public override Task Execute()
	{
		// GD.Print(Text);
		EmitSignal(SignalName.Print, Text);
		return Task.CompletedTask;
	}
}

public partial class ScriptActionMoveObjectBy : AbstractScriptAction
{
	public Array<ScriptObjectController> ScriptObjectControllers { get; set; }
	public string ObjectControllerID { get; set; }

	public Vector2 TargetPositionDelta { get; set; }

	public ScriptActionMoveObjectBy(Array<ScriptObjectController> scriptObjectControllers, string objectControllerID, Vector2 targetPositionDelta) : base()
	{
		ScriptObjectControllers = scriptObjectControllers;
		ObjectControllerID = objectControllerID;
		TargetPositionDelta = targetPositionDelta;
	}

	public async override Task Execute()
	{
		GD.Print($"Moving {ObjectControllerID} by {TargetPositionDelta}");

		foreach (var scriptObjectController in ScriptObjectControllers)
			if (scriptObjectController.ID == ObjectControllerID)
				await scriptObjectController.MoveBy(TargetPositionDelta);
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
	public Action<string> PrintError;
	public Action<string> Print;
	public Action<float> InkWait;
	public Action<string, float, float> InkMoveObjectBy;

	// Func<string, Variant> InkGetVariable;
	// Action<string, bool> InkSetVariable;
	// Func<string, Variant> InkGetScriptVisits;

	public InkStory InkStory { get; set; }

	public ScriptManager(InkStory story, Action<string> print)
	{
		InkStory = story;

		PrintError = (string message) => GD.PrintErr(message);
		Print = (string message) =>
		{
			ScriptActionPrint action = new ScriptActionPrint(message);
			action.Print += (string text) => print(text);
			ActionQueue.Add(action);
		};
		InkWait = (float seconds) => ActionQueue.Add(new ScriptActionWait(seconds));
		InkMoveObjectBy = (objectControllerID, posX, posY) => ActionQueue.Add(new ScriptActionMoveObjectBy(ScriptObjects, objectControllerID, new Vector2(posX, posY)));

		Bind();
	}

	private void Bind()
	{
		InkStory.BindExternalFunction("print_error", PrintError);
		InkStory.BindExternalFunction("print", Print);
		InkStory.BindExternalFunction("wait", InkWait);
		InkStory.BindExternalFunction("move_object_by", InkMoveObjectBy);
	}

	private void Unbind()
	{
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