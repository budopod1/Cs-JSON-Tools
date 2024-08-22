public class PrettyPrintConfig(int indentationAmount, int maxLineContentLen) {
    public int IndentationAmount = indentationAmount;
    public int MaxLineContentLen = maxLineContentLen;

    public string Indent(string text) {
        string indent = new(' ', IndentationAmount);
        return indent + text.Replace("\n", "\n"+indent);
    }
}
