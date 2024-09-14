using System.Net.Sockets;

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

        UnknowError = 200,
        DatabaseError = 201,
        ServiceError = 202,
        NetError = 203,
        SocketError = 204,
    }

    public class Errors
    {
        public required int Code { get; set; }
        public required string Message { get; set; }  // Send to client
        public object? Data { get; set; }

        public static readonly Dictionary<ErrorType, Errors> Dict = new Dictionary<ErrorType, Errors>
        {
            { ErrorType.NoError, new Errors { Code = (int)ErrorType.NoError, Message = "No error" } },

            { ErrorType.InvalidArgument, new Errors { Code = (int)ErrorType.InvalidArgument, Message = "Invalid argument." } },
            { ErrorType.RecordDuplication, new Errors { Code = (int)ErrorType.RecordDuplication, Message = "Record duplication." } },
            { ErrorType.NotExist, new Errors { Code = (int)ErrorType.NotExist, Message = "Record does not exist." } },

            { ErrorType.UnknowError, new Errors { Code = (int)ErrorType.UnknowError, Message = "Service exception." } },
            { ErrorType.DatabaseError, new Errors { Code = (int)ErrorType.DatabaseError, Message = "Service exception." } },
            { ErrorType.ServiceError, new Errors { Code = (int)ErrorType.ServiceError, Message = "Service exception." } },
            { ErrorType.NetError, new Errors { Code = (int)ErrorType.NetError, Message = "Service exception." } },
            { ErrorType.SocketError, new Errors { Code = (int)ErrorType.SocketError, Message = "Service exception." } },
        };

        public static Errors NoError<T>(T data) { return new Errors { Code = (int)ErrorType.NoError, Message = "", Data = data }; }
    }
}
