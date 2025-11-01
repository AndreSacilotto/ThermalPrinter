using System.Text;
using EscPosNet;

namespace ThermalPrinter;

public partial class Monitor : Window
{
    public static Monitor Instance { get; private set; } = null!;

    [Export] public Label LabelStatus = null!;

    public override void _Ready()
    {
        if (Instance is not null)
        {
            GD.PrintErr($"More than one {nameof(Monitor)} exists");
            Instance.QueueFree();
        }
        Instance = this;

        CloseRequested += Hide;

        //if (!UseArgs(Util.UtilOS.Args))
        //    GetTree().Quit(-1);
    }

    public EscPosPrinter? EscPos { get; private set; }

    public void SetPrinter(EscPosPrinter printer)
    {
        EscPos = printer;
        printer.Printer.StatusChanged += Printer_StatusChanged;
    }

    public void UnsetPrinter(EscPosPrinter printer)
    {
        if (EscPos == printer)
            EscPos = null;
        printer.Printer.StatusChanged -= Printer_StatusChanged;
    }

    private void Printer_StatusChanged(object? sender, PrinterStatus.ASBEventsArgs ev)
    {
        static string ToStatusText(bool value) => value switch {
            true => "Yes",
            false => "No",
        };
        var e = ev.Status;

        var sb = new StringBuilder();
        sb.AppendLine($"Waiting for Online Recovery: {ToStatusText(e.IsWaitingForOnlineRecovery)}");
        sb.AppendLine($"Paper Currently Feeding: {ToStatusText(e.IsPaperFeedingByButton)}");
        sb.AppendLine($"Paper Feed Button Pushed: {ToStatusText(e.IsPaperFeedButtonPushed)}");
        sb.AppendLine($"Printer Online: {ToStatusText(e.IsPrinterOnline)}");
        sb.AppendLine($"Cash Drawer Open: {ToStatusText(e.IsDrawerHigh)}");
        sb.AppendLine($"Cover Open: {ToStatusText(e.IsCoverOpen)}");
        sb.AppendLine($"Paper Low: {ToStatusText(e.NearEndSensorPaperAdequate)}");
        sb.AppendLine($"Paper Out: {ToStatusText(e.EndSensorPaperPresent)}");
        sb.AppendLine($"In Error State: {ToStatusText(e.HasError)}");
        sb.AppendLine($"Recoverable Error Occurred: {ToStatusText(e.RecoverableErrorOccured)}");
        sb.AppendLine($"Unrecoverable Error Occurred: {ToStatusText(e.UnrecoverableErrorOccured)}");
        sb.AppendLine($"Autocutter Error Occurred: {ToStatusText(e.AutocutterErrorOccured)}");
        sb.AppendLine($"Recoverable Non-Autocutter Error Occurred: {ToStatusText(e.AutoRecoverableErrorOccured)}");

        LabelStatus.Text = sb.ToString();
    }

    protected bool UseArgs(Dictionary<string, string> args)
    {
        PrinterModes mode;
        if (args.TryGetValue("mode", out var value))
            mode = Enum.Parse<PrinterModes>(value, true);
        else
            return false;

        switch (mode)
        {
            case PrinterModes.Network:
            {
                if (!args.TryGetValue("host", out var host))
                    return false;

                if (!args.TryGetValue("port", out var port))
                    return false;

                if (!args.TryGetValue("printername", out var printerName))
                    return false;

                var printer = new EscPosPrinter(host, int.Parse(port), printerName);
                SetPrinter(printer);

                break;
            }
            case PrinterModes.Serial:
            {

                if (!args.TryGetValue("portname", out var portname))
                    return false;

                if (!args.TryGetValue("baudrate", out var baudrate))
                    return false;

                var printer = new EscPosPrinter(portname, int.Parse(baudrate));
                SetPrinter(printer);

                break;
            }
            default:
            return false;
        }

        return true;
    }


}
