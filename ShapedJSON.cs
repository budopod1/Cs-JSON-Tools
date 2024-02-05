using System;

public class ShapedJSON {
    IJSONValue val;
    IJSONShape shape;

    public ShapedJSON(IJSONValue val, IJSONShape shape) {
        shape.Verify(val);
        this.val = val;
        this.shape = shape;
    }

    public ShapedJSON this[int key] {
        get => new ShapedJSON(
            ((val is JSONNull) ? val : ((JSONList)val)[key]), 
            ((IJSONListShape)shape).GetSub()
        );
        set {
            ((IJSONListShape)shape).GetSub().Verify((IJSONValue)value);
            ((JSONList)val)[key] = ((IJSONValue)value);
        }
    }

    public ShapedJSON this[string key] {
        get => new ShapedJSON(
            ((val is JSONNull) ? val : ((JSONObject)val)[key]), 
            ((IJSONObjectShape)shape).GetSub(key)
        );
        set {
            ((IJSONObjectShape)shape).GetSub(key).Verify((IJSONValue)value);
            ((JSONObject)val)[key] = ((IJSONValue)value);
        }
    }

    public void NotNull() {
        if (val.IsNull()) {
            throw new InvalidJSONException(
                "Expected non-null value, found null", val
            );
        }
    }

    public string GetString() {
        if (val is JSONNull) return null;
        return ((JSONString)val).Value;
    }

    public double? GetDouble() {
        if (val is JSONNull) return null;
        if (val is JSONInt) return ((JSONInt)val).Value;
        return ((JSONDouble)val).Value;
    }

    public int? GetInt() {
        if (val is JSONNull) return null;
        return ((JSONInt)val).Value;
    }

    public JSONList GetList() {
        if (val is JSONNull) return null;
        return (JSONList)val;
    }

    public JSONObject GetObject() {
        if (val is JSONNull) return null;
        return (JSONObject)val;
    }

    public ShapedJSON ToShape(IJSONShape newShape) {
        return new ShapedJSON(val, newShape);
    }
}
