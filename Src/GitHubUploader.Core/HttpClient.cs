using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace GitHubUploader.Core
{
	public static class HttpClient
	{
		public static string Post(string url, IDictionary<string, object> formData)
		{
			var request = (HttpWebRequest) WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			var builder = new StringBuilder();
			foreach (var pair in formData)
			{
				if (builder.Length > 0) builder.Append("&");
				builder.AppendFormat("{0}={1}", pair.Key, HttpUtility.UrlEncode((pair.Value ?? "").ToString()));
			}

			using (Stream requestRream = request.GetRequestStream())
			{
				byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());
				requestRream.Write(bytes, 0, bytes.Length);
			}

			using (WebResponse response = request.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					using (var responseReader = new StreamReader(responseStream))
					{
						return responseReader.ReadToEnd();
					}
				}
			}
		}

		public static ResponseDetails PostWithFile(string url, string fileName, Func<Stream> contents, string paramName, string contentType, IDictionary<string, object> formData)
		{
			string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
			byte[] boundaryAsBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

			var request = (HttpWebRequest) WebRequest.Create(url);
			request.ContentType = "multipart/form-data; boundary=" + boundary;
			request.Method = "POST";
			request.KeepAlive = true;

			Stream requestStream = request.GetRequestStream();

			WriteFormData(requestStream, formData, boundaryAsBytes);

			WriteFileData(requestStream, paramName, fileName, contentType, contents, boundaryAsBytes);

			WriteTrailer(requestStream, boundary);

			return ParseResponse(request);
		}

		static void WriteTrailer(Stream requestStream, string boundary)
		{
			byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
			requestStream.Write(trailer, 0, trailer.Length);
			requestStream.Close();
		}

		static void WriteFileData(Stream requestStream, string paramName, string fileName, string contentType, Func<Stream> contents, byte[] boundaryAsBytes)
		{
			requestStream.Write(boundaryAsBytes, 0, boundaryAsBytes.Length);

			const string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
			string header = string.Format(headerTemplate, paramName, fileName, contentType);
			byte[] headerbytes = Encoding.UTF8.GetBytes(header);
			requestStream.Write(headerbytes, 0, headerbytes.Length);

			using (Stream fileStream = contents())
			{
				var buffer = new byte[4096];
				int bytesRead;
				while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
				{
					requestStream.Write(buffer, 0, bytesRead);
				}
				fileStream.Close();
			}
		}

		static void WriteFormData(Stream requestStream, IDictionary<string, object> formData, byte[] boundaryAsBytes)
		{
			const string formDataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

			foreach (string key in formData.Keys)
			{
				requestStream.Write(boundaryAsBytes, 0, boundaryAsBytes.Length);
				string formItem = string.Format(formDataTemplate, key, formData[key]);
				byte[] formItemAsBytes = Encoding.UTF8.GetBytes(formItem);
				requestStream.Write(formItemAsBytes, 0, formItemAsBytes.Length);
			}
		}

		static ResponseDetails ParseResponse(WebRequest request)
		{
			var response = (HttpWebResponse) request.GetResponse();

			using (Stream responseStream = response.GetResponseStream())
			{
				using (var streamReader = new StreamReader(responseStream))
				{
					return new ResponseDetails
					       	{
					       		Content = streamReader.ReadToEnd(),
					       		Status = response.StatusCode
					       	};
				}
			}
		}

		public static ResponseDetails Get(string url, Dictionary<string, object> dictionary)
		{
			string urlWithParameters = BuildUrl(url, dictionary);

			WebRequest request = WebRequest.Create(urlWithParameters);

			return ParseResponse(request);
		}

		static string BuildUrl(string url, Dictionary<string, object> parameters)
		{
			var urlBuilder = new StringBuilder();

			urlBuilder.Append(url);

			if (!url.EndsWith("?") && parameters.Count > 0)
			{
				urlBuilder.Append("?");
			}

			int count = 0;

			foreach (var pair in parameters)
			{
				if (count > 0) urlBuilder.Append("&");
				urlBuilder.AppendFormat("{0}={1}", HttpUtility.UrlEncode(pair.Key), HttpUtility.UrlEncode(pair.Value.ToString()));
				count++;
			}

			return urlBuilder.ToString();
		}
	}
}