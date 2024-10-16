using Godot;
using System;
using System.Threading.Tasks;

public partial class ScriptObjectController : Node2D
{
	[Export] public string ID { get; set; }

	GodotObject Parent => GetParent<GodotObject>();

	public async Task MoveBy(Vector2 targetPositionDelta)
	{
		if (Parent is Node2D node2D)
		{
			Vector2 targetPosition = node2D.Position + targetPositionDelta;
			await MoveTo(targetPosition);
		}
	}

	public async Task MoveTo(Vector2 targetPosition)
	{
		if (Parent is Node2D node2D)
		{
			Position = node2D.Position;

			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(node2D, "position", targetPosition, 1.0f);
			await ToSignal(tween, "finished");
		}
	}

	public void Print(string message)
	{
		if (Parent is Label label)
			label.Text += $"{message}\n";
	}
}
