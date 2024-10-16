using System;
using System.Threading.Tasks;
using Godot;
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