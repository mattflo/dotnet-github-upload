using System;
using System.Linq;
using GitHubUploader.Core;
using Xunit;

namespace GitHubUploader.Tests
{
	public class GithubUploaderIntegrationTests
	{
		const string TestRepository = "GitHubUploadDotNet-Test";
		readonly GithubUploader uploader = new GithubUploader("githubuploadtest", "08b2dd7156e5726b0554f9140e01f713");

		[Fact]
		public void Delete_all_files_removes_all_files()
		{
			UploadFile();

			Assert.True(uploader.ListFiles(TestRepository).Count() > 0);

			uploader.DeleteAll(TestRepository);

			Assert.Equal(0, uploader.ListFiles(TestRepository).Count());
		}

		[Fact]
		public void Upload_file_returns_location()
		{
			string location = UploadFile();

			Assert.True(location.StartsWith("http://github.s3.amazonaws.com/"));
		}

		string UploadFile()
		{
			return uploader.Upload(new UploadInfo
			                       	{
			                       		ContentType = "text/plain",
			                       		Data = new byte[] {1, 2, 3, 4},
			                       		Description = "Test",
			                       		Name = "name" + Guid.NewGuid(),
			                       		Repository = TestRepository
			                       	});
		}
	}
}