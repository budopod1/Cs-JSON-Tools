using System;
using System.Linq;
using System.Collections.Generic;

public class JSONDouble : IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => new List<byte> {2};
    
    public double Value;

    public JSONDouble(double value) {
        Value = value;
    }

    public string Stringify() {
        return Value.ToString();
    }

    public string PrettyPrint(PrettyPrintConfig config) {
        return Stringify();
    }

    public IEnumerable<byte> ToBJSON(BJSONEnv env) {
        byte[] bytes = BitConverter.GetBytes(Value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes.ToList();
    }

    public static IJSONValue OrNull(double? num) {
        return num.HasValue ? new JSONDouble(num.Value) : (IJSONValue)new JSONNull();
    }
}
