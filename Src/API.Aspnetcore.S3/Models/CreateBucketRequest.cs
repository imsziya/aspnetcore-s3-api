namespace API.Aspnetcore.S3.Models;

public record CreateBucketRequest(string BucketName);

public record CreateFolderRequest(string BucketName, string FolderName) : CreateBucketRequest(BucketName);
