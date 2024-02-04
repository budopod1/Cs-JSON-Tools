using System;
using System.IO;
using System.Collections.Generic;

public class Test {
    public static int Main(string[] args) {
        string text = "{\"foo\": [3, null, -3.4e8, true, \"\\u03B1\\u03B5\"]}";
        
        try {
            IJSONValue val = JSONTools.ParseJSON(text);

            Console.WriteLine(val.ToJSON());
        } catch (InvalidJSONException err) {
            JSONTools.ShowError(text, err, "foobar.txt");
        }
        
        return 0;
    }
}
