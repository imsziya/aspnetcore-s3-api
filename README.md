# ASP.NET Core S3 API

REST API built with ASP.NET Core for common Amazon S3 operations such as bucket management, folder placeholder creation, object upload, object listing, object download, and pre-signed download URL generation.

## Tech Stack

- ASP.NET Core Web API
- .NET 10
- AWS SDK for .NET
- Swagger / OpenAPI

## Architecture

This application uses a simple layered ASP.NET Core Web API architecture. It is organized around HTTP controllers, application services, lightweight models, and the AWS SDK client registered through dependency injection.

The request flow is:

```text
HTTP Request
    ↓
Controller
    ↓
Service Interface
    ↓
Service Implementation
    ↓
AWS SDK IAmazonS3 Client
    ↓
Amazon S3
```

### Layers

| Layer | Location | Responsibility |
| --- | --- | --- |
| API / Presentation Layer | `Controllers` | Exposes REST endpoints, receives route/query/body/form-data input, and returns HTTP responses. |
| Service Layer | `Services` | Contains the application logic for S3 bucket and object operations. Controllers depend on service interfaces instead of directly using the AWS SDK. |
| Model Layer | `Models` | Defines request and response DTOs used by the API. |
| Infrastructure / External Service Layer | AWS SDK `IAmazonS3` | Provides access to Amazon S3. The client is registered in `Program.cs` using `AddAWSService<IAmazonS3>()`. |
| Composition Root | `Program.cs` | Configures controllers, Swagger/OpenAPI, AWS options, dependency injection, middleware, and endpoint mapping. |

### Dependency Injection

Services are registered in `Program.cs`:

```csharp
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddScoped<IBucketService, BucketService>();
builder.Services.AddScoped<IObjectService, ObjectService>();
```

This keeps controllers thin and allows S3-specific logic to live in the service layer:

- `BucketController` depends on `IBucketService`.
- `ObjectController` depends on `IObjectService`.
- `BucketService` and `ObjectService` depend on `IAmazonS3`.

### Architectural Style

The project is best described as a monolithic, layered REST API. It is not using Clean Architecture, CQRS, Repository Pattern, MediatR, domain-driven design, or a microservice split. Because the current scope is focused on S3 operations, the layers are intentionally lightweight and live inside a single ASP.NET Core project.

## Project Structure

```text
.
├── README.md
├── LICENSE
└── Src
    ├── API.Aspnetcore.S3.slnx
    └── API.Aspnetcore.S3
        ├── Controllers
        │   ├── BucketController.cs
        │   └── ObjectController.cs
        ├── Models
        │   ├── CreateBucketRequest.cs
        │   └── ObjectResponse.cs
        ├── Services
        │   ├── BucketService.cs
        │   └── ObjectService.cs
        ├── Program.cs
        ├── appsettings.json
        └── API.Aspnetcore.S3.csproj
```

## Features

- Create an S3 bucket.
- List all accessible S3 buckets.
- Delete an S3 bucket.
- Get a bucket location.
- Create an S3 folder placeholder by creating an empty object with a trailing slash key.
- Upload one object.
- Upload multiple objects.
- List object keys in a bucket.
- Download an object.
- Generate a pre-signed download URL.

`ObjectService` also contains methods for deleting objects and generating pre-signed upload URLs, but those methods are not currently exposed through `ObjectController`.

## Prerequisites

- .NET 10 SDK
- AWS account with S3 access
- AWS credentials configured for the AWS SDK default credential chain

The application registers `IAmazonS3` through `AddAWSService<IAmazonS3>()`, so credentials and region are resolved by the standard AWS SDK configuration providers, including environment variables, the shared AWS credentials/config files, and IAM roles when running on AWS infrastructure.

## AWS Configuration

For local development, the simplest setup is to configure the AWS CLI:

```bash
aws configure
```

Provide:

- AWS Access Key ID
- AWS Secret Access Key
- Default AWS Region
- Default output format, such as `json`

You can also use a named profile:

```bash
aws configure --profile myprofile
export AWS_PROFILE=myprofile
```

The AWS identity used by the API needs permissions for the S3 operations you call. Common permissions include:

