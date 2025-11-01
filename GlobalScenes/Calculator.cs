using Generator.Attributes;

namespace ThermalPrinter;

[SceneScript(true)]
public partial class Calculator : Control
{
    [Export] public Button Button_Back = null!;
    [Export] public Button Button_Print = null!;
    [Export] public Button Button_PrintHist = null!;
    [Export] public Button Button_HideBtns = null!;

    [Export] public Button Button_Clear = null!;
    [Export] public Button Button_Reset = null!;
    [Export] public Label Label_Feedback = null!;

    [Export] public ItemList Historic = null!;
    [Export] public LineEdit Line = null!;
    [Export] public SpinBox Spin_DecimalPlaces = null!;

    [Export] public CalculatorButtonsContainer CalculatorButtons = null!;

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

        Line.TextSubmitted += SubmitCommand;
        Line.TextChanged += PreviewCommand;

        Spin_DecimalPlaces.ValueChanged += (val) => PreviewCommand(Line.Text);

        CalculatorButtons.OnInput += AddInput;
        CalculatorButtons.OnInputControl += InputControl;

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

    private void InputControl(AsciiControl.Char ch)
    {
        if (ch == AsciiControl.Char.ETX)
            SubmitCommand(Line.Text);
        else if (ch == AsciiControl.Char.DEL)
            RemoveInput(1);
    }

    public void AddInput(char ch)
    {
        var str = Line.Text.Trim();
        if (ch == CalcButtonsContainer.NEGATION)
            Line.Text = CalcUtil.NegateStr(str);
        else
            Line.Text = str + ch;
        Line.CaretEndColumn();
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
        Label_Feedback.Text = "";
    }

    public void Reset()
    {
        Clear();
        Historic.Clear();
    }

    private void SubmitCommand(string command)
    {
        var txt = CalcUtil.EvaluateExpressionStr(command, Spin_DecimalPlaces.IntValue());
        if (txt is not null)
        {
            Historic.AddItem(command);
            Historic.ScrollToBottom();
            Label_Feedback.Text = txt;
            Line.Text = txt;
            Line.CaretEndColumn();
        }
    }

    private void HistoricItemSelected(long index)
    {
        var command = Historic.GetItemText((int)index);
        var txt = CalcUtil.EvaluateExpressionStr(command, Spin_DecimalPlaces.IntValue());
        if (txt is not null)
        {
            Label_Feedback.Text = command;
            Line.Text = txt;
            Line.CaretEndColumn();
        }
    }

    private void PreviewCommand(string command)
    {
        var txt = CalcUtil.EvaluateExpressionStr(command, Spin_DecimalPlaces.IntValue());
        if (txt is not null)
            Label_Feedback.Text = txt;
        else
            Label_Feedback.Text = "";
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
