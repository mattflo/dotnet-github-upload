using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace GitHubUploader.Core
{
	public class S3Uploader : IS3Uploader
	{
		public string Upload(Func<Stream> streamFunc, string url, string fileName, string policy, string key, string awsAccessKeyId, string contentType, string signature, string acl)
		{
			ResponseDetails uploadTos3Response = HttpClient.PostWithFile(url, fileName, streamFunc, "file", contentType, new Dictionary<string, object>
			                                                                                                             	{
			                                                                                                             		{"Filename", fileName},
			                                                                                                             		{"policy", policy},
			                                                                                                             		{"success_action_status", 201},
			                                                                                                             		{"key", key},
			                                                                                                             		{"AWSAccessKeyId",awsAccessKeyId},
			                                                                                                             		{"Content-Type",contentType ??"application/octet-stream"},
			                                                                                                             		{"signature",signature},
			                                                                                                             		{"acl", acl}
			                                                                                                             	});
			if (uploadTos3Response.Status != HttpStatusCode.Created)
			{
				throw new Exception("Failed to upload file" + ExtractS3ErrorMessage(uploadTos3Response.Content));
			}

			return ParseS3ResponseForLocation(uploadTos3Response.Content);
		}

		public string ParseS3ResponseForLocation(string content)
		{
			XDocument doc = XDocument.Parse(content);
			return doc.Root.Element("Location").Value;
		}

		public string ExtractS3ErrorMessage(string xml)
		{
			try
			{
				// see http://docs.amazonwebservices.com/AmazonS3/latest/dev/index.html?UsingRESTError.html
				XDocument doc = XDocument.Parse(xml);
				XElement error = doc.Root;
				return string.Format(" due to {0} ({1})", error.Element("Code").Value, error.Element("Message").Value);
			}
			catch
			{
				return "";
			}
		}
	}
}