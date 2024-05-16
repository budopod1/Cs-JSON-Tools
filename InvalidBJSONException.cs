using System;

public class InvalidBJSONException : Exception {
    public InvalidBJSONException(string message) : base(message) {}
}
