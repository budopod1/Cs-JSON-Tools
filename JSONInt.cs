using System;
using System.Linq;
using System.Collections.Generic;

public class JSONInt : IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => new List<byte> {3};
    
    public int Value;

    public JSONInt(int value) {
        Value = value;
    }

    public string Stringify() {
        return Value.ToString();
    }

    public string PrettyPrint(PrettyPrintConfig config) {
        return Stringify();
    }

    public IEnumerable<byte> ToBJSON(BJSONEnv env) {
        return BJSONEnv.ToVWInt(Value);
    }

    public static IJSONValue OrNull(int? num) {
        return num.HasValue ? new JSONInt(num.Value) : (IJSONValue)new JSONNull();
    }
}
