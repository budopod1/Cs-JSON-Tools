using System;
using System.Collections.Generic;

public interface IJSONValue {
    IEnumerable<byte> ID { get; }
    JSONSpan span { get; set; }
    
    string Stringify();
    IEnumerable<byte> ToBJSON(BJSONEnv env);
}
