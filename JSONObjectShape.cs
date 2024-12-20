namespace CsJSONTools;
public class JSONObjectShape(Dictionary<string, IJSONShape> shape) : IJSONObjectShape {
    readonly Dictionary<string, IJSONShape> shape = shape;

    public void Verify(IJSONValue val) {
        if (val is not JSONObject obj) {
            throw new InvalidJSONException(
                "Expected object", val
            );
        }
        foreach (KeyValuePair<string, IJSONShape> pair in shape) {
            if (!obj.TryGetValue(pair.Key, out IJSONValue sub)) {
                throw new InvalidJSONException(
                    $"Required key '{pair.Key}' not found", val
                );
            }
            pair.Value.Verify(sub);
        }
    }

    public IJSONShape GetSub(string key) {
        return shape.GetValueOrDefault(key);
    }
}
