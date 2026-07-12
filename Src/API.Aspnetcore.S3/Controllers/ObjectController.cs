using API.Aspnetcore.S3.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Aspnetcore.S3.Controllers;

[Route("api/objects")]
[ApiController]
public class ObjectController(IObjectService service) : ControllerBase
{
    [HttpPost("upload/{bucketName}")]
    public async Task<IActionResult> UploadFileAsync(string bucketName, IFormFile file)
    {
        var res = await service.UploadFileAsync(bucketName, file);

        return Ok(res);
    }

    [HttpPost("uploads/{bucketName}")]
    public async Task<IActionResult> UploadFilesAsync(string bucketName, List<IFormFile> files)
    {
        var res = await service.UploadFilesAsync(bucketName, files);

        return Ok(res);
    }

    [HttpGet("download")]
    public async Task<IActionResult> Download(
    string bucket,
    string key)
    {
        var response = await service.DownloadFileAsync(bucket, key);

        return File(response.ResponseStream,
                    response.Headers.ContentType,
                    key);
    }

    [HttpGet]
    public async Task<IActionResult> GetObjectsAsync(string bucketName)
    {
        var res = await service.GetObjectsAsync(bucketName);

        return Ok(res);
    }

    [HttpGet("pre-signed-url")]
    public async Task<IActionResult> GetPreSignedUrlAsync(string bucketName, string key, int expiryMinutes)
    {
        var res = await service.GenerateDownloadUrlAsync(bucketName, key, expiryMinutes);

        return Ok(res);
    }

}
