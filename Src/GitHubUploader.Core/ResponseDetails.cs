using System.Net;

namespace GitHubUploader.Core
{
	public class ResponseDetails
	{
		public string Content { get; set; }
		public HttpStatusCode Status { get; set; }
	}
}