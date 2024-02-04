using System;
using System.IO;
using System.Collections.Generic;

public class Test {
    public static int Main(string[] args) {
        JSONObject obj = new JSONObject();
        obj["foo"] = new JSONString("bar\nbaz");

        Console.WriteLine(obj.ToJSON());
        
        return 0;
    }
}
