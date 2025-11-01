using EscPosNet;
using EscPosNet.Emitters;

namespace ThermalPrinter;

public class EscPosImmediatePrinter
{
    // Ethernet or WiFi (This uses an Immediate Printer, no live paper status events, but is easier to use)
    public readonly ImmediateNetworkPrinter Printer;
    public readonly CommandEmitter Emitter = new();

    public EscPosImmediatePrinter(string hostnameOrIp = "192.168.1.50", int port = 9100) : this($"{hostnameOrIp}:{port}") { }
    public EscPosImmediatePrinter(string address)
    {
        Printer = new ImmediateNetworkPrinter() { ConnectionString = address };
    }

    public async Task TestAsync()
    {
        var e = Emitter;

        e.Start();

        e.Align(PrintAlign.Center);
        e.PrintLine("");
        e.SetBarcodeHeightInDots(360);
        e.SetBarWidth(BarWidth.Default);
        e.SetBarLabelPosition(BarLabelPrintPosition.None);
        e.PrintBarcode(BarcodeType.ITF, "0123456789");
        e.PrintLine("");
        e.PrintLine("B&H PHOTO & VIDEO");
        e.PrintLine("420 NINTH AVE.");
        e.PrintLine("NEW YORK, NY 10001");
        e.PrintLine("(212) 502-6380 - (800)947-9975");
        e.Styles(PrintStyle.Underline);
        e.PrintLine("www.bhphotovideo.com");
        e.Styles(PrintStyle.None);
        e.PrintLine("");
        e.Align(PrintAlign.Left);
        e.PrintLine("Order: 123456789        Date: 02/01/19");
        e.PrintLine("");
        e.PrintLine("");
        e.Styles(PrintStyle.FontB);
        e.PrintLine("1   TRITON LOW-NOISE IN-LINE MICROPHONE PREAMP");
        e.PrintLine("    TRFETHEAD/FETHEAD                        89.95         89.95");
        e.PrintLine("----------------------------------------------------------------");
        e.Align(PrintAlign.Right);
        e.PrintLine("SUBTOTAL         89.95");
        e.PrintLine("Total Order:         89.95");
        e.PrintLine("Total Payment:         89.95");
        e.PrintLine("");
        e.Align(PrintAlign.Left);
        e.Styles(PrintStyle.Bold | PrintStyle.FontB);
        e.PrintLine("SOLD TO:                        SHIP TO:");
        e.Styles(PrintStyle.FontB);
        e.PrintLine("  FIRSTN LASTNAME                 FIRSTN LASTNAME");
        e.PrintLine("  123 FAKE ST.                    123 FAKE ST.");
        e.PrintLine("  DECATUR, IL 12345               DECATUR, IL 12345");
        e.PrintLine("  (123)456-7890                   (123)456-7890");
        e.PrintLine("  CUST: 87654321");
        e.PrintLine("");
        e.PrintLine("");

        await Printer.WriteAsync(e.EndMemory());
    }

}
