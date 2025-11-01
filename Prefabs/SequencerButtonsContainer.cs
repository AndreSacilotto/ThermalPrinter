namespace ThermalPrinter;

public partial class SequencerButtonsContainer : CalcButtonsContainer
{
    [Export] public Button Button_Erase = null!;
    [Export] public Button Button_Load = null!;

    public override void _Ready()
    {
        base._Ready();
        ConnectButton(Button_Erase, AsciiControl.Char.SO);
        ConnectButton(Button_Load, AsciiControl.Char.SI);
    }
}
