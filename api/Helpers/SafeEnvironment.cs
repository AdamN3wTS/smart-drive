using System;

namespace api.Helpers;

public static class SafeEnvironment
{
    public static string GetSafeEnvironment(string variable)
    {
        var value = Environment.GetEnvironmentVariable(variable);
        if (value==null)
        {
            throw new NullReferenceException();
        }
        return value;
    }
}
