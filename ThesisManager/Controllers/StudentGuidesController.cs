using Microsoft.AspNetCore.Mvc;
using ThesisManager.Data;
using ThesisManager.Models;

namespace ThesisManager.Controllers
{
    public class StudentGuidesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public StudentGuidesController(ApplicationDbContext db) => _db = db;

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(StudentGuide g)
        {
            if (!ModelState.IsValid) return View(g);
            _db.StudentGuides.Add(g);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}