using System.Diagnostics;

namespace Bowl.Common
{
    public delegate void LogMethod(string? message, params object?[] args);

    public class Utils
    {
        public static void Log(LogMethod log, params object?[] args)
        {
            var methodBase = new StackTrace()?.GetFrame(1)?.GetMethod();
            var _class = methodBase?.DeclaringType;
            var _method = methodBase?.Name;

            log($"{_class} => {_method}", args);
        }

        public static Errors ErrorHandle(ErrorType type, object? data)
        {
            return type == ErrorType.NoError
                ? Errors.NoError(data)
                : Errors.Dict[type];
        }
    }
}
