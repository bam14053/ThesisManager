using Microsoft.AspNetCore.Mvc;
using ThesisManager.Data;
using ThesisManager.Models;

namespace ThesisManager.Controllers
{
    public class SupervisorsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SupervisorsController(ApplicationDbContext db) => _db = db;

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Supervisor sup)
        {
            if (!ModelState.IsValid) return View(sup);
            _db.Supervisors.Add(sup);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}