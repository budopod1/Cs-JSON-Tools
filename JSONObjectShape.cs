using System;
using System.Collections.Generic;

public class JSONObjectShape : IJSONObjectShape {
    Dictionary<string, IJSONShape> shape;

    public JSONObjectShape(Dictionary<string, IJSONShape> shape) {
        this.shape = shape;
    }

    public void Verify(IJSONValue val) {
        JSONObject obj = val as JSONObject;
        if (obj == null) {
            throw new InvalidJSONException(
                "Expected object", val
            );
        }
        foreach (KeyValuePair<string, IJSONShape> pair in shape) {
            if (!obj.ContainsKey(pair.Key)) {
                throw new InvalidJSONException(
                    $"Required key '{pair.Key}' not found", val
                );
            }
            pair.Value.Verify(obj[pair.Key]);
        }
    }

    public IJSONShape GetSub(string key) {
        return shape[key];
    }
}
