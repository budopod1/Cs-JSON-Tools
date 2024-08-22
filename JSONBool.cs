public class JSONBool(bool value) : IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => [1];

    public bool Value = value;

    public string Stringify() {
        return Value ? "true" : "false";
    }

    public string PrettyPrint(PrettyPrintConfig config) {
        return Stringify();
    }

    public IEnumerable<byte> ToBinJSON(BinJSONEnv env) {
        return [Value ? (byte)0x01 : (byte)0x00];
    }

    public static IJSONValue OrNull(bool? val) {
        return val.HasValue ? new JSONBool(val.Value) : new JSONNull();
    }
}
