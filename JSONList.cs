using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

public class JSONList : Collection<IJSONValue>, IJSONValue {
    public JSONSpan span { get; set; }
    public IEnumerable<byte> ID => new List<byte> {4};
    
    public JSONList(IEnumerable<IJSONValue> values) {
        foreach (IJSONValue value in values) Add(value);
    }

    public JSONList() {}

    public string ToJSON() {
        return $"[{String.Join(", ", this.Select(item => item.ToJSON()))}]";
    }

    public IEnumerable<byte> ToBJSON(BJSONEnv env) {
        if (Count == 0) {
            return new List<byte> {0};
        }
        IEnumerable<byte> itemID = this[0].ID;
        IEnumerable<byte> result = BJSONEnv.ToVWInt(Count).Concat(itemID);
        foreach (IJSONValue val in this) {
            if (val.ID.First() != itemID.First()) {
                throw new InconsistentJSONTypesException(this);
            }
            result = result.Concat(val.ToBJSON(env));
        }
        return result;
    }
}
