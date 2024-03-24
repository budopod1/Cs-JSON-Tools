using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

public static class JSONTools {
    public static string ENList(IEnumerable<string> enumerable, string joiner="and") {
        List<string> list = enumerable.ToList();
        if (list.Count == 0) return "none";
        if (list.Count == 1) return list[0];
        if (list.Count == 2) return $"{list[0]} {joiner} {list[1]}";
        string beginning = "";
        for (int i = 0; i < list.Count-1; i++) {
            beginning += list[i] + ", ";
        }
        return $"{beginning}{joiner} {list[list.Count-1]}";
    }

    public static string ToLiteral(string input) {
        // https://stackoverflow.com/a/14087738
        StringBuilder literal = new StringBuilder(input.Length + 2);
        literal.Append("\"");
        foreach (var c in input) {
            switch (c) {
                case '\"': literal.Append("\\\""); break;
                case '\\': literal.Append(@"\\"); break;
                case '\0': literal.Append(@"\0"); break;
                case '\a': literal.Append(@"\a"); break;
                case '\b': literal.Append(@"\b"); break;
                case '\f': literal.Append(@"\f"); break;
                case '\n': literal.Append(@"\n"); break;
                case '\r': literal.Append(@"\r"); break;
                case '\t': literal.Append(@"\t"); break;
                case '\v': literal.Append(@"\v"); break;
                default:
                    // ASCII printable character
                    if (c >= 0x20 && c <= 0x7e) {
                        literal.Append(c);
                    // As UTF16 escaped character
                    } else {
                        literal.Append(@"\u");
                        literal.Append(((int)c).ToString("x4"));
                    }
                    break;
            }
        }
        literal.Append("\"");
        return literal.ToString();
    }

    public static string ToLiteralChar(char c) {
        StringBuilder literal = new StringBuilder(3);
        literal.Append("\"");
        switch (c) {
            case '\'': literal.Append("\\'"); break;
            case '\\': literal.Append(@"\\"); break;
            case '\0': literal.Append(@"\0"); break;
            case '\a': literal.Append(@"\a"); break;
            case '\b': literal.Append(@"\b"); break;
            case '\f': literal.Append(@"\f"); break;
            case '\n': literal.Append(@"\n"); break;
            case '\r': literal.Append(@"\r"); break;
            case '\t': literal.Append(@"\t"); break;
            case '\v': literal.Append(@"\v"); break;
            default:
                // ASCII printable character
                if (c >= 0x20 && c <= 0x7e) {
                    literal.Append(c);
                // As UTF16 escaped character
                } else {
                    literal.Append(@"\u");
                    literal.Append(((int)c).ToString("x4"));
                }
                break;
        }
        literal.Append("\"");
        return literal.ToString();
    }

    public static string FromLiteral(string input, bool hasQuotes=true) {
        // TODO: change function to not throw InvalidJSONException
        if (hasQuotes) input = input.Substring(1, input.Length-2);
        StringBuilder result = new StringBuilder(input.Length);
        bool wasBackslash = false;
        for (int i = 0; i < input.Length; i++) {
            char chr = input[i];
            if (wasBackslash) {
                switch (chr) {
                    case '"': result.Append('"'); break;
                    case '\\': result.Append('\\'); break;
                    case '0': result.Append('\0'); break;
                    case 'a': result.Append('\a'); break;
                    case 'b': result.Append('\b'); break;
                    case 'f': result.Append('\f'); break;
                    case 'n': result.Append('\n'); break;
                    case 'r': result.Append('\r'); break;
                    case 't': result.Append('\t'); break;
                    case 'v': result.Append('\v'); break;
                    case 'u':
                        if (i + 4 >= input.Length) {
                            throw new LiteralDecodeException(
                                "Expected unicode code after '\\u', found EOF", i
                            );
                        }
                        string hex = input.Substring(i+1, 4);
                        int code = 0;
                        int scale = 1;
                        for (int j = 3; j >= 0; j--) {
                            char nibble = Char.ToLower(hex[j]);
                            if ('0' <= nibble && nibble <= '9') {
                                code += scale * (nibble - '0');
                            } else if ('a' <= nibble && nibble <= 'f') {
                                code += scale * (nibble - 'a' + 10);
                            } else {
                                throw new LiteralDecodeException(
                                    $"Expected hex digit, found {ToLiteralChar(nibble)}", 
                                    i+j+1
                                );
                            }
                            scale *= 16;
                        }
                        result.Append((char)code);
                        i += 4;
                        break;
                    default:
                        throw new LiteralDecodeException(
                            $"Invalid escape code, '\\{ToLiteralChar(chr)}'", i
                        );
                }
                wasBackslash = false;
            } else {
                if (chr == '\\') {
                    wasBackslash = true;
                } else {
                    result.Append(chr);
                }
            }
        }
        return result.ToString();
    }
    
