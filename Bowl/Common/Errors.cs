namespace Bowl.Common
{
    /**
     * 1 ~ 99: Reserve
     * 100 ~ 199: Client Error
     * 200 ~ 299: Service Error
     */
    public enum ErrorType
    {
        NoError = 0,

        InvalidArgument = 101,
        RecordDuplication = 102,
        NotExist = 103,

        DatabaseError = 201,
    }

    public class Errors
    {
        public required int Code { get; set; }
        public required string Message { get; set; }
        public object? Content { get; set; }

        public static readonly Dictionary<ErrorType, Errors> Dict = new Dictionary<ErrorType, Errors>
        {
            { ErrorType.InvalidArgument, new Errors { Code = (int)ErrorType.InvalidArgument, Message = "Invalid argument." } },
            { ErrorType.RecordDuplication, new Errors { Code = (int)ErrorType.RecordDuplication, Message = "Record duplication." } },
            { ErrorType.NotExist, new Errors { Code = (int)ErrorType.NotExist, Message = "Record does not exist." } },
            { ErrorType.DatabaseError, new Errors { Code = (int)ErrorType.DatabaseError, Message = "Service exception." } },
        };

        static public Errors NoError<T>(T content) { return new Errors { Code = (int)ErrorType.NoError, Message = "", Content = content }; }
    }
}
