using System;

public class LiteralDecodeException : Exception {
    public int Position;

    public LiteralDecodeException(string message, int position) : base(message) {
        Position = position;
    }
}
