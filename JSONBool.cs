using System;
using System.Collections.Generic;

public class JSONBool : IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => new List<byte> {1};
    
    public bool Value;

    public JSONBool(bool value) {
        Value = value;
    }

    public string Stringify() {
        return Value ? "true" : "false";
    }

    public string PrettyPrint(PrettyPrintConfig config) {
        return Stringify();
    }

    public IEnumerable<byte> ToBJSON(BJSONEnv env) {
        return new List<byte> {Value ? (byte)0x01 : (byte)0x00};
    }

    public static IJSONValue OrNull(bool? val) {
        return val.HasValue ? new JSONBool(val.Value) : (IJSONValue)new JSONNull();
    }
}
