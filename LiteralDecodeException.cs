using System;

public class LiteralDecodeException(string message, int position) : Exception(message) {
    public int Position = position;
}
