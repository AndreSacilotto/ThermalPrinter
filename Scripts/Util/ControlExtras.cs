
namespace ThermalPrinter;

public static class ControlExtras
{
    public static Task AsTask(this SignalAwaiter awaiter)
    {
        var tcs = new TaskCompletionSource();
        awaiter.OnCompleted(tcs.SetResult);
        return tcs.Task;
    }

    public static SignalAwaiter WaitNextFrame(Node node) => node.ToSignal(node.GetTree(), "process_frame");

    public static void VisibilityInvert(this CanvasItem item) => item.Visible = !item.Visible;

    public static int IntValue(this SpinBox spin) => (int)Math.Round(spin.Value);

    public static void CaretEndColumn(this LineEdit line) => line.CaretColumn = line.Text.Length;

    public static void ScrollToBottom(this ItemList itemList)
    {
        Task.Run(async () =>
        {
            await WaitNextFrame(itemList);
            var scroll = itemList.GetVScrollBar();
            scroll.Value = scroll.MaxValue;
        });
    }

}
