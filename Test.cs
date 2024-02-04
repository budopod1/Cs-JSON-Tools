using System;
using System.IO;
using System.Collections.Generic;

public class Test {
    public static int Main(string[] args) {
        try {
            IJSONValue val = JSONTools.ParseJSON(
                "{\"foo\": [3, null, -3.4e8, true, \"\\u03B5\\u03B5\"]}"
            );

            Console.WriteLine(val.ToJSON());
        } catch (InvalidJSONException e) {
            Console.WriteLine(e.span);
            Console.WriteLine(e.Message);
        }
        
        return 0;
    }
}
