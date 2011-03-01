using System;

namespace GitHubUploader.Core
{
	public class FileInfo
	{
		public string Description { get; set; }
		public DateTime Date { get; set; }
		public string Size { get; set; }
		public string Id { get; set; }
		public string Link { get; set; }
		public string Name { get; set; }
	}
}