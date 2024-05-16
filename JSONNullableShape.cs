using System;

public class JSONNullableShape : IJSONListShape, IJSONObjectShape {
    IJSONShape norm;

    public JSONNullableShape(IJSONShape norm) {
        this.norm = norm;
    }

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
