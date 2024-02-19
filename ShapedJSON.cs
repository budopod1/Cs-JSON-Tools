using System;
using System.Linq;
using System.Collections.Generic;

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
            ((JSONList)val)[key], ((IJSONListShape)shape).GetSub()
        );
        set {
            ((IJSONListShape)shape).GetSub().Verify((IJSONValue)value);
            ((JSONList)val)[key] = ((IJSONValue)value);
        }
    }

    public ShapedJSON this[string key] {
        get => new ShapedJSON(
            ((JSONObject)val)[key], ((IJSONObjectShape)shape).GetSub(key)
        );
        set {
            ((IJSONObjectShape)shape).GetSub(key).Verify((IJSONValue)value);
            ((JSONObject)val)[key] = ((IJSONValue)value);
        }
    }

    public string GetString() {
        return ((JSONString)val).Value;
    }

    public double? GetDouble() {
        if (val is JSONInt) return ((JSONInt)val).Value;
        return ((JSONDouble)val).Value;
    }

    public int? GetInt() {
        return ((JSONInt)val).Value;
    }

    public JSONList GetList() {
        return (JSONList)val;
    }

    public JSONObject GetObject() {
        return (JSONObject)val;
    }

    public bool? GetBool() {
        return ((JSONBool)val).Value;
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
        return GetObject().Select(pair => new KeyValuePair<string, ShapedJSON>(
            pair.Key, new ShapedJSON(
                pair.Value, ((IJSONObjectShape)shape).GetSub(pair.Key)
            )
        ));
    }

    public int ListCount() {
        return ((JSONList)val).Count;
    }

    public int ObjectCount() {
        return ((JSONObject)val).Count;
    }

    public IJSONValue GetJSON() {
        return val;
    }

    public IJSONShape GetShape() {
        return shape;
    }
}
