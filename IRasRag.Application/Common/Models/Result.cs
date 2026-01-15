namespace IRasRag.Application.Common.Models
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public ResultType Type { get; set; }
        public static Result Success(string message)
            => new() { IsSuccess = true, Message = message, Type = ResultType.Ok };
        public static Result Failure(string message, ResultType type)
            => new() { IsSuccess = false, Message = message, Type = type };
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }
        public static Result<T> Success(T data, string? message = null)
            => new() { IsSuccess = true, Data = data, Message = message, Type = ResultType.Ok };
        public new static Result<T> Failure(string message, ResultType type)
            => new() { IsSuccess = false, Message = message, Type = type};
    }

    public enum ResultType
    {
        Ok,
        NotFound,
        BadRequest,
        Unauthorized,
        Conflict,
        Unexpected
    }
}
