using System;
using System.Collections.Generic;

public interface IJSONValue {
    IEnumerable<byte> ID { get; }
    JSONSpan span { get; set; }

    string Stringify();
    string PrettyPrint(PrettyPrintConfig config);
    IEnumerable<byte> ToBJSON(BJSONEnv env);
}
