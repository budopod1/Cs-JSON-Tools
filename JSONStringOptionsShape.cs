using System;
using System.Linq;
using System.Collections.Generic;

public class JSONStringOptionsShape(List<string> options) : IJSONShape {
    readonly List<string> options = options;

    public void Verify(IJSONValue value) {
        if (!(value is JSONString)) {
            throw new InvalidJSONException("Expected string", value);
        }
        string text = ((JSONString)value).Value;
        if (!options.Contains(text)) {
            string expected = JSONTools.ENList(options.Select(option=>JSONTools.ToLiteral(option)), "or");
            throw new InvalidJSONException($"Expected {expected}, found {JSONTools.ToLiteral(text)}", value);
        }
    }
}
