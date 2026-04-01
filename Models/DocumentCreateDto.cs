namespace DocumentAPI.Models
{
	public class DocumentCreateDto
	{
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string FilePath { get; set; } = string.Empty;
		public long? FileSize { get; set; }
		public string FileType { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public string UploadedBy { get; set; } = string.Empty;
	}
}
