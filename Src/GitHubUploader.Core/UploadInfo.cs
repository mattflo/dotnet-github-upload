namespace GitHubUploader.Core
{
	public class UploadInfo
	{
		public string Repository { get; set; }
		public byte[] Data { get; set; }
		public string Name { get; set; }
		public string FileName { get; set; }
		public bool Replace { get; set; }
		public string ContentType { get; set; }
		public string Description { get; set; }
	}
}