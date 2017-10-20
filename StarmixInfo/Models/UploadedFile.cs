using System;
namespace StarmixInfo.Models
{
    public struct UploadedFile
    {
        public UploadedFile(string name, DateTime uploaded, string uploadFolder)
        {
            FileName = name;
            UploadDate = uploaded;

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Path += uploadFolder;
            uriBuilder.Path += "/" + FileName;
            UriPath = uriBuilder.Path;
        }

        public string FileName { get; }
        public DateTime UploadDate { get; }
        public string UriPath { get; }
    }
}
