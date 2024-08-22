using System.Collections.ObjectModel;

public class JSONList : Collection<IJSONValue>, IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => [IsUnitype() ? (byte)4 : (byte)8];

    public JSONList(IEnumerable<IJSONValue> values) {
        foreach (IJSONValue value in values) Add(value);
    }

    public JSONList() {}

    public string Stringify() {
        return $"[{string.Join(", ", this.Select(item => item.Stringify()))}]";
    }

    public string PrettyPrint(PrettyPrintConfig config) {
        IEnumerable<string> stringifiedParts = this.Select(item => item.PrettyPrint(config));
        if (stringifiedParts.All(part => part.IndexOf('\n') == -1)) {
            string basicStringification = $"[{string.Join(", ", stringifiedParts)}]";
            if (basicStringification.Length < config.MaxLineContentLen) {
                return basicStringification;
            }
        }
        return $"[\n{config.Indent(string.Join(",\n", stringifiedParts))}\n]";
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

    public IEnumerable<byte> ToBinJSON(BinJSONEnv env) {
        if (IsUnitype()) {
            if (Count == 0) {
                return [0];
            }
            IEnumerable<byte> result = BinJSONEnv.ToVWInt((uint)Count).Concat(this[0].ID);
            foreach (IJSONValue val in this) {
                result = result.Concat(val.ToBinJSON(env));
            }
            return result;
        } else {
            IEnumerable<byte> result = BinJSONEnv.ToVWInt((uint)Count);
            foreach (IJSONValue val in this) {
                result = result.Concat(val.ID).Concat(val.ToBinJSON(env));
            }
            return result;
        }
    }
}
