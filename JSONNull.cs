using System;
using System.Linq;
using System.Collections.Generic;

public class JSONNull : IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => new List<byte> {5};
    
    public string Stringify() {
        return "null";
    }

    public string PrettyPrint(PrettyPrintConfig config) {
        return Stringify();
    }

    public IEnumerable<byte> ToBJSON(BJSONEnv env) {
        return new List<byte>();
    }
}
