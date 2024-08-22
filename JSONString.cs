public class JSONString(string value) : IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => [7];

    public string Value = value;

    public string Stringify() {
        return JSONTools.ToLiteral(Value);
    }

    public string PrettyPrint(PrettyPrintConfig config) {
        return Stringify();
    }

    public IEnumerable<byte> ToBinJSON(BinJSONEnv env) {
        return env.RegisterString(Value);
    }

    public static IJSONValue OrNull(string str) {
        return str != null ? new JSONString(str) : new JSONNull();
    }
}
