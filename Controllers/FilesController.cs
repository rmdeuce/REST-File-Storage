using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using RestApiFiles.Data;
using RestApiFiles.Models;
using File = RestApiFiles.Models.File;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace RestApiFiles.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FilesController : Controller
    {
        private IWebHostEnvironment hostEnvironment;
        public FilesDbContext dbcontext;
        
        public FilesController(IWebHostEnvironment hostingEnv, FilesDbContext _dbcontext)
        {
            hostEnvironment = hostingEnv;
            dbcontext = _dbcontext; 
        }
        
        [HttpPost]
        public ActionResult<string> PostFile(int id)
        {
            try
            {
                var files = HttpContext.Request.Form.Files;

                if (files.Count>0)
                {
                    foreach (var file in files)
                    {
                        FileInfo fi = new FileInfo(file.FileName);
                        var newfilname = "File" + DateTime.Now.TimeOfDay.Milliseconds + fi.Extension;
                        var dirPath = Path.Combine(hostEnvironment.WebRootPath, "Files");
                        if (!Directory.Exists(dirPath))
                            Directory.CreateDirectory(dirPath);
                        var filePath = Path.Combine(dirPath, newfilname);

                        using (var stream= new FileStream(filePath,FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        File fayl = new File();
                        fayl.FilesPath = Path.Combine("Files",newfilname);
                        fayl.Size = file.Length+ " bytes";
                        fayl.Creadt=DateTime.Now;
                        dbcontext.Files.Add(fayl);
                        dbcontext.SaveChanges();
                    }

                    return "Post successfully";
                }
                else
                {
                    return " error post";
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        public ActionResult<List<File>> GetAll()
        {
            var result = dbcontext.Files.ToList();

            return result;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> DownloadFile([FromRoute] int id, [FromServices] FilesDbContext context)
        {
            var result = await context.Files.FirstOrDefaultAsync(x => x.Id == id);

            var uploadFilesPath = Path.Combine(hostEnvironment.WebRootPath, result?.FilesPath);

            byte[] stream = await System.IO.File.ReadAllBytesAsync(uploadFilesPath);
            return File(stream, "text/plain", Path.GetFileName(uploadFilesPath));

        }
        
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<File>> Delete([FromRoute]int id, [FromServices] FilesDbContext context)
        {
            var result = await context.Files.FirstOrDefaultAsync(x => x.Id == id);
            if(result == null)
                return NotFound(new { message = "files Not Found"});
            
            try
            {
                context.Files.Remove(result);
                await context.SaveChangesAsync();
                return Ok(new { message = "files Removed"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Could not Delete this files. Error: {ex.Message}"});
            }
            
        }
    }
}