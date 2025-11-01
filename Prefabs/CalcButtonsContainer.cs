namespace ThermalPrinter;

public partial class CalcButtonsContainer : Control
{
    public const char NEGATION = (char)170;

    public event Action<char>? OnInput = null;
    public event Action<AsciiControl.Char>? OnInputControl = null;

    [Export] public Button Button_1 = null!;
    [Export] public Button Button_2 = null!;
    [Export] public Button Button_3 = null!;
    [Export] public Button Button_4 = null!;
    [Export] public Button Button_5 = null!;
    [Export] public Button Button_6 = null!;
    [Export] public Button Button_7 = null!;
    [Export] public Button Button_8 = null!;
    [Export] public Button Button_9 = null!;
    [Export] public Button Button_0 = null!;
    [Export] public Button Button_Comma = null!;

    [Export] public Button Button_Add = null!;
    [Export] public Button Button_Sub = null!;
    [Export] public Button Button_Div = null!;
    [Export] public Button Button_Mult = null!;
    [Export] public Button Button_Rem = null!;

    [Export] public Button Button_Neg = null!;
    [Export] public Button Button_Del = null!;

    public override void _Ready()
    {
        ConnectButton(Button_0, '0');
        ConnectButton(Button_1, '1');
        ConnectButton(Button_2, '2');
        ConnectButton(Button_3, '3');
        ConnectButton(Button_4, '4');
        ConnectButton(Button_5, '5');
        ConnectButton(Button_6, '6');
        ConnectButton(Button_7, '7');
        ConnectButton(Button_8, '8');
        ConnectButton(Button_9, '9');
        ConnectButton(Button_Comma, '.');

        ConnectButton(Button_Add, '+');
        ConnectButton(Button_Sub, '-');
        ConnectButton(Button_Mult, '*');
        ConnectButton(Button_Div, '/');
        ConnectButton(Button_Rem, '%');
        ConnectButton(Button_Neg, NEGATION);

        ConnectButton(Button_Del, AsciiControl.Char.DEL);
    }

    protected void ConnectButton(Button button, char input) => button.Pressed += () => OnInput?.Invoke(input);
    protected void ConnectButton(Button button, AsciiControl.Char input) => button.Pressed += () => OnInputControl?.Invoke(input);
}

