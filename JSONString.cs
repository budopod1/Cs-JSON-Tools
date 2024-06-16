using System;
using System.Collections.Generic;

public class JSONString : IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => new List<byte> {7};
    
    public string Value;

    public JSONString(string value) {
        Value = value;
    }

    public string Stringify() {
        return JSONTools.ToLiteral(Value);
    }

    public string PrettyPrint(PrettyPrintConfig config) {
        return Stringify();
    }

    public IEnumerable<byte> ToBJSON(BJSONEnv env) {
        return env.RegisterString(Value);
    }

    public static IJSONValue OrNull(string str) {
        return str != null ? new JSONString(str) : (IJSONValue)new JSONNull();
    }
}