- `s3:ListAllMyBuckets`
- `s3:CreateBucket`
- `s3:DeleteBucket`
- `s3:GetBucketLocation`
- `s3:ListBucket`
- `s3:PutObject`
- `s3:GetObject`
- `s3:DeleteObject`

For production, prefer least-privilege IAM policies and avoid storing long-lived credentials in application configuration.

## Configuration

The checked-in `appsettings.json` only configures logging and allowed hosts:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

If you want to configure AWS options explicitly through configuration, the AWS SDK for .NET can read an `AWS` section, for example:

```json
{
  "AWS": {
    "Profile": "myprofile",
    "Region": "us-east-1"
  }
}
```

Do not commit access keys or secret keys to this file.

## Run Locally

Restore and build:

```bash
dotnet restore Src/API.Aspnetcore.S3.slnx
dotnet build Src/API.Aspnetcore.S3.slnx
```

Run the API:

```bash
dotnet run --project Src/API.Aspnetcore.S3/API.Aspnetcore.S3.csproj
```

The launch profile configures these local URLs:

- HTTP: `http://localhost:5260`
- HTTPS: `https://localhost:7093`

Swagger UI is enabled in the Development environment:

```text
https://localhost:7093/swagger
```

## API Endpoints

### Buckets

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/api/buckets` | Create a bucket. |
| `POST` | `/api/buckets/folder` | Create a folder placeholder in a bucket. |
| `GET` | `/api/buckets` | List accessible buckets. |
| `GET` | `/api/buckets/{bucketName}/location` | Get a bucket location. |
| `DELETE` | `/api/buckets/{bucketName}` | Delete a bucket. |

### Objects

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/api/objects/upload/{bucketName}` | Upload one file as multipart form data. |
| `POST` | `/api/objects/uploads/{bucketName}` | Upload multiple files as multipart form data. |
| `GET` | `/api/objects?bucketName={bucketName}` | List object keys in a bucket. |
| `GET` | `/api/objects/download?bucket={bucketName}&key={objectKey}` | Download an object. |
| `GET` | `/api/objects/pre-signed-url?bucketName={bucketName}&key={objectKey}&expiryMinutes={minutes}` | Generate a pre-signed download URL. |

## Request Examples

Create a bucket:

```bash
curl -X POST http://localhost:5260/api/buckets \
  -H "Content-Type: application/json" \
  -d '{"bucketName":"my-s3-bucket"}'
```

Create a folder placeholder:

```bash
curl -X POST http://localhost:5260/api/buckets/folder \
  -H "Content-Type: application/json" \
  -d '{"bucketName":"my-s3-bucket","folderName":"documents"}'
```

List buckets:

```bash
curl http://localhost:5260/api/buckets
```

Upload a file:

```bash
curl -X POST http://localhost:5260/api/objects/upload/my-s3-bucket \
  -F "file=@./example.txt"
```

Upload multiple files:

```bash
curl -X POST http://localhost:5260/api/objects/uploads/my-s3-bucket \
  -F "files=@./example-1.txt" \
  -F "files=@./example-2.txt"
```

List objects:

```bash
curl "http://localhost:5260/api/objects?bucketName=my-s3-bucket"
```

Download an object:

```bash
curl -L "http://localhost:5260/api/objects/download?bucket=my-s3-bucket&key=example.txt" \
  -o example.txt
```

Generate a pre-signed download URL:

```bash
curl "http://localhost:5260/api/objects/pre-signed-url?bucketName=my-s3-bucket&key=example.txt&expiryMinutes=15"
```

Delete a bucket:

```bash
curl -X DELETE http://localhost:5260/api/buckets/my-s3-bucket
```

## Development Notes

- Swagger and OpenAPI are only mapped when `ASPNETCORE_ENVIRONMENT` is `Development`.
- Bucket deletion requires the bucket to be empty.
- Uploaded object keys use the incoming file name.
- There is no authentication or authorization policy configured in this API yet, even though `UseAuthorization()` is present in the pipeline.
- Exceptions are not currently translated into custom API error responses, so AWS SDK exceptions may surface as default ASP.NET Core error responses.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
