namespace CsJSONTools;
public class JSONIntShape : IJSONShape {
    public void Verify(IJSONValue value) {
        if (value is not JSONInt) {
            throw new InvalidJSONException("Expected int", value);
        }
    }
}
