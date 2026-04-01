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

		// ✅ UPLOAD
		[HttpPost("upload")]
		public async Task<IActionResult> UploadFile(
			[FromForm] IFormFile file,
			[FromForm] string uploadedBy,
			[FromForm] string description)
		{
			try
			{
				var result = await _fileService
					.UploadFileAsync(file, uploadedBy, description);

				return Ok(new
				{
					message = "File uploaded successfully",
					data = result
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Upload failed");
				return BadRequest(new { message = ex.Message });
			}
		}

		// ✅ DOWNLOAD
		[HttpGet("{id}/download")]
		public async Task<IActionResult> Download(int id)
		{
			var doc = await _context.Documents.FindAsync(id);
			if (doc == null) return NotFound();

			var bytes = await _fileService.ReadFileAsync(doc.FilePath);
			return File(bytes, "application/octet-stream", doc.FileName);
		}
	}
}


//using DocumentAPI.Data;
//using DocumentAPI.Models;
//using DocumentAPI.Services;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace DocumentAPI.Controllers
//{
//	[Route("api/[controller]")]
//	[ApiController]
//	public class DocumentController : ControllerBase
//	{
//		private readonly DocumentDbContext _context;
//		private readonly IFileStorageService _fileService;
//		private readonly ILogger<DocumentController> _logger;

//		public DocumentController(<DirectedGraph xmlns="http://schemas.microsoft.com/vs/2009/dgml">
//  <Nodes>
//    <Node Id="(@1 @2)" Visibility="Hidden" />
//    <Node Id="(@3 Namespace=DocumentAPI.Controllers Type=DocumentController)" Category="CodeSchema_Class" CodeSchemaProperty_IsPublic="True" CommonLabel="DocumentController" Icon="Microsoft.VisualStudio.Class.Public" IsDragSource="True" Label="DocumentController" SourceLocation="(Assembly=file:///E:/API/DocumentAPI/DocumentAPI/Controllers/DocumentController.cs StartLineNumber=11 StartCharacterOffset=14 EndLineNumber=11 EndCharacterOffset=32)" />
//  </Nodes>
//  <Links>
//    <Link Source="(@1 @2)" Target="(@3 Namespace=DocumentAPI.Controllers Type=DocumentController)" Category="Contains" />
//  </Links>
//  <Categories>
//    <Category Id="CodeSchema_Class" Label="Class" BasedOn="CodeSchema_Type" Icon="CodeSchema_Class" />
//    <Category Id="CodeSchema_Type" Label="Type" Icon="CodeSchema_Class" />
//    <Category Id="Contains" Label="Contains" Description="Whether the source of the link contains the target object" IsContainment="True" />
//  </Categories>
//  <Properties>
//    <Property Id="CodeSchemaProperty_IsPublic" Label="Is Public" Description="Flag to indicate the scope is Public" DataType="System.Boolean" />
//    <Property Id="CommonLabel" DataType="System.String" />
//    <Property Id="Icon" Label="Icon" DataType="System.String" />
//    <Property Id="IsContainment" DataType="System.Boolean" />
//    <Property Id="IsDragSource" Label="IsDragSource" Description="IsDragSource" DataType="System.Boolean" />
//    <Property Id="Label" Label="Label" Description="Displayable label of an Annotatable object" DataType="System.String" />
//    <Property Id="SourceLocation" Label="Start Line Number" DataType="Microsoft.VisualStudio.GraphModel.CodeSchema.SourceLocation" />
//    <Property Id="Visibility" Label="Visibility" Description="Defines whether a node in the graph is visible or not" DataType="System.Windows.Visibility" />
//  </Properties>
//  <QualifiedNames>
//    <Name Id="Assembly" Label="Assembly" ValueType="Uri" />
//    <Name Id="File" Label="File" ValueType="Uri" />
//    <Name Id="Namespace" Label="Namespace" ValueType="System.String" />
//    <Name Id="Type" Label="Type" ValueType="System.Object" />
//  </QualifiedNames>
//  <IdentifierAliases>
//    <Alias n="1" Uri="Assembly=$(VsSolutionUri)/DocumentAPI/DocumentAPI.csproj" />
//    <Alias n="2" Uri="File=$(VsSolutionUri)/DocumentAPI/Controllers/DocumentController.cs" />
//    <Alias n="3" Uri="Assembly=$(03ff2f31-6b9c-47e2-9488-145ebbcbf060.OutputPathUri)" />
//  </IdentifierAliases>
//  <Paths>
//    <Path Id="03ff2f31-6b9c-47e2-9488-145ebbcbf060.OutputPathUri" Value="file:///E:/API/DocumentAPI/DocumentAPI/bin/Debug/net8.0/DocumentAPI.dll" />
//    <Path Id="VsSolutionUri" Value="file:///E:/API/DocumentAPI" />
//  </Paths>
//</DirectedGraph>
//			DocumentDbContext context,
//			IFileStorageService fileService,
//			ILogger<DocumentController> logger)
//		{
//			_context = context;
//			_fileService = fileService;
//			_logger = logger;
//		}

