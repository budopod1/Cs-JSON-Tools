using System;
using System.Linq;
using System.Collections.Generic;

public class JSONNull : IJSONValue {
    public JSONSpan span { get; set; }
    
    public string ToJSON() {
        return "null";
    }
}
