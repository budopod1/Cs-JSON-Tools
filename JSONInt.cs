public class JSONInt(int value) : IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => [3];

    public int Value = value;

    public string Stringify() {
        return Value.ToString();
    }

    public string PrettyPrint(PrettyPrintConfig config) {
        return Stringify();
    }

    public IEnumerable<byte> ToBinJSON(BinJSONEnv env) {
        return BinJSONEnv.ToVWInt((uint)(Value < 0 ? -Value << 1 | 1 : Value << 1));
    }

    public static IJSONValue OrNull(int? num) {
        return num.HasValue ? new JSONInt(num.Value) : new JSONNull();
    }
}
