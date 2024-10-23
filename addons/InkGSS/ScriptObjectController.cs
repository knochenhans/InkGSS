using Godot;
using System;
using System.Threading.Tasks;

public partial class ScriptObjectController : Node2D
{
	[Export] public string ID { get; set; }

	public Node Parent => GetParent<Node>();

	public async Task OnMoveBy(Vector2 targetPositionDelta)
	{
		if (Parent is Node2D node2D)
		{
			Vector2 targetPosition = node2D.Position + targetPositionDelta;
			await OnMoveTo(targetPosition);
		}
	}

	public async Task OnMoveTo(Vector2 targetPosition)
	{
		if (Parent is Node2D node2D)
		{
			Position = node2D.Position;

			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(node2D, "position", targetPosition, 1.0f);
			await ToSignal(tween, "finished");
		}
	}

	public void OnPrint(string message)
	{
		if (Parent is Label label)
			label.Text += $"{message}\n";
	}

	public void OnHide()
	{
		if (Parent is Node2D node2D)
			node2D.Visible = false;
	}

	public void OnShow(bool visible)
	{
		if (Parent is Node2D node2D)
			node2D.Visible = visible;
	}

	public void OnDestroy()
	{
		if (Parent is Node2D node2D)
			node2D.QueueFree();
	}
}
