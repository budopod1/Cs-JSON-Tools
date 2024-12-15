namespace CsJSONTools;
public class JSONStringOptionsShape(IEnumerable<string> options) : IJSONShape {
    readonly IEnumerable<string> options = options;

    public void Verify(IJSONValue value) {
        if (value is not JSONString jsonString) {
            throw new InvalidJSONException("Expected string", value);
        }
        string text = jsonString.Value;
        if (!options.Contains(text)) {
            string expected = JSONTools.ENList(options.Select(JSONTools.ToLiteral), "or");
            throw new InvalidJSONException($"Expected {expected}, found {JSONTools.ToLiteral(text)}", value);
        }
    }
}
