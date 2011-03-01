using System;
using System.IO;

namespace GitHubUploader.Core
{
	public interface IS3Uploader
	{
		string Upload(Func<Stream> streamFunc, string url, string fileName, string policy, string key, string awsAccessKeyId, string contentType, string signature, string acl);
	}
}