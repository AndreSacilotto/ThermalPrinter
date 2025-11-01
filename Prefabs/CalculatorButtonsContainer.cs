namespace ThermalPrinter;

public partial class CalculatorButtonsContainer : CalcButtonsContainer
{
    [Export] public Button Button_LeftParenthesis = null!;
    [Export] public Button Button_RightParenthesis = null!;
    [Export] public Button Button_Submit = null!;

    public override void _Ready()
    {
        base._Ready();
        ConnectButton(Button_LeftParenthesis, '(');
        ConnectButton(Button_RightParenthesis, ')');
        ConnectButton(Button_Submit, AsciiControl.Char.ETX);
    }
}
