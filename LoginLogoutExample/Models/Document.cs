using System;
using System.Collections.Generic;

namespace LoginLogoutExample.Models
{
    public partial class Document
    {
        public int FileId { get; set; }
        public string FileName { get; set; } = null!;
        public string FileContent { get; set; } = null!;
        public bool FileStatus { get; set; }
        public int UserId { get; set; }

        public virtual Userdetail User { get; set; } = null!;
    }
}
