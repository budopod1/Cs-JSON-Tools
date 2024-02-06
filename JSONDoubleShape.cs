using System;

public class JSONDoubleShape : IJSONShape {
    public void Verify(IJSONValue value) {
        if (!(value is JSONInt || value is JSONDouble)) {
            throw new InvalidJSONException("Expected number", value);
        }
    }
}
