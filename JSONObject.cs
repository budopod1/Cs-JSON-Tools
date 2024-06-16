using System;
using System.Linq;
using System.Collections.Generic;

public class JSONObject : Dictionary<string, IJSONValue>, IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => new List<byte> {6};
    
    public string Stringify() {
        return "{"+String.Join(", ", this.Select(pair => (
            JSONTools.ToLiteral(pair.Key) + ": " + pair.Value.Stringify()
        )))+"}";
    }

    public string PrettyPrint(PrettyPrintConfig config) {
        IEnumerable<string> stringifiedParts = this.Select(pair => (
            JSONTools.ToLiteral(pair.Key) + ": " + pair.Value.PrettyPrint(config)
        ));
        if (stringifiedParts.All(part => part.IndexOf('\n') == -1)) {
            string basicStringification = "{"+String.Join(", ", stringifiedParts)+"}";
            if (basicStringification.Length < config.MaxLineContentLen) {
                return basicStringification;
            }
        }
        return "{\n"+config.Indent(String.Join(",\n", stringifiedParts))+"\n}";
    }

    public IEnumerable<byte> ToBJSON(BJSONEnv env) {
        IEnumerable<byte> result = BJSONEnv.ToVWInt(Count);
        foreach (KeyValuePair<string, IJSONValue> pair in this) {
            IEnumerable<byte> key = env.RegisterString(pair.Key);
            IEnumerable<byte> value = pair.Value.ToBJSON(env);
            result = result.Concat(key.Concat(pair.Value.ID).Concat(value));
        }
        return result;
    }
}
