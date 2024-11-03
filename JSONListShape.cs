namespace CsJSONTools;
public class JSONListShape(IJSONShape sub) : IJSONListShape {
    readonly IJSONShape sub = sub;

    public void Verify(IJSONValue value) {
        if (value is JSONList) {
            foreach (IJSONValue subValue in (JSONList)value) {
                sub.Verify(subValue);
            }
        } else {
            throw new InvalidJSONException("Expected list", value);
        }
    }

    public IJSONShape GetSub() {
        return sub;
    }
}
