using DocumentAPI.Models;

namespace DocumentAPI.Services
{
	//public class IFileStorageService
	//{
		public interface IFileStorageService
		{
			void EnsureDirectoryExists(string path);
			Task<string> SaveFileAsync(IFormFile file, string category = "General");
			Task<Documents> UploadFileAsync(IFormFile file, string uploadedBy, string description);
			bool FileExists(string path);
			Task<byte[]> ReadFileAsync(string path);
			bool DeleteFile(string path);
			StorageInfo GetStorageInfo();
		}
	//}
}
