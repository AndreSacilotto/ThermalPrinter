using Generator.Attributes;

namespace ThermalPrinter;

[SceneScript(true)]
public partial class Sequencer : Control
{
    [Export] public Button Button_Back = null!;
    [Export] public Button Button_Print = null!;
    [Export] public Button Button_PrintHist = null!;
    [Export] public Button Button_HideBtns = null!;

    [Export] public Button Button_Clear = null!;
    [Export] public Button Button_Reset = null!;
    [Export] public Label Label_Memory = null!;

    [Export] public ItemList Historic = null!;
    [Export] public LineEdit Line = null!;
    [Export] public SpinBox Spin_DecimalPlaces = null!;

    [Export] public SequencerButtonsContainer CalculatorButtons = null!;

    public override void _Ready()
    {
        FocusBehaviorRecursive = FocusBehaviorRecursiveEnum.Disabled;
        //Historic.FocusBehaviorRecursive = FocusBehaviorRecursiveEnum.Enabled;
        Spin_DecimalPlaces.FocusBehaviorRecursive = FocusBehaviorRecursiveEnum.Enabled;
        Line.FocusBehaviorRecursive = FocusBehaviorRecursiveEnum.Enabled;
        Line.GrabFocus();

        Button_HideBtns.Pressed += CalculatorButtons.VisibilityInvert;
        Button_Back.Pressed += BackPressed;

        Button_Clear.Pressed += Clear;
        Button_Reset.Pressed += Reset;

        CalculatorButtons.OnInput += AddInput;
        CalculatorButtons.OnInputControl += InputControl;
        Line.GuiInput += Line_GuiInput;

        Historic.ItemSelected += HistoricItemSelected;

        Button_Print.Pressed += Print;
        Button_PrintHist.Pressed += PrintHist;
    }

    public override void _UnhandledInput(InputEvent ev)
    {
        if (ev is InputEventKey ivk && ivk.Pressed && !Line.HasFocus())
        {
            Line.GrabFocus();
            AddInput((char)ivk.Unicode);
        }
    }

    private void BackPressed() => GetTree().ChangeSceneToPacked(Main.Scene);

    private void Line_GuiInput(InputEvent ev)
    {
        if (ev is InputEventKey ivk && ivk.Pressed)
        {
            switch (ivk.Keycode)
            {
                case Key.Shift:
                case Key.Ctrl:
                case Key.Alt:
                case Key.Meta:
                case Key.Capslock:
                case Key.Numlock:
                case Key.Scrolllock:
                return;
                default:
                {
                    var c = (char)ivk.Unicode;
                    if (c == '+' || c == '-' || c == '*' || c == '/' || c == '%')
                    {
                        AddInput(c);
                        GetViewport().SetInputAsHandled();
                    }
                    break;
                }
            }
        }
    }

    private double _memory = 0d;
    public string SetMemory(double value)
    {
        _memory = value;
        var str = CalcUtil.FloatToString(value, Spin_DecimalPlaces.IntValue());
        Label_Memory.Text = str;
        return str;
    }
    public void SetMemory(string text)
    {
        _memory = double.Parse(text);
        Label_Memory.Text = text;
    }

    private void InputControl(AsciiControl.Char ch)
    {
        if (ch == AsciiControl.Char.DEL)
        {
            RemoveInput(1);
        }
        else if (ch == AsciiControl.Char.SI)
        {
            Line.Text = _memory.ToString();
        }
        else if (ch == AsciiControl.Char.SO)
        {
            SetMemory(0);
        }
    }

    public const char HISTORIC_SEPARATOR = '=';

    public void AddInput(char ch)
    {
        var str = Line.Text.Trim();
        switch (ch)
        {
            case CalcButtonsContainer.NEGATION:
            {
                if (str.Length > 0)
                {
                    Line.Text = CalcUtil.NegateStr(str);
                    Line.CaretEndColumn();
                }
                break;
            }
            case '+':
            case '-':
            case '*':
            {
                SubmitCommand(str, ch);
                break;
            }
            case '%':
            case '/':
            {
                if (!str.StartsWith('0'))
                    SubmitCommand(str, ch);
                break;
            }
            default:
            {
                Line.Text = str + ch;
                Line.CaretEndColumn();
                break;
            }
        }
    }

    private void SubmitCommand(string expression, char ch)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return;
        var evalExpression = $"{_memory}{ch}({expression})";
        GD.PrintT(evalExpression);
        if (CalcUtil.TryEvaluateExpression<double>(evalExpression, out var result))
        {
            var resultStr = SetMemory(result);
            Historic.AddItem($"{ch} {expression} {HISTORIC_SEPARATOR} {resultStr}");
            Historic.ScrollToBottom();
            Line.Clear();
        }
    }

    public void RemoveInput(int len)
    {
        var txt = Line.Text;
        var caret = Line.CaretColumn;
        if (txt.Length > 0 && caret + len <= txt.Length)
            Line.Text = txt.Remove(caret, len);
    }

    public void Clear()
    {
        Line.Clear();
        SetMemory(0);
    }

    public void Reset()
    {
        Clear();
        Historic.Clear();
    }

    private void HistoricItemSelected(long index)
    {
        var itemText = Historic.GetItemText((int)index);
        var items = itemText.Split(HISTORIC_SEPARATOR);
        if (items.Length == 2)
        {
            var value = items[1].Trim();
            Line.Text = value;
            Line.CaretEndColumn();
        }
    }


    #region Print
    private void Print()
    {
        var escpos = Monitor.Instance.EscPos;
        if (escpos is null)
            return;

        var e = escpos.Emitter;
        e.Start();
        e.Align(EscPosNet.Emitters.PrintAlign.Left);
        e.PrintLine();
        e.PrintLine(Line.Text);
        e.PrintLine();
        escpos.Printer.Write(e.EndSpan());
    }

    private void PrintHist()
    {
        var escpos = Monitor.Instance.EscPos;
        if (escpos is null)
            return;

        var e = escpos.Emitter;
        e.Start();
        e.Align(EscPosNet.Emitters.PrintAlign.Left);
        e.PrintLine();
        for (int i = 0; i < Historic.ItemCount; i++)
        {
            var item = Historic.GetItemText(i);
            e.PrintLine(item);
        }
        e.PrintLine();
        escpos.Printer.Write(e.EndSpan());
    }
    #endregion

}
