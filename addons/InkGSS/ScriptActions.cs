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