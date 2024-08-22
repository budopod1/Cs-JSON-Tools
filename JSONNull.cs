public class JSONNull : IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => [5];

    public string Stringify() {
        return "null";
    }

    public string PrettyPrint(PrettyPrintConfig config) {
        return Stringify();
    }

    public IEnumerable<byte> ToBinJSON(BinJSONEnv env) {
        return [];
    }
}
