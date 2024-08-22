public class InvalidJSONException : Exception {
    public JSONSpan span = null;

    public InvalidJSONException(string message, IJSONValue value) : base(message) {
        span = value.span;
    }

    public InvalidJSONException(string message, JSONSpan span) : base(message) {
        this.span = span;
    }

    public InvalidJSONException(string message) : base(message) {}
}
