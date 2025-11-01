using System.Numerics;
using System.Globalization;

namespace ThermalPrinter;

public static class CalcUtil
{
    public static string DecimalSeparator { get; set; } = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;

    public static string NegateStr(string str) => str.StartsWith('-') ? str.Substring(1) : '-' + str;

    public static string FloatToString(IFormattable val, int decimalPlaces)
    {
        string format = "0";
        if (decimalPlaces > 0)
            format += DecimalSeparator + new string('#', decimalPlaces);
        return val.ToString(format, CultureInfo.InvariantCulture);
    }

    public static object? EvaluateExpression(string expression)
    {
        expression = expression.Trim().Replace(",", DecimalSeparator);
        if (expression.Length == 0)
            return null;
        object? eval;
        try
        {
            var exp = new NCalc.Expression(expression, NCalc.ExpressionOptions.None, CultureInfo.InvariantCulture);
            eval = exp.Evaluate();
        }
        catch (Exception)
        {
            return null;
        }
        return eval;
    }

    public static bool TryEvaluateExpression<T>(string expression, out double value)
    {
        var eval = EvaluateExpression(expression);
        if (eval is int i)
        {
            value = i;
            return true;
        }
        if (eval is double d)
        {
            value = d;
            return true;
        }
        value = 0d;
        return false;
    }
    public static string? EvaluateExpressionStr(string expression, int decimalPlaces = 0)
    {
        var eval = EvaluateExpression(expression);
        if (eval is IFormattable formattable)
            FloatToString(formattable, decimalPlaces);
        return null;
    }
}
