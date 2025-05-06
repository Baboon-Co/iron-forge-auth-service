using FluentResults;
using Infrastructure.ResultErrors.Enums;

namespace Infrastructure.ResultErrors;

public class GrpcResultError(ResponseErrorType errorType, string message = "Error occured.") : Error(message)
{
    public ResponseErrorType ErrorType { get; } = errorType;
}