using LoginLogoutExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LoginLogoutExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private LoginLogoutExampleContext _context;
        Userdetail user = new Userdetail();
        public HomeController(ILogger<HomeController> logger, LoginLogoutExampleContext context)
        {
            _logger = logger;
            _context = context;

        }
        public IActionResult Index()
        {
            var documents = _context.Documents.Where(d => d.UserId == int.Parse(HttpContext.Session.GetString("userId"))).ToList();
            return View(documents);
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        public IActionResult EditView(int FileId)
        {
            var document = _context.Documents.SingleOrDefault(d => d.FileId == FileId);
            return View(document);
        }
        [HttpPost]
        public IActionResult EditView(int FileId, string FileName, string FileContent)
        {
            user.Id = int.Parse(HttpContext.Session.GetString("userId"));
            Document updatedDocument = new Document()
            {
                FileId = FileId,
                FileName = FileName,
                FileContent = FileContent,
                User = user,
            };

            user = _context.Userdetails
                    .Include(u => u.Documents)
                    .FirstOrDefault(u => u.Id == user.Id);

            if (user != null)
            {
                // Find the specific document to update
                var existingDocument = user.Documents.FirstOrDefault(d => d.FileId == updatedDocument.FileId);

                if (existingDocument != null)
                {
                    // Update the document properties
                    existingDocument.FileName = updatedDocument.FileName;
                    existingDocument.FileContent = updatedDocument.FileContent;

                    // Save changes to the database
                    _context.SaveChanges();
                    // Update the file on disk
                    UpdateFileContent(existingDocument.FileName, existingDocument.FileContent);
                }
            }
            return View(updatedDocument);
        }

        private void UpdateFileContent(string fileName, string newContent)
        {
            // Construct the file path
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "upload", fileName);

            // Update the file content by overwriting the entire file
            System.IO.File.WriteAllText(filePath, newContent);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult UploadView()
        {
            var model = new FilesViewModel();
            foreach (var item in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "upload")))
            {
                model.Files.Add(new FileDetails { Name = System.IO.Path.GetFileName(item), Path = item });
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult UploadView(IFormFile[] files)
        {
            var UserId = HttpContext.Session.GetString("userId");
            foreach (var file in files)
            {
                var fileName = System.IO.Path.GetFileName(file.FileName);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "upload", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                using (var localFile = System.IO.File.OpenWrite(filePath))
                using (var uploadedFile = file.OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                }
                // Read the content of the file
                var fileContent = System.IO.File.ReadAllText(filePath);

                // Create a Document object
                var document = new Document
                {
                    FileName = fileName,
                    FileContent = fileContent,
                    // Other properties of your Document object
                    FileStatus = true,
                    UserId = int.Parse(UserId)
                };

                // Insert the Document into the database
                _context.Documents.Add(document);
                _context.SaveChanges();
            }

            // Get file from server
            var model = new FilesViewModel();
            foreach (var item in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "upload")))
            {
                model.Files.Add(new FileDetails
                {
                    Name = System.IO.Path.GetFileName(item), Path = item
                });
            }
            return View(model);
        }

        public async Task<IActionResult> Download(string fileName)
        {
            if (fileName == null)
            {
                return Content("Filename is not avalaible");
            }
            _logger.LogInformation("Thread is initialized and started!");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "upload", fileName);

            var memory = new MemoryStream();

            using (var stream = new FileStream(path, FileMode.Open))
            {
                _logger.LogInformation($"New thread is start! {Environment.CurrentManagedThreadId}");
                await Task.Delay(5000);
                await stream.CopyToAsync(memory);
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            }
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".mkv", "video/x-matroska"},
                {".mp4", "video/mp4"}
            };
        }
    }
}