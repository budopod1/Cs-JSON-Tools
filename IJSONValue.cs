namespace CsJSONTools;
public interface IJSONValue {
    IEnumerable<byte> ID { get; }
    JSONSpan span { get; set; }

    string Stringify();
    string PrettyPrint(PrettyPrintConfig config);
    IEnumerable<byte> ToBinJSON(BinJSONEnv env);
}
