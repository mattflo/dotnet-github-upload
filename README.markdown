Introduction
------------

This project is a port of the Ruby net::github:upload library to the .Net Framework.

It allows for uploading of files to the "Downloads" section of a github project.

Why would I want this?
----------------------

If you want to automate the uploading of files to the "Download" section of a github project, from say a PowerShell script.

What do I need to use this
--------------------------

This library makes use of the HTML Agility Pack and the Newtonsoft JSON.Net library.

Quick Examples
--------------

**Uploading**

	var uploader = new GithubUploader("github-login", "gihub-secret-token");
		
	string fileLocation = uploader.Upload(new UploadInfo
			                       	{
			                       		ContentType = "text/plain",
			                       		Data = new byte[] {1, 2, 3, 4}, // alternatively, set the Property "FileName" with a path to a file.
			                       		Description = "Test",
			                       		Name = "name",
			                       		Repository = "RepositoryXYZ"
			                       	});
									
	// file location will be in the format:  http://github.s3.amazonaws.com/downloads%2Fgithubuploadtest%2FGitHubUploadDotNet-Test%2Fname29ad9d6e-9fac-4a76-84e3-cae384c97ed9
	Console.WriteLine(fileLocation);  
	
**Listing Files**

	var uploader = new GithubUploader("github-login", "gihub-secret-token");
		
	var files = uploader.ListFiles("RepositoryXYZ");
	
	foreach (var file in files)
	{
		Console.WriteLine("File: {0}, Id: {1}, Size: {2}, Link: {3}", file.Name, file.Id, file.Size, file.Link);
	}
	
**Deleting Files**

	var uploader = new GithubUploader("github-login", "gihub-secret-token");
		
	var files = uploader.ListFiles("RepositoryXYZ");
	
	foreach (var file in files)
	{
		uploader.Delete("RepositoryXYZ", file.Id);
	}
	
	// alternatively, you can also just "DeleteAll"
	
	uploader.DeleteAll("RepositoryXYZ");
	
Project Structure
-----------------

  - **GitHubUploader.Core**  - Core implementation for uploading to Github.
  - **GitHubUploader.Tests** - Test project - includes both unit and integration tests (uses the XUnit.Net test framework).

Additional Resources
--------------------

**The original versions of this library**

  - [net-github-upload-perl][1]
  - [ruby-net-github-upload][2]  

Maintainer
----------

The current maintainer of this Port to .Net is Alex Henderson a.k.a [@bittercoder][3].  

  - [Bittercoder's Blog][3].
  - [Bittercoder's Twitter][4].
  - [Bittercoder's Email][5].

  [1]: https://github.com/Constellation/ruby-net-github-upload
  [2]: https://github.com/typester/net-github-upload-perl
  [3]: http://blog.bittercoder.com/
  [4]: http://twitter.com/bittercoder
  [5]: http://mailto:bittercoder@gmail.com