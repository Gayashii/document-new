namespace DocumentAPI.Models
{
	public class Documents
	{
		//public class Document
		//{
			public int Id { get; set; }
			public string Title { get; set; } = string.Empty;
			public string Description { get; set; } = string.Empty;
			public string FilePath { get; set; } = string.Empty;
			public string FileName { get; set; } = string.Empty;
			public long? FileSize { get; set; }
			public string FileType { get; set; } = string.Empty;
			public string Category { get; set; } = string.Empty;
			public string UploadedBy { get; set; } = string.Empty;
			public DateTime CreatedDate { get; set; }
			public DateTime ModifiedDate { get; set; }
			public string Status { get; set; } = "Active";
		//}
	}
}

