using System;
using System.Collections.Generic;

public class JSONObjectAnyKeyShape : IJSONObjectShape {
    IJSONShape sub;

    public JSONObjectAnyKeyShape(IJSONShape sub) {
        this.sub = sub;
    }

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
