namespace CsJSONTools;
public interface IJSONObjectShape : IJSONShape {
    IJSONShape GetSub(string key);
}
