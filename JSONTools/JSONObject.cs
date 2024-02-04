using System;
using System.Linq;
using System.Collections.Generic;

public class JSONObject : Dictionary<string, IJSONValue>, IJSONValue {
    public JSONSpan span { get; set; }
    
    public string ToJSON() {
        return "{"+String.Join(", ", this.Select(pair=>(
            JSONTools.ToLiteral(pair.Key) + ": " + pair.Value.ToJSON()
        )))+"}";
    }
}
