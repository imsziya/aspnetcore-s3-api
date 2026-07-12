namespace API.Aspnetcore.S3.Models;

public class ObjectResponse
{
    public string BucketName { get; set; } = string.Empty;

    public List<string> Keys { get; set; } = [];
}
