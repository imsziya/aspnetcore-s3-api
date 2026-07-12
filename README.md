# aspnetcore-s3-api
RESTful API built with ASP.NET Core to perform Amazon S3 operations, including file upload, download, and storage management.

## Project Details

- **Framework:** ASP.NET Core (see `Src/API.Aspnetcore.S3`)
- **Features:** create/list buckets, upload/download objects, delete objects, and basic object metadata support.
- **Key files:** Controllers are in `Src/API.Aspnetcore.S3/Controllers`, services in `Src/API.Aspnetcore.S3/Services`.

## AWS Configuration (via AWS CLI)

This project expects AWS credentials and configuration to be provided via the AWS CLI. Configure your environment as follows.

1. Install the AWS CLI (if not already installed). Follow https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html for platform-specific instructions.

2. Configure credentials and default region using `aws configure` and provide an IAM user's Access Key ID and Secret Access Key with S3 permissions:

```bash
aws configure
# When prompted, enter your Access Key ID, Secret Access Key, default region (e.g. us-east-1), and default output (e.g. json)
```

3. (Optional) Use named profiles if you manage multiple accounts or roles:

```bash
aws configure --profile myprofile
# Use the profile by setting AWS_PROFILE=myprofile or configuring the SDK to use it
export AWS_PROFILE=myprofile
```

4. Quick S3 commands to verify access:

```bash
# List buckets
aws s3 ls

# Create a bucket
aws s3 mb s3://my-test-bucket --region us-east-1

# Upload a file
aws s3 cp ./localfile.txt s3://my-test-bucket/

# Download a file
aws s3 cp s3://my-test-bucket/localfile.txt ./
```

5. IAM requirements: the credentials used should have permissions for S3 operations the API needs (e.g., `s3:ListBucket`, `s3:PutObject`, `s3:GetObject`, `s3:DeleteObject`). For production use, follow least-privilege principles and consider using IAM roles.

6. Environment integration: the application uses the AWS SDK default credential/provider chain (environment variables, shared credentials file, or IAM role when running on AWS). No additional app-specific credential config is required when AWS CLI is configured.

## Example `appsettings.json`

You can optionally provide application configuration for a default S3 bucket or to reference a named AWS profile. The AWS SDK will still use the standard credential chain unless you explicitly configure credentials in code.

```json
{
	"S3": {
		"DefaultBucket": "my-app-bucket",
		"Region": "us-east-1"
	},
	"AWS": {
		"Profile": "myprofile"
	}
}
```

- **Note:** Storing long-lived credentials in `appsettings.json` is not recommended. Prefer the AWS shared credentials file (`~/.aws/credentials`), environment variables, or IAM roles.

## Local setup script

A helper script is included at `scripts/setup.sh` to assist with common local setup tasks (configure a profile, create a test bucket, verify connectivity).