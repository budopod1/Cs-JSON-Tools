using System;
using System.Collections.Generic;

public interface IJSONValue {
    IEnumerable<byte> ID { get; }
    JSONSpan span { get; set; }
    
    string ToJSON();
    IEnumerable<byte> ToBJSON(BJSONEnv env);
}
