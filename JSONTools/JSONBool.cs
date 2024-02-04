using System;

public class JSONBool : IJSONValue {
    public JSONSpan span { get; set; }
    
    public bool? Value;

    public JSONBool(bool? value) {
        Value = value;
    }

    public string ToJSON() {
        return Value.HasValue ? (Value.Value?"true":"false") : "null";
    }
}
