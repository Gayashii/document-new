using DocumentAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DocumentAPI.Data
{
	public class DocumentDbContext: DbContext
	{

		public DocumentDbContext(DbContextOptions<DocumentDbContext> options)
			: base(options)
		{
		}

		public DbSet<Documents> Documents { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Documents>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
				entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
				entity.Property(e => e.Status).HasMaxLength(50);
				entity.Property(e => e.Category).HasMaxLength(100);
			});
		}

	}
}
