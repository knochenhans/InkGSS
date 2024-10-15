using Godot;
using System;
using System.Threading.Tasks;

public partial class ScriptObjectController : Node2D
{
	[Export] public string ID { get; set; }

	Node2D Parent => GetParent<Node2D>();

	public override void _Ready()
	{
	}

	public async Task MoveBy(Vector2 targetPositionDelta)
	{
		Position = Parent.Position;

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(Parent, "position", Position + targetPositionDelta, 1.0f);
		await ToSignal(tween, "finished");
	}
}
