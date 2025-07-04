namespace CsJSONTools;
public class ShapedJSON {
    readonly IJSONValue val;
    readonly IJSONShape shape;

    public ShapedJSON(IJSONValue val, IJSONShape shape) {
        shape.Verify(val);
        this.val = val;
        this.shape = shape;
    }

    public ShapedJSON this[int key] {
        get => new(
            ((JSONList)val)[key], ((IJSONListShape)shape).GetSub()
        );
        set {
            ((IJSONListShape)shape).GetSub().Verify((IJSONValue)value);
            ((JSONList)val)[key] = (IJSONValue)value;
        }
    }

    public ShapedJSON this[string key] {
        get => new(
            ((JSONObject)val)[key], ((IJSONObjectShape)shape).GetSub(key)
        );
        set {
            ((IJSONObjectShape)shape).GetSub(key).Verify((IJSONValue)value);
            ((JSONObject)val)[key] = (IJSONValue)value;
        }
    }

    public string GetString() {
        return ((JSONString)val).Value;
    }

    public string GetStringOrNull() {
        if (val is JSONNull) return null;
        return ((JSONString)val).Value;
    }

    public double GetDouble() {
        if (val is JSONInt) return ((JSONInt)val).Value;
        return ((JSONDouble)val).Value;
    }

    public double? GetDoubleOrNull() {
        if (val is JSONNull) return null;
        return ((JSONDouble)val).Value;
    }

    public int GetInt() {
        return ((JSONInt)val).Value;
    }

    public int? GetIntOrNull() {
        if (val is JSONNull) return null;
        return ((JSONInt)val).Value;
    }

    public JSONList GetList() {
        return (JSONList)val;
    }

    public JSONObject GetObject() {
        return (JSONObject)val;
    }

    public bool GetBool() {
        return ((JSONBool)val).Value;
    }

    public bool? GetBoolOrNull() {
        if (val is JSONNull) return null;
        return ((JSONBool)val).Value;
    }

    public bool IsNull() {
        return val is JSONNull;
    }

    public ShapedJSON ToShape(IJSONShape newShape) {
        return new ShapedJSON(val, newShape);
    }

    public IEnumerable<ShapedJSON> IterList() {
        return GetList().Select(sub => new ShapedJSON(
            sub, ((IJSONListShape)shape).GetSub()
        ));
    }

    public IEnumerable<KeyValuePair<string, ShapedJSON>> IterObject() {
        return GetObject().Select(pair => {
            IJSONShape subshape = ((IJSONObjectShape)shape).GetSub(pair.Key);
            return new KeyValuePair<string, ShapedJSON>(
                pair.Key, subshape == null ? null : new ShapedJSON(pair.Value, subshape)
            );
        }).Where(pair => pair.Value != null);
    }

    public bool HasKey(string key) {
        return GetObject().ContainsKey(key);
    }

    public int ListCount() {
        return GetList().Count;
    }

    public int ObjectCount() {
        return GetObject().Count;
    }

    public IJSONValue GetJSON() {
        return val;
    }

    public IJSONShape GetShape() {
        return shape;
    }
}
