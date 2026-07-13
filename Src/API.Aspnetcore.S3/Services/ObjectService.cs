using Amazon.S3;
using Amazon.S3.Model;
using API.Aspnetcore.S3.Models;

namespace API.Aspnetcore.S3.Services;

public interface IObjectService
{
    Task<string> UploadFileAsync(string bucketName, IFormFile file);

    Task<List<string>> UploadFilesAsync(string bucketName, List<IFormFile> files);
    Task<GetObjectResponse> DownloadFileAsync(string bucketName, string key);

    Task<ObjectResponse> GetObjectsAsync(string bucketName);

    Task DeleteObjectAsync(string bucketName, string key);

    Task DeleteObjectsAsync(string bucketName, List<string> keys);

    Task<string> GenerateDownloadUrlAsync(string bucketName,
                                         string key,
                                         int expiryMinutes);

    Task<string> GenerateUploadUrlAsync(string bucketName,
                                        string key,
                                        int expiryMinutes);
}

public class ObjectService(IAmazonS3 _amazonS3) : IObjectService
{
    public async Task DeleteObjectAsync(string bucketName, string key) => await _amazonS3.DeleteObjectAsync(bucketName, key);

    public async Task DeleteObjectsAsync(string bucketName, List<string> keys)
    {
        var request = new DeleteObjectsRequest
        {
            BucketName = bucketName,
            Objects = [.. keys
            .Select(x => new KeyVersion
            {
                Key = x
            })]
        };

        await _amazonS3.DeleteObjectsAsync(request);
    }

    public async Task<GetObjectResponse> DownloadFileAsync(string bucketName, string key) =>
        await _amazonS3.GetObjectAsync(bucketName, key);

    public async Task<string> GenerateDownloadUrlAsync(string bucketName, string key, int expiryMinutes)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };

        return await _amazonS3.GetPreSignedURLAsync(request);
    }

    public Task<string> GenerateUploadUrlAsync(string bucketName, string key, int expiryMinutes)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };

        return Task.FromResult(_amazonS3.GetPreSignedURL(request));
    }

    public async Task<ObjectResponse> GetObjectsAsync(string bucketName)
    {
        var response = await _amazonS3.ListObjectsAsync(bucketName);
        return new ObjectResponse(bucketName, [.. response.S3Objects.Select(x => x.Key)]);
    }

    public async Task<string> UploadFileAsync(string bucketName, IFormFile file)
    {
        using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = file.FileName,
            InputStream = stream,
            ContentType = file.ContentType
        };

        await _amazonS3.PutObjectAsync(request);

        return file.FileName;
    }

    public async Task<List<string>> UploadFilesAsync(string bucketName, List<IFormFile> files)
    {
        var uploadedFiles = new List<string>();

        foreach (var file in files)
        {
            await UploadFileAsync(bucketName, file);
            uploadedFiles.Add(file.FileName);
        }

        return uploadedFiles;
    }
}