    public static char FromLiteralChar(string input, bool hasQuotes=true) {
        int offset = hasQuotes ? 1 : 0;
        if (input[offset] == '\\') {
            switch (input[offset+1]) {
                case '"': return '"';
                case '\\': return '\\';
                case '0': return '\0';
                case 'a': return '\a';
                case 'b': return '\b';
                case 'f': return '\f';
                case 'n': return '\n';
                case 'r': return '\r';
                case 't': return '\t';
                case 'v': return '\v';
                default: return input[offset+1];
            }
        } else {
            return input[offset];
        }
    }

    public static char[] Whitespace = {' ', '\n', '\t'};

    static void Expect(string text, string next, int i_, out int i) {
        i = i_;
        while (true) {
            if (i >= text.Length) {
                throw new InvalidJSONException(
                    $"Expected {ToLiteral(next)}, found EOF", new JSONSpan(i)
                );
            }
            if (text.Substring(i).StartsWith(next)) {
                i += next.Length;
                return;
            }
            if (!Whitespace.Contains(text[i])) {
                throw new InvalidJSONException(
                    $"Expected {ToLiteral(next)}, found {ToLiteralChar(text[i])}'", new JSONSpan(i)
                );
            }
            i++;
        }
    }

    static string ExpectAny(string text, string[] nexts, int i_, out int i) {
        i = i_;
        string expected = ENList(nexts.Select(next=>ToLiteral(next)), "or");
        while (true) {
            if (i >= text.Length) {
                throw new InvalidJSONException(
                    $"Expected {expected}, found EOF", new JSONSpan(i)
                );
            }
            foreach (string next in nexts) {
                if (text.Substring(i).StartsWith(next)) {
                    i += next.Length;
                    return next;
                }
            }
            if (!Whitespace.Contains(text[i])) {
                throw new InvalidJSONException(
                    $"Expected {expected}, found {ToLiteralChar(text[i])}", new JSONSpan(i)
                );
            }
            i++;
        }
    }

    static char? NextChar(string text, int i_, out int i) {
        for (i = i_; i < text.Length; i++) {
            if (!Whitespace.Contains(text[i])) {
                return text[i++];
            }
        }
        return null;
    }

    static char? Peek(string text, int i) {
        for (; i < text.Length; i++) {
            if (!Whitespace.Contains(text[i])) {
                return text[i];
            }
        }
        return null;
    }

    static string ExpectString(string text, int i_, out int i) {
        i = i_;
        Expect(text, "\"", i, out i);
        int start = i;
        bool wasBackslash = false;
        string content = "";
        bool finished = false;
        for (; i < text.Length; i++) {
            char chr = text[i];
            if (wasBackslash) {
                wasBackslash = false;
            } else {
                if (chr == '\\') {
                    wasBackslash = true;
                } else if (chr == '"') {
                    finished = true;
                    i++;
                    break;
                }
            }
            content += chr;
        }
        if (!finished) {
            throw new InvalidJSONException(
                "Unterminated string", new JSONSpan(i_)
            );
        }
        try {
            return FromLiteral(content, false);
        } catch (LiteralDecodeException e) {
            throw new InvalidJSONException(
                e.Message, new JSONSpan(e.Position+start)
            );
        }
    }

    static NumUnion ExpectNumber(string text, int i_, out int i) {
        for (i = i_; i < text.Length; i++) {
            if (!Whitespace.Contains(text[i])) break;
        }
        if (i == text.Length) {
            throw new InvalidJSONException(
                $"Expected number, found EOF", new JSONSpan(i)
            );
        }
        int start = i;
        string numberText = "";
        string numberChars = "0123456789E+-.";
        bool isDouble = false;
        bool isStart = true;
        for (; i < text.Length; i++) {
            char chr = text[i];
            if (chr == 'e') chr = 'E';
            if (!numberChars.Contains(chr)) {
                if (isStart) {
                    throw new InvalidJSONException(
                        $"Expected number, found {ToLiteralChar(chr)}", new JSONSpan(i)
                    );
                }
                break;
            }
            if (chr == '.' || chr == 'E') isDouble = true;
            numberText += chr;
            isStart = false;
        }
        try {
            if (isDouble) {
                return new NumUnion(Double.Parse(
                    numberText, System.Globalization.NumberStyles.Float
                ));
            } else {
                return new NumUnion(Int32.Parse(numberText));
            }
        } catch (FormatException) {
            throw new InvalidJSONException(
                $"Invalid number", new JSONSpan(start, i-1)
            );
        }
    }

    static void ExpectEOF(string text, int i_, out int i) {
        for (i = i_; i < text.Length; i++) {
            if (!Whitespace.Contains(text[i])) {
                throw new InvalidJSONException(
                    $"Expected EOF, found {ToLiteralChar(text[i])}", new JSONSpan(i)
                );
            }
        }
    }

