using System;

public class InconsistentJSONTypesException : Exception {
    public IJSONValue JSON;

    public InconsistentJSONTypesException(IJSONValue JSON) {
        this.JSON = JSON;
    }
}
