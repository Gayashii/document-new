using DocumentAPI.Data;
using DocumentAPI.Models;
using DocumentAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocumentAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentController : ControllerBase
	{
		private readonly DocumentDbContext _context;
		private readonly IFileStorageService _fileService;
		private readonly ILogger<DocumentController> _logger;

		public DocumentController(
			DocumentDbContext context,
			IFileStorageService fileService,
			ILogger<DocumentController> logger)
		{
			_context = context;
			_fileService = fileService;
			_logger = logger;
		}

		// GET: api/Document
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Documents>>> GetDocuments()
		{
			return await _context.Documents.ToListAsync();
		}

		// GET: api/Document/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Documents>> GetDocument(int id)
		{
			var document = await _context.Documents.FindAsync(id);
			if (document == null)
				return NotFound();

			return document;
		}

		// GET: api/Document/stats/total
		[HttpGet("stats/total")]
		public async Task<IActionResult> GetTotalCount()
		{
			var total = await _context.Documents.CountAsync();
			return Ok(new { total });
		}

		// GET: api/Document/stats/uploads-overview
		[HttpGet("stats/uploads-overview")]
		public async Task<IActionResult> GetUploadsOverview()
		{
			var uploads = await _context.Documents
				.GroupBy(d => d.CreatedDate.Date)
				.Select(g => new { date = g.Key, count = g.Count() })
				.OrderBy(x => x.date)
				.ToListAsync();

			return Ok(uploads);
		}

		// GET: api/Document/stats/file-types
		[HttpGet("stats/file-types")]
		public async Task<IActionResult> GetFileTypes()
		{
			var fileTypes = await _context.Documents
				.GroupBy(d => d.FileType)
				.Select(g => new { type = g.Key, count = g.Count() })
				.OrderByDescending(x => x.count)
				.ToListAsync();

			return Ok(fileTypes);
		}

		// GET: api/Document/search/{query}
		[HttpGet("search/{query}")]
		public async Task<ActionResult<IEnumerable<Documents>>> SearchDocuments(string query)
		{
			var documents = await _context.Documents
				.Where(d => d.Title.Contains(query) ||
						   d.Description.Contains(query) ||
						   d.Category.Contains(query))
				.ToListAsync();

			return documents;
		}

		// GET: api/Document/category/{category}
		[HttpGet("category/{category}")]
		public async Task<ActionResult<IEnumerable<Documents>>> GetDocumentsByCategory(string category)
		{
			var documents = await _context.Documents
				.Where(d => d.Category == category)
				.ToListAsync();

			return documents;
		}

		// GET: api/Document/storage/info
		[HttpGet("storage/info")]
		public ActionResult<StorageInfo> GetStorageInfo()
		{
			return _fileService.GetStorageInfo();
		}

		// POST: api/Document/upload
		[HttpPost("upload")]
		public async Task<IActionResult> UploadFile(
			[FromForm] IFormFile file,
			[FromForm] string uploadedBy,
			[FromForm] string description)
		{
			try
			{
				var result = await _fileService.UploadFileAsync(file, uploadedBy, description);
				return Ok(new { message = "File uploaded successfully", data = result });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Upload failed");
				return BadRequest(new { message = ex.Message });
			}
		}

		// POST: api/Document
		[HttpPost]
		public async Task<ActionResult<Documents>> CreateDocument(DocumentCreateDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.FilePath))
				return BadRequest("Title and FilePath are required");

			var directory = Path.GetDirectoryName(dto.FilePath);
			if (!string.IsNullOrEmpty(directory))
				_fileService.EnsureDirectoryExists(directory);

			var document = new Documents
			{
				Title = dto.Title,
				Description = dto.Description,
				FilePath = dto.FilePath,
				FileName = Path.GetFileName(dto.FilePath),
				FileSize = dto.FileSize,
				FileType = dto.FileType,
				Category = dto.Category,
				UploadedBy = dto.UploadedBy,
				CreatedDate = DateTime.UtcNow,
				ModifiedDate = DateTime.UtcNow,
				Status = "Active"
			};

			_context.Documents.Add(document);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, document);
		}

		// PUT: api/Document/5
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateDocument(int id, DocumentUpdateDto dto)
		{
			var document = await _context.Documents.FindAsync(id);
			if (document == null)
				return NotFound();

			document.Title = dto.Title ?? document.Title;
			document.Description = dto.Description ?? document.Description;
			document.Status = dto.Status ?? document.Status;
			document.Category = dto.Category ?? document.Category;
			document.ModifiedDate = DateTime.UtcNow;

			await _context.SaveChangesAsync();
			return Ok(document);
		}

		// DELETE: api/Document/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteDocument(int id, [FromQuery] bool deleteFile = false)
		{
			var document = await _context.Documents.FindAsync(id);
			if (document == null)
				return NotFound();

			if (deleteFile)
				_fileService.DeleteFile(document.FilePath);

			_context.Documents.Remove(document);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// GET: api/Document/{id}/download
		[HttpGet("{id}/download")]
		public async Task<IActionResult> Download(int id)
		{
			var document = await _context.Documents.FindAsync(id);
			if (document == null)
				return NotFound("Document not found");

			if (!_fileService.FileExists(document.FilePath))
				return NotFound("File not found on server");

			try
			{
				var bytes = await _fileService.ReadFileAsync(document.FilePath);
				return File(bytes, "application/octet-stream", document.FileName);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error downloading document");
				return StatusCode(500, "Error downloading document");
			}
		}
	}
}
