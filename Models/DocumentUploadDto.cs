namespace DocumentAPI.Models
{
	public class DocumentUploadDto
	{
		public IFormFile File { get; set; }
		public string UploadedBy { get; set; }
		public string Description { get; set; }
	}
}
