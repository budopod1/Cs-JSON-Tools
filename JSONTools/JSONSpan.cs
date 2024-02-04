using System;
using System.Collections.Generic;
using System.Linq;

public class JSONSpan {
    int start;
    int end;

    public JSONSpan(int start, int end) {
        this.start = start;
        this.end = end;
    }

    public JSONSpan(int i) {
        this.start = i;
        this.end = i;
    }

    public int GetStart() {
        return start;
    }

    public int GetEnd() {
        return end;
    }

    public int Size() {
        return end - start + 1;
    }

    public static JSONSpan Merge(IEnumerable<JSONSpan> spans) {
        IEnumerable<JSONSpan> nonNull = spans.Where(
            span => span != null
        );
        if (nonNull.Count() == 0) return null;
        int start = nonNull.Select(span => span.GetStart()).Min();
        int end = nonNull.Select(span => span.GetEnd()).Max();
        return new JSONSpan(start, end);
    }

    public static JSONSpan Merge(JSONSpan a, JSONSpan b) {
        return Merge(new List<JSONSpan> {a, b});
    }

    public override string ToString() {
        return $"{GetType().Name}({start}–{end})";
    }

    public JSONSpan Shift(int amount) {
        return new JSONSpan(start + amount, end + amount);
    }
}
