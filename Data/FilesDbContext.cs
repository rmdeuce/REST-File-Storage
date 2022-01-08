
using Microsoft.EntityFrameworkCore;
using RestApiFiles.Models;

namespace RestApiFiles.Data
{
    public class FilesDbContext: DbContext
    {
        public FilesDbContext(DbContextOptions<FilesDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<File>Files { get; set; }

    }
}