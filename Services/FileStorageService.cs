
using DocumentAPI.Data;
using DocumentAPI.Models;
using DocumentAPI.Services;

namespace Document.Services
{
	public class FileStorageService : IFileStorageService
	{
		private readonly string _baseStoragePath;
		private readonly ILogger<FileStorageService> _logger;
		private readonly DocumentDbContext _context;

		public FileStorageService(
			ILogger<FileStorageService> logger,
			DocumentDbContext context)
		{
			_logger = logger;
			_context = context;

			_baseStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "DocumentStorage");
			EnsureDirectoryExists(_baseStoragePath);
		}

		public void EnsureDirectoryExists(string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

		public async Task<string> SaveFileAsync(IFormFile file, string category = "General")
		{
			var categoryPath = Path.Combine(_baseStoragePath, category);
			EnsureDirectoryExists(categoryPath);

			var datePath = Path.Combine(categoryPath, DateTime.UtcNow.ToString("yyyy-MM"));
			EnsureDirectoryExists(datePath);

			var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
			var fullPath = Path.Combine(datePath, uniqueFileName);

			using (var stream = new FileStream(fullPath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			return fullPath;
		}

		// ✅ MAIN METHOD
		public async Task<Documents> UploadFileAsync(IFormFile file, string uploadedBy, string description)
		{
			if (file == null || file.Length == 0)
				throw new Exception("Invalid file");

			var savedPath = await SaveFileAsync(file);

			var document = new Documents
			{
				Title = file.FileName,
				Description = description,
				FileName = file.FileName,
				FilePath = savedPath,
				FileSize = file.Length,
				FileType = Path.GetExtension(file.FileName),
				Category = "General",
				UploadedBy = uploadedBy,
				CreatedDate = DateTime.UtcNow,
				Status = "Active"
			};

			_context.Documents.Add(document);
			await _context.SaveChangesAsync();

			return document;
		}

		public bool FileExists(string path) => File.Exists(path);

		public async Task<byte[]> ReadFileAsync(string path)
		{
			if (!FileExists(path))
				throw new FileNotFoundException();

			return await File.ReadAllBytesAsync(path);
		}

		public bool DeleteFile(string path)
		{
			if (FileExists(path))
			{
				File.Delete(path);
				return true;
			}
			return false;
		}

		public StorageInfo GetStorageInfo()
		{
			var storageInfo = new StorageInfo
			{
				BasePath = _baseStoragePath,
				Categories = new List<CategoryInfo>()
			};

			if (!Directory.Exists(_baseStoragePath))
				return storageInfo;

			try
			{
				var allFiles = Directory.GetFiles(_baseStoragePath, "*.*", SearchOption.AllDirectories);
				storageInfo.TotalFiles = allFiles.Length;
				storageInfo.TotalSizeBytes = allFiles.Sum(f => new FileInfo(f).Length);
				storageInfo.TotalSizeMB = Math.Round(storageInfo.TotalSizeBytes / 1024.0 / 1024.0, 2);

				var categories = Directory.GetDirectories(_baseStoragePath);
				foreach (var category in categories)
				{
					var categoryFiles = Directory.GetFiles(category, "*.*", SearchOption.AllDirectories);
					storageInfo.Categories.Add(new CategoryInfo
					{
						Name = Path.GetFileName(category),
						FileCount = categoryFiles.Length,
						SizeBytes = categoryFiles.Sum(f => new FileInfo(f).Length)
					});
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting storage info");
			}

			return storageInfo;
		}
	}
}











//using DocumentAPI.Data;
//using DocumentAPI.Models;
//using DocumentAPI.Services;
//using Microsoft.EntityFrameworkCore;

//namespace Document.Services
//{
//	public class FileStorageService: IFileStorageService
//	{

//			private readonly string _baseStoragePath;
//			private readonly ILogger<FileStorageService> _logger;
//			private readonly DocumentDbContext _context;

//		public FileStorageService(ILogger<FileStorageService> logger)
//			{
//				_logger = logger;
//				_baseStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "DocumentStorage");
//				EnsureDirectoryExists(_baseStoragePath);
//				_logger.LogInformation($"Storage initialized at: {_baseStoragePath}");
//			}

//			public void EnsureDirectoryExists(string path)
//			{
//				if (!Directory.Exists(path))
//				{
//					Directory.CreateDirectory(path);
//					_logger.LogInformation($"Created directory: {path}");
//				}
//			}

//			public async Task<string> SaveFileAsync(IFormFile file, string category = "General")
//			{
//				try
//				{
//					// Create category folder
//					var categoryPath = Path.Combine(_baseStoragePath, SanitizeFolderName(category));
//					EnsureDirectoryExists(categoryPath);

//					// Create date-based subfolder (YYYY-MM)
//					var datePath = Path.Combine(categoryPath, DateTime.UtcNow.ToString("yyyy-MM"));
//					EnsureDirectoryExists(datePath);

//					// Generate unique filename
//					var uniqueFileName = $"{Guid.NewGuid()}_{SanitizeFileName(file.FileName)}";
//					var fullPath = Path.Combine(datePath, uniqueFileName);

//					// Save file
//					using (var stream = new FileStream(fullPath, FileMode.Create))
//					{
//						await file.CopyToAsync(stream);
//					}

//					_logger.LogInformation($"File saved: {fullPath}");
//					return fullPath;
//				}
//				catch (Exception ex)
//				{
//					_logger.LogError(ex, "Error saving file");
//					throw;
//				}
//			}

//			public bool FileExists(string path)
//			{
//				return File.Exists(path);
//			}

//			public async Task<byte[]> ReadFileAsync(string path)
//			{
//				if (!FileExists(path))
//					throw new FileNotFoundException("File not found", path);

//				return await File.ReadAllBytesAsync(path);
//			}

//			public bool DeleteFile(string path)
//			{
//				try
//				{
//					if (FileExists(path))
//					{
//						File.Delete(path);
//						_logger.LogInformation($"File deleted: {path}");
//						return true;
//					}
//					return false;
//				}
//				catch (Exception ex)
//				{
//					_logger.LogError(ex, $"Error deleting file: {path}");
//					return false;
//				}
//			}

//			public StorageInfo GetStorageInfo()
//			{
//				var storageInfo = new StorageInfo
//				{
//					BasePath = _baseStoragePath
//				};

//				if (!Directory.Exists(_baseStoragePath))
//					return storageInfo;

//				var allFiles = Directory.GetFiles(_baseStoragePath, "*.*", SearchOption.AllDirectories);
//				storageInfo.TotalFiles = allFiles.Length;
//				storageInfo.TotalSizeBytes = allFiles.Sum(f => new FileInfo(f).Length);
//				storageInfo.TotalSizeMB = Math.Round(storageInfo.TotalSizeBytes / 1024.0 / 1024.0, 2);

//				// Get category information
//				var categories = Directory.GetDirectories(_baseStoragePath);
//				foreach (var category in categories)
//				{
//					var categoryFiles = Directory.GetFiles(category, "*.*", SearchOption.AllDirectories);
//					storageInfo.Categories.Add(new CategoryInfo
//					{
//						Name = Path.GetFileName(category),
//						FileCount = categoryFiles.Length,
//						SizeBytes = categoryFiles.Sum(f => new FileInfo(f).Length)
//					});
//				}

//				return storageInfo;
//			}

//			private string SanitizeFileName(string fileName)
//			{
//				var invalidChars = Path.GetInvalidFileNameChars();
//				return string.Join("_", fileName.Split(invalidChars));
//			}

//			private string SanitizeFolderName(string folderName)
//			{
//				if (string.IsNullOrWhiteSpace(folderName))
//					return "General";

//				var invalidChars = Path.GetInvalidPathChars();
//				return string.Join("_", folderName.Split(invalidChars));
//			}

//		public async Task<Documents> UploadFileAsync(IFormFile file, string uploadedBy, string description)
//		{
//			if (file == null || file.Length == 0)
//				throw new Exception("Invalid file");

//			// ✅ Use your existing method (VERY IMPORTANT)
//			var savedPath = await SaveFileAsync(file, "General");

//			// Extract file type
//			var fileType = Path.GetExtension(file.FileName);

//			// 🗄 Save to DB
//			var document = new Documents
//			{
//				Title = file.FileName,
//				Description = description,
//				FileName = file.FileName,
//				FilePath = savedPath, // full path from your storage system
//				FileSize = file.Length,
//				FileType = fileType,
//				Category = "General",
//				UploadedBy = uploadedBy,
//				CreatedDate = DateTime.Now,
//				Status = "Active"
//			};

//			_context.Documents.Add(document);
//			await _context.SaveChangesAsync();

//			return document;
//		}
//	}
//	}


