using System;

public class PrettyPrintConfig {
    public int IndentationAmount;
    public int MaxLineContentLen;

    public PrettyPrintConfig(int indentationAmount, int maxLineContentLen) {
        IndentationAmount = indentationAmount;
        MaxLineContentLen = maxLineContentLen;
    }

    public string Indent(string text) {
        string indent = new string(' ', IndentationAmount);
        return indent + text.Replace("\n", "\n"+indent);
    }
}
