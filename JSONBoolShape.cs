namespace CsJSONTools;
public class JSONBoolShape : IJSONShape {
    public void Verify(IJSONValue value) {
        if (!(value is JSONBool)) {
            throw new InvalidJSONException("Expected bool", value);
        }
    }
}
