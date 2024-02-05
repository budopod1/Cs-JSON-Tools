using System;

public class JSONListShape : IJSONListShape {
    IJSONShape sub;

    public JSONListShape(IJSONShape sub) {
        this.sub = sub;
    }
    
    public void Verify(IJSONValue value) {
        if (value is JSONList) {
            foreach (IJSONValue subValue in ((JSONList)value)) {
                sub.Verify(subValue);
            }
        } else if (!(value is JSONNull)) {
            throw new InvalidJSONException("Expected list", value);
        }
    }

    public IJSONShape GetSub() {
        return sub;
    }
}
