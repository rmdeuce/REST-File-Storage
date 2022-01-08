using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RestApiFiles.Models
{
    public class File
    {
        [Key]
        public int Id { get; set; }

        public string FilesPath { get; set; }
        
        public string  Size { get; set; }
        public DateTime Creadt { get; set;  }
    }
}