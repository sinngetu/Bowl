using System.Diagnostics;

namespace Bowl.Common
{
    public delegate void LogMethod(string? message, params object?[] args);

    public class Utils
    {
        static public IConfiguration config { get; private set; }

        static Utils()
        {
            var builder = new ConfigurationBuilder().AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: true
            );

            config = builder.Build();
        }

        static public void Log(LogMethod log, params object?[] args)
        {
            var methodBase = new StackTrace()?.GetFrame(1)?.GetMethod();
            var _class = methodBase?.DeclaringType;
            var _method = methodBase?.Name;

            log($"{_class} => {_method}", args);
        }

        static public Errors ErrorHandle(ErrorType type, object? data)
        {
            var isSuccessAndHasData = (type == ErrorType.NoError) && (data != null);

            return isSuccessAndHasData
                ? Errors.NoError(data)
                : Errors.Dict[type];
        }
    }
}
