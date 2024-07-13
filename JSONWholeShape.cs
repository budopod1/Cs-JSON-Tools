using System;

public class JSONWholeShape : IJSONShape {
    public void Verify(IJSONValue value) {
        JSONInt intHolder = value as JSONInt;
        if (intHolder == null) {
            throw new InvalidJSONException("Expected an integer", value);
        }
        if (intHolder.Value < 0) {
            throw new InvalidJSONException("Expected value greater than or equal to 0", value);
        }
    }
}
