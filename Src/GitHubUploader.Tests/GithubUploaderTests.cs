using System;
using System.IO;
using System.Linq;
using GitHubUploader.Core;
using Xunit;

namespace GitHubUploader.Tests
{
	public class GithubUploaderTests
	{
		GithubUploader uploader = new GithubUploader("login", "token");

		[Fact]
		public void Parse_html_for_files_returns_correct_number_of_files()
		{
			var files = uploader.ParseHtmlForFiles(File.ReadAllText("SampleFileList.html")).ToList();

			Assert.Equal(2, files.Count);
		}

		[Fact]
		public void Parse_html_for_files_when_no_files_returns_correct_number_of_files()
		{
			var files = uploader.ParseHtmlForFiles(File.ReadAllText("EmptyFileList.html")).ToList();

			Assert.Equal(0, files.Count);
		}

        //[Fact]
        //public void Parse_html_for_files_returns_correct_file_details()
        //{
        //    var file = uploader.ParseHtmlForFiles(File.ReadAllText("SampleFileList.html")).First();

        //    Assert.Equal("Crystals.JPG", file.Name);
        //    Assert.Equal(new DateTime(2011, 02, 28, 14, 29, 59), file.Date);
        //    Assert.Equal("test2 - some crystals", file.Description);
        //    Assert.Equal("81487", file.Id);
        //    Assert.Equal("/downloads/bittercoder/Lob/Crystals.JPG", file.Link);
        //    Assert.Equal("87KB", file.Size);
        //}
	}
}