using System;

public class JSONStringShape : IJSONShape {
    public void Verify(IJSONValue value) {
        if (!(value is JSONString || value is JSONNull)) {
            throw new InvalidJSONException("Expected string", value);
        }
    }
}
