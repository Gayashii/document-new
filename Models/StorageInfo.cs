namespace DocumentAPI.Models
{
	public class StorageInfo
	{
		public string BasePath { get; set; } = string.Empty;
		public int TotalFiles { get; set; }
		public long TotalSizeBytes { get; set; }
		public double TotalSizeMB { get; set; }
		public List<CategoryInfo> Categories { get; set; } = new();
	}

	public class CategoryInfo
	{
		public string Name { get; set; } = string.Empty;
		public int FileCount { get; set; }
		public long SizeBytes { get; set; }
	}

}
