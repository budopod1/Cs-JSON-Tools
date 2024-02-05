using System;

public class JSONIntShape : IJSONShape {
    public void Verify(IJSONValue value) {
        if (!(value is JSONInt || value is JSONNull)) {
            throw new InvalidJSONException("Expected int", value);
        }
    }
}
