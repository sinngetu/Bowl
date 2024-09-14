using System.Diagnostics;

namespace Bowl.Common
{
    public delegate void LogMethod(string? message, params object?[] args);

    public static class Utils
    {
        public static IConfiguration config { get; private set; }
        public static IServiceScopeFactory serviceFactory { get; private set; }
        public static ILogger logger { get; private set; }

        static Utils()
        {
            var builder = new ConfigurationBuilder().AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: true
            );

            config = builder.Build();
        }

        public static void Initialize(IServiceProvider provider, ILogger log)
        {
            serviceFactory = provider.GetRequiredService<IServiceScopeFactory>();
            logger = log;
        }

        public static string GetClassNameAndMethodName()
        {
            var methodBase = new StackTrace()?.GetFrame(1)?.GetMethod();
            var _class = methodBase?.DeclaringType;
            var _method = methodBase?.Name;

            return $"[{_class} => {_method}] ";
        }

        public static Errors ErrorHandle(ErrorType type, object? data)
        {
            var isSuccessAndHasData = (type == ErrorType.NoError) && (data != null);

            return isSuccessAndHasData
                ? Errors.NoError(data)
                : Errors.Dict[type];
        }
    }
}
