using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.ViewModels;


namespace TaskManagement.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf" };
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

        public TasksController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Tasks
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();
            var vm = new CreateTaskViewModel();
            ViewBag.Tasks = tasks;
            return View(vm);
        }

        // POST: /Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTaskViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Tasks = await _context.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();
                return View("Index", vm);
            }

            string? originalName = null;
            string? storedName = null;

            if (vm.Attachment != null && vm.Attachment.Length > 0)
            {
                var ext = Path.GetExtension(vm.Attachment.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("Attachment", "Only image files (JPG, PNG, GIF, WEBP) and PDFs are allowed.");
                    ViewBag.Tasks = await _context.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();
                    return View("Index", vm);
                }

                if (vm.Attachment.Length > MaxFileSize)
                {
                    ModelState.AddModelError("Attachment", "File size must not exceed 10 MB.");
                    ViewBag.Tasks = await _context.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();
                    return View("Index", vm);
                }

                originalName = Path.GetFileName(vm.Attachment.FileName);
                storedName = $"{Guid.NewGuid()}{ext}";

                var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, storedName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await vm.Attachment.CopyToAsync(stream);
            }

            var task = new TaskItem
            {
                Title = vm.Title.Trim(),
                Description = vm.Description?.Trim(),
                Status = vm.Status,
                FileName = originalName,
                StoredFileName = storedName,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Task \"{task.Title}\" created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // DELETE: /Tasks/Delete/5  (AJAX)
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return Json(new { success = false, message = "Task not found." });

            // Remove physical file if present
            if (!string.IsNullOrEmpty(task.StoredFileName))
            {
                var filePath = Path.Combine(_env.WebRootPath, "uploads", task.StoredFileName);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Task \"{task.Title}\" deleted successfully." });
        }

        // GET: /Tasks/Download/5
        public async Task<IActionResult> Download(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null || string.IsNullOrEmpty(task.StoredFileName))
                return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, "uploads", task.StoredFileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var ext = Path.GetExtension(task.StoredFileName).ToLowerInvariant();
            var mimeType = ext == ".pdf" ? "application/pdf" : $"image/{ext.TrimStart('.')}";
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(bytes, mimeType, task.FileName ?? task.StoredFileName);
        }
    }
}
