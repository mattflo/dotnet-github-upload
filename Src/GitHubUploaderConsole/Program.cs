using GitHubUploader.Core;

namespace GitHubUploaderConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var login = args[0];

            var token = args[1];

            var uploader = new GithubUploader(login, token);

            var info = new UploadInfo();

            info.FileName = args[2];

            info.Name = args[2];

            info.Repository = "nspec";

            uploader.Upload(info);
        }
    }
}
