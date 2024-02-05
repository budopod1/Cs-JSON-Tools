using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

public class JSONList : Collection<IJSONValue>, IJSONValue {
    public JSONSpan span { get; set; }
    
    public JSONList(IEnumerable<IJSONValue> values) {
        foreach (IJSONValue value in values) Add(value);
    }

    public JSONList() {}

    public string ToJSON() {
        return $"[{String.Join(", ", this.Select(item => item.ToJSON()))}]";
    }

    public bool IsNull() {
        return false;
    }
}