//		// GET: api/documents
//		//[HttpGet]
//		//public async Task<ActionResult<IEnumerable<Documents>>> GetDocuments()
//		//{
//		//	return await _context.Documents.ToListAsync();
//		//}

//		// GET: api/documents/5
//		[HttpGet("{id}")]
//		public async Task<ActionResult<Documents>> GetDocument(int id)
//		{
//			var document = await _context.Documents.FindAsync(id);

//			if (document == null)
//				return NotFound();

//			return document;
//		}



//		// POST: api/documents
//		[HttpPost]
//		public async Task<ActionResult<Documents>> CreateDocument(DocumentCreateDto dto)
//		{
//			if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.FilePath))
//				return BadRequest("Title and FilePath are required");

//			var directory = Path.GetDirectoryName(dto.FilePath);
//			if (!string.IsNullOrEmpty(directory))
//			{
//				_fileService.EnsureDirectoryExists(directory);
//			}

//			var document = new Documents
//			{
//				Title = dto.Title,
//				Description = dto.Description,
//				FilePath = dto.FilePath,
//				FileName = Path.GetFileName(dto.FilePath),
//				FileSize = dto.FileSize,
//				FileType = dto.FileType,
//				Category = dto.Category,
//				UploadedBy = dto.UploadedBy,
//				CreatedDate = DateTime.UtcNow,
//				ModifiedDate = DateTime.UtcNow,
//				Status = "Active"
//			};

//			_context.Documents.Add(document);
//			await _context.SaveChangesAsync();

//			return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, document);
//		}

//		// GET: api/documents/5/download
//		[HttpGet("{id}/download")]
//		public async Task<IActionResult> DownloadDocument(int id)
//		{
//			var document = await _context.Documents.FindAsync(id);
//			if (document == null)
//				return NotFound("Document not found");

//			if (!_fileService.FileExists(document.FilePath))
//				return NotFound("File not found on server");

//			try
//			{
//				var fileBytes = await _fileService.ReadFileAsync(document.FilePath);
//				return File(fileBytes, "application/octet-stream", document.FileName);
//			}
//			catch (Exception ex)
//			{
//				_logger.LogError(ex, "Error downloading document");
//				return StatusCode(500, "Error downloading document");
//			}
//		}

//		// PUT: api/documents/5
//		[HttpPut("{id}")]
//		public async Task<IActionResult> UpdateDocument(int id, DocumentUpdateDto dto)
//		{
//			var document = await _context.Documents.FindAsync(id);
//			if (document == null)
//				return NotFound();

//			document.Title = dto.Title ?? document.Title;
//			document.Description = dto.Description ?? document.Description;
//			document.Status = dto.Status ?? document.Status;
//			document.Category = dto.Category ?? document.Category;
//			document.ModifiedDate = DateTime.UtcNow;

//			await _context.SaveChangesAsync();
//			return Ok(document);
//		}

//		// DELETE: api/documents/5
//		[HttpDelete("{id}")]
//		public async Task<IActionResult> DeleteDocument(int id, [FromQuery] bool deleteFile = false)
//		{
//			var document = await _context.Documents.FindAsync(id);
//			if (document == null)
//				return NotFound();

//			if (deleteFile)
//			{
//				_fileService.DeleteFile(document.FilePath);
//			}

//			_context.Documents.Remove(document);
//			await _context.SaveChangesAsync();

//			return NoContent();
//		}

//		// GET: api/documents/search/{query}
//		[HttpGet("search/{query}")]
//		public async Task<ActionResult<IEnumerable<Documents>>> SearchDocuments(string query)
//		{
//			var documents = await _context.Documents
//				.Where(d => d.Title.Contains(query) ||
//						   d.Description.Contains(query) ||
//						   d.Category.Contains(query))
//				.ToListAsync();

//			return documents;
//		}

//		// GET: api/documents/category/{category}
//		[HttpGet("category/{category}")]
//		public async Task<ActionResult<IEnumerable<Documents>>> GetDocumentsByCategory(string category)
//		{
//			var documents = await _context.Documents
//				.Where(d => d.Category == category)
//				.ToListAsync();

//			return documents;
//		}

//		// GET: api/documents/storage/info
//		[HttpGet("storage/info")]
//		public ActionResult<StorageInfo> GetStorageInfo()
//		{
//			return _fileService.GetStorageInfo();
//		}

//		//upload file
//		[HttpPost("upload")]
//		public async Task<IActionResult> UploadFile([FromForm] IFormFile file,	[FromForm] string uploadedBy,	[FromForm] string description)
//		{
//			try
//			{
//				var result = await _fileService.UploadFileAsync(file, uploadedBy, description);

//				return Ok(new
//				{
//					message = "File uploaded successfully",
//					data = result
//				});
//			}
//			catch (Exception ex)
//			{
//				return BadRequest(new { message = ex.Message });
//			}
//		}
//	}
//}

