namespace DocumentAPI.Models
{
	public class FileDownloadDto
	{
		public byte[] Content { get; set; } = Array.Empty<byte>();
		public string ContentType { get; set; } = string.Empty;
		public string FileName { get; set; } = string.Empty;
	}
}
