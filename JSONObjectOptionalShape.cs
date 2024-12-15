namespace CsJSONTools;
public class JSONObjectOptionalShape(Dictionary<string, IJSONShape> shape) : IJSONObjectShape {
    readonly Dictionary<string, IJSONShape> shape = shape;

    public void Verify(IJSONValue val) {
        if (val is not JSONObject obj) {
            throw new InvalidJSONException(
                "Expected object", val
            );
        }
        foreach (KeyValuePair<string, IJSONValue> pair in obj) {
            if (shape.TryGetValue(pair.Key, out IJSONShape subshape)) {
                subshape.Verify(pair.Value);
            } else {
                throw new InvalidJSONException(
                    $"Unexpected key '{pair.Key}'", obj
                );
            }
        }
    }

    public IJSONShape GetSub(string key) {
        return shape.GetValueOrDefault(key);
    }
}
