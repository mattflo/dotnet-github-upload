using GitHubUploader.Core;
using Xunit;

namespace GitHubUploader.Tests
{
	public class S3UploaderTests
	{
		readonly S3Uploader uploader = new S3Uploader();

		[Fact]
		public void Parse_s3_response_for_location()
		{
			string xml =
				@"<?xml version=""1.0"" encoding=""UTF-8""?>
<PostResponse>
<Location>http://github.s3.amazonaws.com/downloads%2Fbittercoder%2FLob%2Fname38a38892-7704-4255-83c8-455999f5d950</Location>
<Bucket>github</Bucket>
<Key>downloads/bittercoder/Lob/name38a38892-7704-4255-83c8-455999f5d950</Key>
<ETag>""08d6c05a21512a79a1dfeb9d2a8f262f""</ETag>
</PostResponse>";

			Assert.Equal("http://github.s3.amazonaws.com/downloads%2Fbittercoder%2FLob%2Fname38a38892-7704-4255-83c8-455999f5d950", uploader.ParseS3ResponseForLocation(xml));
		}

		[Fact]
		public void Parse_s3_response_for_error()
		{
			string xml =
				@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Error>
  <Code>NoSuchKey</Code>
  <Message>The resource you requested does not exist</Message>
  <Resource>/mybucket/myfoto.jpg</Resource> 
  <RequestId>4442587FB7D0A2F9</RequestId>
</Error>";

			Assert.Equal(" due to NoSuchKey (The resource you requested does not exist)", uploader.ExtractS3ErrorMessage(xml));
		}
	}
}