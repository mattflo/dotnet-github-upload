using Newtonsoft.Json;

namespace GitHubUploader.Core
{
	public class GithubUploadResponse
	{
		[JsonProperty("policy")]
		public string Policy { get; set; }

		[JsonProperty("path")]
		public string Path { get; set; }

		[JsonProperty("accesskeyid")]
		public string AccessKeyId { get; set; }

		[JsonProperty("content_type")]
		public string ContentType { get; set; }

		[JsonProperty("signature")]
		public string Signature { get; set; }

		[JsonProperty("acl")]
		public string Acl { get; set; }
	}
}