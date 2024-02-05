using System;

public class JSONBoolShape : IJSONShape {
    public void Verify(IJSONValue value) {
        if (!(value is JSONBool || value is JSONNull)) {
            throw new InvalidJSONException("Expected bool", value);
        }
    }
}
