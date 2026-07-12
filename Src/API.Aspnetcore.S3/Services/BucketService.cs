using Amazon.S3;
using Amazon.S3.Model;

namespace API.Aspnetcore.S3.Services;

public interface IBucketService
{
    Task CreateBucketAsync(string bucketName);

    Task<List<string>> GetBucketsAsync();

    Task DeleteBucketAsync(string bucketName);

    Task<string> GetBucketLocationAsync(string bucketName);

    Task CreateFolderAsync(string bucketName, string folderName);

}

public class BucketService(IAmazonS3 _amazonS3) : IBucketService
{

    public async Task CreateBucketAsync(string bucketName)
    {
        try
        {
            await _amazonS3.EnsureBucketExistsAsync(bucketName);
        }
        catch (Exception)
        {
            throw new Exception("Bucket already exists.");
        }
    }

    public async Task CreateFolderAsync(string bucketName, string folderName)
    {
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = folderName + "/",
            ContentBody = ""
        };

        await _amazonS3.PutObjectAsync(request);
    }

    public async Task DeleteBucketAsync(string bucketName) => await _amazonS3.DeleteBucketAsync(bucketName);

    public async Task<string> GetBucketLocationAsync(string bucketName)
    {
        var response = await _amazonS3.GetBucketLocationAsync(bucketName);

        return response.Location.Value;
    }

    public async Task<List<string>> GetBucketsAsync()
    {
        var response = await _amazonS3.ListBucketsAsync();

        return [.. response.Buckets.Select(x => x.BucketName)];
    }
}
