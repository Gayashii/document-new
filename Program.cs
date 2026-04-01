
using Document.Services;
using DocumentAPI.Data;
using DocumentAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ✅ DETAILED LOGGING
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// ✅ Controllers with better JSON handling
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
		options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
	});

builder.Services.AddEndpointsApiExplorer();

// ✅ SIMPLIFIED SWAGGER (to isolate the issue)
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
	{
		Title = "Document API",
		Version = "v1"
	});

	// Custom operation IDs to avoid conflicts
	options.CustomOperationIds(apiDesc =>
	{
		return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)
			? $"{methodInfo.DeclaringType?.Name}_{methodInfo.Name}"
			: null;
	});
});

// DB Context
builder.Services.AddDbContext<DocumentDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
	?? ["http://localhost:4200"];
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngular",
		policy => policy
			.WithOrigins(allowedOrigins)
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials());
});

// Services
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Document API V1");
	});
}

app.UseCors("AllowAngular");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();



//using Document.Services;
//using DocumentAPI.Data;
//using DocumentAPI.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using System.Reflection;

//var builder = WebApplication.CreateBuilder(args);

//// ✅ DETAILED LOGGING
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.SetMinimumLevel(LogLevel.Information);

//// ✅ Controllers with better JSON handling
//builder.Services.AddControllers()
//	.AddJsonOptions(options =>
//	{
//		options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
//		options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
//	});

//builder.Services.AddEndpointsApiExplorer();

//// ✅ SIMPLIFIED SWAGGER (to isolate the issue)
//builder.Services.AddSwaggerGen(options =>
//{
//	options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//	{
//		Title = "Document API",
//		Version = "v1"
//	});

//	// Custom operation IDs to avoid conflicts
//	options.CustomOperationIds(apiDesc =>
//	{
//		return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)
//			? $"{methodInfo.DeclaringType?.Name}_{methodInfo.Name}"
//			: null;
//	});
//});

//// DB Context
//builder.Services.AddDbContext<DocumentDbContext>(options =>
//	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// CORS
//builder.Services.AddCors(options =>
//{
//	options.AddPolicy("AllowAngular",
//		policy => policy
//			.WithOrigins("http://localhost:4200")
//			.AllowAnyMethod()
//			.AllowAnyHeader()
//			.AllowCredentials());
//});

//// Services
//builder.Services.AddScoped<IFileStorageService, FileStorageService>();

//var app = builder.Build();

//// ✅ ALWAYS show detailed errors in development
//if (app.Environment.IsDevelopment())
//{
//	app.UseDeveloperExceptionPage();
//	app.UseSwagger();
//	app.UseSwaggerUI(c =>
//	{
//		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Document API V1");
//	});
//}

//app.UseHttpsRedirection();
//app.UseCors("AllowAngular");
//app.UseAuthorization();
//app.MapControllers();

//app.Run();