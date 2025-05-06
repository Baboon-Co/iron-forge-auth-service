using System.Text;
using Google.Rpc;

namespace Api.Extensions;

public static class GrpcsBadRequestExtensions
{
    public static string ToDebugString(this BadRequest badRequest)
    {
        var builder = new StringBuilder();
        foreach (var v in badRequest.FieldViolations)
            builder.AppendLine($"{v.Field}: {v.Description}");
        return builder.ToString();
    }
}