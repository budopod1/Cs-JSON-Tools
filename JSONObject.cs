namespace CsJSONTools;
public class JSONObject : Dictionary<string, IJSONValue>, IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => [6];

    public string Stringify() {
        return "{"+ string.Join(", ", this.Select(pair =>
            JSONTools.ToLiteral(pair.Key) + ": " + pair.Value.Stringify()
        ))+"}";
    }

    public string PrettyPrint(PrettyPrintConfig config) {
        IEnumerable<string> stringifiedParts = this.Select(pair =>
            JSONTools.ToLiteral(pair.Key) + ": " + pair.Value.PrettyPrint(config)
        );
        if (stringifiedParts.All(part => part.IndexOf('\n') == -1)) {
            string basicStringification = "{"+ string.Join(", ", stringifiedParts)+"}";
            if (basicStringification.Length < config.MaxLineContentLen) {
                return basicStringification;
            }
        }
        return "{\n"+config.Indent(string.Join(",\n", stringifiedParts))+"\n}";
    }

    public IEnumerable<byte> ToBinJSON(BinJSONEnv env) {
        IEnumerable<byte> result = BinJSONEnv.ToVWInt((uint)Count);
        foreach (KeyValuePair<string, IJSONValue> pair in this) {
            IEnumerable<byte> key = env.RegisterString(pair.Key);
            IEnumerable<byte> value = pair.Value.ToBinJSON(env);
            result = result.Concat(key.Concat(pair.Value.ID).Concat(value));
        }
        return result;
    }
}
