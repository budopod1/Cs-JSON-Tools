using System;

public class JSONWholeShape : IJSONShape {
    public void Verify(IJSONValue value) {
        JSONInt intHolder = value as JSONInt;
        if (intHolder == null) {
            throw new InvalidJSONException("Expected int", value);
        }
        if (intHolder.IsNull()) return;
        if (intHolder.Value < 0) {
            throw new InvalidJSONException("Expected value greater than 0", value);
        }
    }
}
