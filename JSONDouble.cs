using System;
using System.Linq;
using System.Collections.Generic;

public class JSONDouble : IJSONValue {
    public JSONSpan span { get; set; }
    
    public double? Value;

    public JSONDouble(double? value) {
        Value = value;
    }

    public string ToJSON() {
        return Value.HasValue ? Value.ToString() : "null";
    }

    public bool IsNull() {
        return Value == null;
    }
}
