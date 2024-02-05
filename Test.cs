using System;
using System.IO;
using System.Collections.Generic;

public class Test {
    public static int Main(string[] args) {
        string text = "{\"foo\": [3, 4, 4.5\n, false], \"bar\": {\"z\": false, \"x\": null}}";
        
        try {
            IJSONValue val = JSONTools.ParseJSON(text);

            Console.WriteLine(val.ToJSON());

            ShapedJSON data = new ShapedJSON(val, new JSONObjectShape(new Dictionary<string, IJSONShape> {
                {"foo", new JSONListShape(new JSONDoubleShape())},
                {"bar", new JSONObjectAnyKeyShape(new JSONBoolShape())}
            }));
        } catch (InvalidJSONException err) {
            JSONTools.ShowError(text, err, "foobar.json");
        }
        
        return 0;
    }
}
