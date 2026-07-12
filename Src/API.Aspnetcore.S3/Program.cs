using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using API.Aspnetcore.S3.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

AWSOptions aWSOptions = builder.Configuration.GetAWSOptions();
builder.Services.AddDefaultAWSOptions(aWSOptions);
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddScoped<IBucketService, BucketService>();
builder.Services.AddScoped<IObjectService, ObjectService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
