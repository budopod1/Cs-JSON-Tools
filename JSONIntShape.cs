public class JSONIntShape : IJSONShape {
    public void Verify(IJSONValue value) {
        if (!(value is JSONInt)) {
            throw new InvalidJSONException("Expected int", value);
        }
    }
}
