using System;

public interface IJSONValue {
    JSONSpan span { get; set; }
    
    string ToJSON();
}
