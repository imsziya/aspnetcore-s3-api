namespace API.Aspnetcore.S3.Models;

public record ObjectResponse(string BucketName, List<string> Keys);