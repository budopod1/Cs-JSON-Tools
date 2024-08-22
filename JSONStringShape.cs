public class JSONStringShape : IJSONShape {
    public void Verify(IJSONValue value) {
        if (!(value is JSONString)) {
            throw new InvalidJSONException("Expected string", value);
        }
    }
}
