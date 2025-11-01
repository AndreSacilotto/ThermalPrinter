using Generator.Attributes;

namespace ThermalPrinter;

[SceneScript(true)]
public partial class Main : Node
{
    [Export] public Button Button_Calculator = null!;
    [Export] public Button Button_Text = null!;

    [Export] public Button Button_Monitor = null!;

    public override void _Ready()
    {
        Button_Calculator.Pressed += Calculator_Pressed;
        Button_Text.Pressed += Text_Pressed;
        Button_Monitor.Pressed += Monitor_Pressed;
    }

    private void Calculator_Pressed() => GetTree().ChangeSceneToPacked(Calculator.Scene);
    private void Text_Pressed() => GetTree().ChangeSceneToPacked(RawPrint.Scene);

    private void Monitor_Pressed() => Monitor.Instance.Visible = !Monitor.Instance.Visible;
}
