using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

public class JSONList : Collection<IJSONValue>, IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => new List<byte> {IsUnitype() ? (byte)4 : (byte)8};
    
    public JSONList(IEnumerable<IJSONValue> values) {
        foreach (IJSONValue value in values) Add(value);
    }

    public JSONList() {}

    public string Stringify() {
        return $"[{String.Join(", ", this.Select(item => item.Stringify()))}]";
    }

    public string PrettyPrint(PrettyPrintConfig config) {
        IEnumerable<string> stringifiedParts = this.Select(item => item.PrettyPrint(config));
        if (stringifiedParts.Any(part => part.IndexOf('\n') == -1)) {
            string basicStringification = $"[{String.Join(", ", stringifiedParts)}]";
            if (basicStringification.Length < config.MaxLineContentLen) {
                return basicStringification;
            }
        }
        return $"[\n{config.Indent(String.Join(",\n", stringifiedParts))}\n]";
    }

    public bool IsUnitype() {
        if (Count == 0) return true;
        byte firstID = this[0].ID.First();
        foreach (IJSONValue val in this) {
            if (val.ID.First() != firstID) {
                return false;
            }
        }
        return true;
    }

    public IEnumerable<byte> ToBJSON(BJSONEnv env) {
        if (IsUnitype()) {
            if (Count == 0) {
                return new List<byte> {0};
            }
            IEnumerable<byte> result = BJSONEnv.ToVWInt(Count).Concat(this[0].ID);
            foreach (IJSONValue val in this) {
                result = result.Concat(val.ToBJSON(env));
            }
            return result;
        } else {
            IEnumerable<byte> result = BJSONEnv.ToVWInt(Count);
            foreach (IJSONValue val in this) {
                result = result.Concat(val.ID).Concat(val.ToBJSON(env));
            }
            return result;
        }
    }
}
