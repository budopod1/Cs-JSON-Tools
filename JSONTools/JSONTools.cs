using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

public static class JSONTools {
    static string ENList(IEnumerable<string> enumerable, string joiner="and") {
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

    public static string FromLiteral(string input) {
        // not including quotes
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
                            throw new InvalidJSONException(
                                "Expected unicode code after '\\u', found EOF", 
                                new JSONSpan(i)
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
                                throw new InvalidJSONException(
                                    $"Expected hex digit, found {nibble}", 
                                    new JSONSpan(i+j+1)
                                );
                            }
                            scale *= 16;
                        }
                        result.Append((char)code);
                        i += 4;
                        break;
                    default:
                        throw new InvalidJSONException(
                            $"Invalid escape code, '\\{chr}'", new JSONSpan(i)
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

    public static char[] Whitespace = {' ', '\n', '\t'};

    static void Expect(string text, string next, int i_, out int i) {
        i = i_;
        while (true) {
            if (i >= text.Length) {
                throw new InvalidJSONException(
                    $"Expected '{next}', found EOF", new JSONSpan(i)
                );
            }
            if (text.Substring(i).StartsWith(next)) {
                i += next.Length;
                return;
            }
            if (!Whitespace.Contains(text[i])) {
                throw new InvalidJSONException(
                    $"Expected '{next}', found '{text[i]}'", new JSONSpan(i)
                );
            }
            i++;
        }
    }

    static string ExpectAny(string text, string[] nexts, int i_, out int i) {
        i = i_;
        string expected = ENList(nexts.Select(next=>$"'{next}'"), "or");
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
                    $"Expected {expected}, found '{text[i]}'", new JSONSpan(i)
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
            return FromLiteral(content);
        } catch (InvalidJSONException e) {
            e.span = e.span?.Shift(start);
            throw e;
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
                        $"Expected number, found '{chr}'", new JSONSpan(i)
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

    public static IJSONValue ParseJSON(string text) {
        return ParseJSON(text, 0, out int i);
    }

    static IJSONValue ParseJSON(string text, int i_, out int i) {
        i = i_;
        char? chr = NextChar(text, i, out int valueStart);
        if (chr == null) {
            throw new InvalidJSONException(
                $"Expected value, found EOF", new JSONSpan(i+1)
            );
        }
        int start = i;
        if (chr.Value == '"') {
            string str = ExpectString(text, i, out i);
            JSONString jstr = new JSONString(str);
            jstr.span = new JSONSpan(start, i-1);
            return jstr;
        } else if (chr.Value == '-' || ('0' <= chr.Value && chr.Value <= '9')) {
            NumUnion num = ExpectNumber(text, i, out i);
            IJSONValue value;
            if (num.HasInt()) {
                value = new JSONInt(num.GetInt());
            } else {
                value = new JSONDouble(num.GetDouble());
            }
            value.span = new JSONSpan(start, i-1);
            return value;
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
                    IJSONValue value = ParseJSON(text, i, out i);
                    obj[key] = value;
                    sep = ExpectAny(
                        text, new string[] {",", "}"}, i, out i
                    );
                } while (sep != "}");
            }
            obj.span = new JSONSpan(start, i-1);
            return obj;
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
                    IJSONValue value = ParseJSON(text, i, out i);
                    list.Add(value);
                    sep = ExpectAny(
                        text, new string[] {",", "]"}, i, out i
                    );
                } while (sep != "]");
            }
            list.span = new JSONSpan(start, i-1);
            return list;
        } else if ('A' <= chr.Value && chr.Value <= 'z') {
            string found = ExpectAny(
                text, new string[] {"true", "false", "null"}, i, out i
            );
            IJSONValue value;
            if (found == "true") {
                value = new JSONBool(true);
            } else if (found == "false") {
                value = new JSONBool(false);
            } else if (found == "null") {
                value = new JSONNull();
            } else {
                throw new InvalidOperationException();
            }
            value.span = new JSONSpan(start, i-1);
            return value;
        } else {
            throw new InvalidJSONException(
                $"Expected value, found '{chr.Value}'", new JSONSpan(i)
            );
        }
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
        int showStart = Math.Max(Math.Min(start, text.Length-1)-showAroundErr, 0);
        int showEnd = Math.Min(end+showAroundErr, text.Length-1);
        for (int i = showStart; i <= showEnd; i++) {
            Console.Write(text[i]);
        }
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Red;
        for (int i = showStart; i <= showEnd; i++) {
            if (start <= i && i <= end) {
                Console.Write('^');
            } else {
                Console.Write(' ');
            }
        }
        if (end >= text.Length) {
            Console.Write("^");
        }
        Console.ResetColor();
        Console.WriteLine();
    }
}
