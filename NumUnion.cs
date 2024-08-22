public class NumUnion {
    readonly int? i = null;
    readonly double? d = null;

    public NumUnion(int? num) {
        i = num;
    }

    public NumUnion(double? num) {
        d = num;
    }

    public bool HasInt() {
        return i != null;
    }

    public bool HasDouble() {
        return d != null;
    }

    public int GetInt() {
        return i.Value;
    }

    public double GetDouble() {
        return d.Value;
    }
}
