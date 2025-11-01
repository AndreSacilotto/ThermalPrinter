namespace ThermalPrinter;


public static class AsciiControl
{
    /// <summary> ASCII Control Characters [0-31, 127]</summary>
    public enum Char : byte
    {
        /// <summary>Null</summary>
        NUL = 0,
        /// <summary>Start of Heading</summary>
        SOH = 1,
        /// <summary>Start of Text</summary>
        STX = 2,
        /// <summary>End of Text</summary>
        ETX = 3,
        /// <summary>End of Transmission</summary>
        EOT = 4,
        /// <summary>Enquiry</summary>
        ENQ = 5,
        /// <summary>Acknowledge</summary>
        ACK = 6,
        /// <summary>Bell</summary>
        BEL = 7,
        /// <summary>Backspace</summary>
        BS = 8,
        /// <summary>Horizontal Tab '\t'</summary>
        HT = 9,
        /// <summary>Line Feed '\n'</summary>
        LF = 10,
        /// <summary>Vertical Tab</summary>
        VT = 11,
        /// <summary>Form Feed</summary>
        FF = 12,
        /// <summary>Carriage Return '\r'</summary>
        CR = 13,
        /// <summary>Shift Out</summary>
        SO = 14,
        /// <summary>Shift In</summary>
        SI = 15,
        /// <summary>Data Link Escape</summary>
        DLE = 16,
        /// <summary>Device Control 1</summary>
        DC1 = 17,
        /// <summary>Device Control 2</summary>
        DC2 = 18,
        /// <summary>Device Control 3</summary>
        DC3 = 19,
        /// <summary>Device Control 4</summary>
        DC4 = 20,
        /// <summary>Negative Acknowledge</summary>
        NAK = 21,
        /// <summary>Synchronous Idle</summary>
        SYN = 22,
        /// <summary>End of Transmission Block</summary>
        ETB = 23,
        /// <summary>Cancel</summary>
        CAN = 24,
        /// <summary>End of Medium</summary>
        EM = 25,
        /// <summary>Substitute</summary>
        SUB = 26,
        /// <summary>Escape</summary>
        ESC = 27,
        /// <summary>File Separator</summary>
        FS = 28,
        /// <summary>Group Separator</summary>
        GS = 29,
        /// <summary>Record Separator</summary>
        RS = 30,
        /// <summary>Unit Separator</summary>
        US = 31,
        /// <summary>Delete</summary>
        DEL = 127
    }
   
    public static bool IsAsciiControl(char val) => IsAsciiControl(val);
    public static bool IsAsciiControl(byte val) => val == (byte)Char.DEL || val <= (byte)Char.US;
    public static bool TryGetAsciiControl(char val, out Char ascii) => TryGetAsciiControl(val, out ascii);
    public static bool TryGetAsciiControl(byte val, out Char ascii)
    {
        if (IsAsciiControl(val))
        {
            ascii = (Char)val;
            return true;
        }
        ascii = (Char)0xFF;
        return false;
    }

    public static char ToChar(this Char ch) => (char)ch;
}