    public static IJSONValue ParseJSON(string text) {
        IJSONValue result = ExpectJSON(text, 0, out int i);
        ExpectEOF(text, i, out i);
        return result;
    }

    static IJSONValue ExpectJSON(string text, int i_, out int i) {
        i = i_;
        char? chr = NextChar(text, i, out int valueStart);
        if (chr == null) {
            throw new InvalidJSONException(
                $"Expected value, found EOF", new JSONSpan(i+1)
            );
        }
        int start = valueStart-1;
        IJSONValue result;
        if (chr.Value == '"') {
            string str = ExpectString(text, i, out i);
            result = new JSONString(str);
            result.span = new JSONSpan(start, i-1);
        } else if (chr.Value == '-' || ('0' <= chr.Value && chr.Value <= '9')) {
            NumUnion num = ExpectNumber(text, i, out i);
            if (num.HasInt()) {
                result = new JSONInt(num.GetInt());
            } else {
                result = new JSONDouble(num.GetDouble());
            }
            result.span = new JSONSpan(start, i-1);
        } else if (chr.Value == '{') {
            i = valueStart;
            chr = Peek(text, i);
            if (chr == null) {
                throw new InvalidJSONException(
                    "Expected value or '}', found EOF", new JSONSpan(i+1)
                );
            }
            JSONObject obj = new JSONObject();
            if (chr == '}') {
                Expect(text, "}", i, out i);
            } else {
                string sep = null;
                do {
                    string key = ExpectString(text, i, out i);
                    Expect(text, ":", i, out i);
                    IJSONValue value = ExpectJSON(text, i, out i);
                    obj[key] = value;
                    sep = ExpectAny(
                        text, new string[] {",", "}"}, i, out i
                    );
                } while (sep != "}");
            }
            obj.span = new JSONSpan(start, i-1);
            result = obj;
        } else if (chr.Value == '[') {
            i = valueStart;
            chr = Peek(text, i);
            if (chr == null) {
                throw new InvalidJSONException(
                    "Expected value or ']', found EOF", new JSONSpan(i+1)
                );
            }
            JSONList list = new JSONList();
            if (chr == ']') {
                Expect(text, "]", i, out i);
            } else {
                string sep = null;
                do {
                    IJSONValue value = ExpectJSON(text, i, out i);
                    list.Add(value);
                    sep = ExpectAny(
                        text, new string[] {",", "]"}, i, out i
                    );
                } while (sep != "]");
            }
            list.span = new JSONSpan(start, i-1);
            result = list;
        } else if ('A' <= chr.Value && chr.Value <= 'z') {
            string found = ExpectAny(
                text, new string[] {"true", "false", "null"}, i, out i
            );
            if (found == "true") {
                result = new JSONBool(true);
            } else if (found == "false") {
                result = new JSONBool(false);
            } else if (found == "null") {
                result = new JSONNull();
            } else {
                throw new InvalidOperationException();
            }
            result.span = new JSONSpan(start, i-1);
        } else {
            throw new InvalidJSONException(
                $"Expected value, found {ToLiteralChar(chr.Value)}", new JSONSpan(i)
            );
        }
        return result;
    }

    public static void ShowError(string text, InvalidJSONException err, string file=null, int showAroundErr=5) {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write("Error");
        if (file != null) {
            Console.Write(" in ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(file);
        }
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write(": ");
        Console.ResetColor();
        Console.WriteLine(err.Message);
        int start = err.span.GetStart();
        int end = err.span.GetEnd();
        bool endOverflow = end >= text.Length;
        if (endOverflow) end = text.Length-1;
        int startLine = 1;
        for (int i = 0; i < start; i++) {
            if (text[i] == '\n') startLine++;
        }
        int endLine = startLine;
        for (int i = start; i <= end; i++) {
            if (text[i] == '\n') endLine++;
        }
        int showStart = Math.Max(Math.Min(start, text.Length-1)-showAroundErr, 0);
        int showEnd = Math.Min(end+showAroundErr, text.Length-1);
        for (int i = showStart; i <= showEnd; i++) {
            if (text[i] == '\n') continue;
            Console.Write(text[i]);
        }
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Red;
        for (int i = showStart; i <= showEnd; i++) {
            if (text[i] == '\n') continue;
            if (start <= i && i <= end) {
                Console.Write('^');
            } else {
                Console.Write(' ');
            }
        }
        if (endOverflow) {
            Console.Write("^");
        }
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write("Position ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(start);
        if (end != start) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("-");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(end);
        }
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write(startLine == endLine ? ", line " : ", lines ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(startLine);
        if (endLine != startLine) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("-");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(endLine);
        }
        Console.ResetColor();
        Console.WriteLine();
    }
}
