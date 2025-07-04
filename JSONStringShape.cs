namespace CsJSONTools;
public class JSONStringShape : IJSONShape {
    public void Verify(IJSONValue value) {
        if (value is not JSONString) {
            throw new InvalidJSONException("Expected string", value);
        }
    }
}
