namespace CsJSONTools;
public class JSONObjectAnyKeyShape(IJSONShape sub) : IJSONObjectShape {
    readonly IJSONShape sub = sub;

    public void Verify(IJSONValue val) {
        JSONObject obj = val as JSONObject;
        if (obj == null) {
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
