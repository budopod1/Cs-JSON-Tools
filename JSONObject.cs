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
