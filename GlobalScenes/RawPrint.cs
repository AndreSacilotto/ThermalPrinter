using Generator.Attributes;

namespace ThermalPrinter;

[SceneScript(true)]
public partial class RawPrint : Control
{
    [Export] public Button Button_Back = null!;
    [Export] public Button Button_Print = null!;
    [Export] public SpinBox Spin_FontSize = null!;

    [Export] public TextEdit Edit = null!;

    public override void _Ready()
    {
        Button_Back.Pressed += BackPressed;
        Spin_FontSize.ValueChanged += ChangeFontSize;
        Button_Print.Pressed += Print;
    }

    private void BackPressed() => GetTree().ChangeSceneToPacked(Main.Scene);

    private static readonly StringName _fontSize = "font_size";
    private void ChangeFontSize(double value) => Edit.AddThemeFontSizeOverride(_fontSize, (int)value);

    public void Print()
    {
        var escpos = Monitor.Instance.EscPos;
        if (escpos is null)
            return;

        var e = escpos.Emitter;

        e.Start();
        e.Align(EscPosNet.Emitters.PrintAlign.Left);
        e.PrintLine();
        e.PrintLine(Edit.Text);
        e.PrintLine();

        escpos.Printer.Write(e.EndSpan());
    }

}
