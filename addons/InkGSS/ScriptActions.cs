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
		Logger.Log($"Executing action for {ObjectControllerID}", Logger.LogTypeEnum.Script);

		var scriptObjectController = GetScriptObjectController();

		if (scriptObjectController != null)
			Logger.Log($"Found script object controller for {ObjectControllerID}", Logger.LogTypeEnum.Script);

		return Task.CompletedTask;
	}
}


public partial class ScriptActionWait : AbstractScriptAction
{
	public float Seconds { get; set; }

	public ScriptActionWait(float seconds) : base() { Seconds = seconds; }
	public override Task Execute()
	{
		Logger.Log($"Waiting for {Seconds} seconds", Logger.LogTypeEnum.Script);
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
		Logger.Log($"Printing message: {Message}", Logger.LogTypeEnum.Script);

		var scriptObjectController = GetScriptObjectController();

		scriptObjectController?.OnPrint(Message);

		return Task.CompletedTask;
	}
}

public partial class ScriptActionMoveObjectBy : ScriptObjectControllerAction
{
	public Vector2 TargetPositionDelta { get; set; }

	public ScriptActionMoveObjectBy(Array<ScriptObjectController> scriptObjectControllers, string objectControllerID, Vector2 targetPositionDelta) : base(scriptObjectControllers, objectControllerID) => TargetPositionDelta = targetPositionDelta;

	public async override Task Execute()
	{
		Logger.Log($"Moving object with ID {ObjectControllerID} by {TargetPositionDelta}", Logger.LogTypeEnum.Script);

		var scriptObjectController = GetScriptObjectController();

		if (scriptObjectController != null)
			await scriptObjectController.OnMoveBy(TargetPositionDelta);
	}
}

public partial class ScriptActionMoveObjectTo : ScriptObjectControllerAction
{
	public Vector2 TargetPosition { get; set; }

	public ScriptActionMoveObjectTo(Array<ScriptObjectController> scriptObjectControllers, string objectControllerID, Vector2 targetPosition) : base(scriptObjectControllers, objectControllerID) => TargetPosition = targetPosition;

	public async override Task Execute()
	{
		Logger.Log($"Moving object with ID {ObjectControllerID} to {TargetPosition}", Logger.LogTypeEnum.Script);

		var scriptObjectController = GetScriptObjectController();

		if (scriptObjectController != null)
			await scriptObjectController.OnMoveTo(TargetPosition);
	}
}

public partial class ScriptActionDestroyObject : ScriptObjectControllerAction
{
	public ScriptActionDestroyObject(Array<ScriptObjectController> scriptObjectControllers, string objectControllerID) : base(scriptObjectControllers, objectControllerID) { }

	public override Task Execute()
	{
		Logger.Log($"Destroying object with ID {ObjectControllerID}", Logger.LogTypeEnum.Script);

		var scriptObjectController = GetScriptObjectController();

		scriptObjectController?.OnDestroy();

		return Task.CompletedTask;
	}
}