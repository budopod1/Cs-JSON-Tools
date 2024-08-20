using System;

public class JSONNullableShape(IJSONShape norm) : IJSONListShape, IJSONObjectShape {
    readonly IJSONShape norm = norm;

    public void Verify(IJSONValue value) {
        if (value is JSONNull) return;
        norm.Verify(value);
    }

    public IJSONShape GetSub() {
        return ((IJSONListShape)norm).GetSub();
    }

    public IJSONShape GetSub(string key) {
        return ((IJSONObjectShape)norm).GetSub(key);
    }
}
