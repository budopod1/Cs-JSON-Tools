using System;
using System.Linq;

public class JSONString : IJSONValue {
    public JSONSpan span { get; set; }
    
    public string Value;

    public JSONString(string value) {
        Value = value;
    }

    public string ToJSON() {
        return Value == null ? "null" : JSONTools.ToLiteral(Value);
    }
}
