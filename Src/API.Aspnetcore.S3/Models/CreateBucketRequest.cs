namespace API.Aspnetcore.S3.Models;

public class CreateBucketRequest
{
    public string BucketName { get; set; } = string.Empty;
}

public class CreateFoldrRequest : CreateBucketRequest
{
    public string FolderName { get; set; } = string.Empty;
}
