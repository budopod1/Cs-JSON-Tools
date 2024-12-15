namespace CsJSONTools;
public class JSONObjectAnyKeyShape(IJSONShape sub) : IJSONObjectShape {
    readonly IJSONShape sub = sub;

    public void Verify(IJSONValue val) {
        if (val is not JSONObject obj) {
            throw new InvalidJSONException(
                "Expected object", val
            );
        }
        foreach (KeyValuePair<string, IJSONValue> pair in obj) {
            sub.Verify(pair.Value);
        }
    }

    public IJSONShape GetSub(string key) {
        return sub;
    }
}
