using System;

public interface IJSONObjectShape : IJSONShape {
    IJSONShape GetSub(string key);
}
