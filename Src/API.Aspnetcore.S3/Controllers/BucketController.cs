using API.Aspnetcore.S3.Models;
using API.Aspnetcore.S3.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Aspnetcore.S3.Controllers;

[Route("api/buckets")]
[ApiController]
public class BucketController(IBucketService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateBucketAsync(CreateBucketRequest request)
    {
        await service.CreateBucketAsync(request.BucketName);

        return Ok("Bucket created successfully.");
    }
    [HttpPost("folder")]
    public async Task<IActionResult> CreateFolderAsync(CreateFolderRequest request)
    {
        await service.CreateFolderAsync(request.BucketName, request.FolderName);

        return Ok("Folder created successfully.");
    }

    [HttpGet]
    public async Task<IActionResult> GetBuckets()
    {
        return Ok(await service.GetBucketsAsync());
    }

    [HttpDelete("{bucketName}")]
    public async Task<IActionResult> Delete(string bucketName)
    {
        await service.DeleteBucketAsync(bucketName);

        return Ok();
    }

    [HttpGet("{bucketName}/location")]
    public async Task<IActionResult> Location(string bucketName)
    {
        return Ok(await service.GetBucketLocationAsync(bucketName));
    }
}
