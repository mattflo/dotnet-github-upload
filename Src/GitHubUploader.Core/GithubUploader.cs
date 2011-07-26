using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace GitHubUploader.Core
{
	public class GithubUploader
	{
		readonly string _login;
		readonly IS3Uploader _s3Uploader;
		readonly string _token;

		public GithubUploader(string login, string token)
			: this(login, token, new S3Uploader())
		{
		}

		public GithubUploader(string login, string token, IS3Uploader s3Uploader)
		{
			if (s3Uploader == null) throw new ArgumentNullException("s3Uploader");
			if (string.IsNullOrEmpty(login)) throw new ArgumentNullException("login");
			if (string.IsNullOrEmpty(token)) throw new ArgumentNullException("token");
			_login = login;
			_token = token;
			_s3Uploader = s3Uploader;
		}

		public IEnumerable<string> ListFiles(string repository)
		{
			repository = QualifyRepositoryName(repository);

			if (string.IsNullOrEmpty(repository)) throw new ArgumentNullException("repository");

			ResponseDetails response = HttpClient.Get(string.Format("https://github.com/{0}/downloads", repository), new Dictionary<string, object> {{"login", _login}, {"token", _token}});

			return ParseHtmlForFiles(response.Content);
		}

		public IEnumerable<string> ParseHtmlForFiles(string html)
		{
			var doc = new HtmlDocument();

			doc.LoadHtml(html);

			var manualDownloadsEl = doc.GetElementbyId("manual_downloads");

			if (manualDownloadsEl == null) yield break;
			
			IEnumerable<HtmlNode> lis =manualDownloadsEl.ChildNodes.Where(n => n.Name.Equals("li", StringComparison.OrdinalIgnoreCase));

			var idRegex = new Regex("\\d+$");

			foreach (HtmlNode li in lis)
			{
				HtmlNode descriptionNode = li.SelectNodes("descendant::h4").First();

			    var name = descriptionNode.ChildNodes.Where(n => n.Name == "a").Select(n => n.InnerText).First();

                //var strings = enumerable.ToArray();

                //var @join = string.Join("", strings);

                //string description = @join.Replace("—", "").Trim();

                //string date = li.SelectNodes("descendant::p/abbr").First().Attributes["title"].Value;
                //string size = li.SelectNodes("descendant::p/strong").First().InnerText;
                //string id = idRegex.Match(li.SelectNodes("a").First().Attributes["href"].Value).Value;

                //HtmlNode anchor = li.SelectNodes("descendant::h4/a").First();

                //string link = anchor.Attributes["href"].Value;
                //string name = anchor.InnerText;

			    yield return name;
			}
		}

		public string Upload(UploadInfo info)
		{
			if (string.IsNullOrEmpty(info.Repository))
			{
				throw new ArgumentException("Repository name is required", "info");
			}

			info.Repository = QualifyRepositoryName(info.Repository);

			if (info.FileName != null)
			{
				if (!File.Exists(info.FileName))
				{
					throw new ArgumentException(string.Format("No file with the name: {0} exists.", info.FileName), "info");
				}
			}

			info.Name = info.Name ?? Path.GetFileName(info.FileName);

			if (info.FileName == null && info.Data == null)
			{
				throw new ArgumentException("Either FileName or Data must be present in the UploadInfo", "info");
			}

			if (info.Data != null && string.IsNullOrEmpty(info.Name))
			{
				throw new ArgumentException("Name must be specified if Data supplied", "info");
			}

			if (info.Replace)
			{
                //IEnumerable<FileInfo> filesToDelete = ListFiles(info.Repository).Where(f => f.Name.Equals(info.Name));

                //foreach (FileInfo file in filesToDelete)
                //{
                //    Delete(info.Repository, file.Id);
                //}
			}
			else
			{
				var files = ListFiles(info.Repository);

				if (files.Any(name => name.EndsWith(info.Name)))
				{
					throw new InvalidOperationException(string.Format("The file {0} is already uploaded. please try a different name", info.Name));
				}
			}

			string url = string.Format("https://github.com/{0}/downloads", info.Repository);

			long fileLength = info.Data != null ? info.Data.Length : new System.IO.FileInfo(info.FileName).Length;

			string response = HttpClient.Post(url, new Dictionary<string, object>
			                                       	{
			                                       		{"file_size", fileLength},
			                                       		{"content_type", info.ContentType},
			                                       		{"file_name", info.Name},
			                                       		{"description", info.Description},
			                                       		{"login", _login},
			                                       		{"token", _token}
			                                       	});

			var uploadResponse = (GithubUploadResponse) JsonConvert.DeserializeObject(response, typeof (GithubUploadResponse));

			Func<Stream> streamFunc = (info.Data != null) ? new Func<Stream>(() => new MemoryStream(info.Data)) : () => File.OpenRead(info.FileName);

			return UploadToS3(info, streamFunc, uploadResponse);
		}

		string UploadToS3(UploadInfo info, Func<Stream> streamFunc, GithubUploadResponse uploadResponse)
		{
			return _s3Uploader.Upload(streamFunc,
			                          "http://github.s3.amazonaws.com/",
			                          info.Name,
			                          uploadResponse.Policy,
			                          uploadResponse.Path,
			                          uploadResponse.AccessKeyId,
			                          uploadResponse.ContentType,
			                          uploadResponse.Signature,
			                          uploadResponse.Acl);
		}

		string QualifyRepositoryName(string repository)
		{
			return !repository.Contains("/") ? _login + "/" + repository : repository;
		}

		public void Delete(string repository, string id)
		{
			repository = QualifyRepositoryName(repository);

			id = id.Replace("download_", "");

			string url = string.Format("https://github.com/{0}/downloads/{1}", repository, id);

			HttpClient.Post(url, new Dictionary<string, object> {{"_method", "delete"}, {"login", _login}, {"token", _token}});
		}

		public void DeleteAll(string repository)
		{
			repository = QualifyRepositoryName(repository);

			if (string.IsNullOrEmpty(repository)) throw new ArgumentNullException("repository");

            //foreach (FileInfo file in ListFiles(repository))
            //{
            //    Delete(repository, file.Id);
            //}
		}
	}
}