using Application.Enums;

namespace Application.Common.Models;

public class Result<T>
{
    public string? Message { get; private set; }
    public TypeResult TypeResult { get; private set; }
    public T? Data { get; private set; }


    public static Result<T> Success(T data) =>
        new Result<T> { Message = "The operation was a success.", TypeResult = TypeResult.Success, Data = data };

    public static Result<T> NotFound(string message = "The operation was not found.") =>
        new Result<T> { Message = message, TypeResult = TypeResult.NotFound };

    public static Result<T> Created(T data) => new Result<T>()
    { Message = "The operation was created successfully.", TypeResult = TypeResult.Created, Data = data };

    public static Result<T> Duplicate(string message = "The operation was duplicated.") => new Result<T>()
    { Message = message, TypeResult = TypeResult.Duplicated };

    public static Result<T> Unauthorized(string message = "The operation was unauthorized.") => new Result<T>()
    { Message = message, TypeResult = TypeResult.Unauthorized };

    public static Result<T> Failure(string message = "The operation has failed.") => new Result<T>()
    { Message = message, TypeResult = TypeResult.Failure };

    public static Result<T> NoContent() => new Result<T>
    {
        TypeResult = TypeResult.NoContent
    };

}