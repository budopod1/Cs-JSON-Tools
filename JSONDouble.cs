public class JSONDouble(double value) : IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => [2];

    public double Value = value;

    public string Stringify() {
        return Value.ToString();
    }

    public string PrettyPrint(PrettyPrintConfig config) {
        return Stringify();
    }

    public IEnumerable<byte> ToBinJSON(BinJSONEnv env) {
        byte[] bytes = BitConverter.GetBytes(Value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes.ToList();
    }

    public static IJSONValue OrNull(double? num) {
        return num.HasValue ? new JSONDouble(num.Value) : new JSONNull();
    }
}
