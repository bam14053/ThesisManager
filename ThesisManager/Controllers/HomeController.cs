using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisManager.Data;
using ThesisManager.Models;

namespace ThesisManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index(string sortBy = "deadline", bool asc = true)
        {
            var q = _db.Students
                .Include(s => s.NumOfStep)
                .AsQueryable();

            // default sort by deadline
            q = sortBy switch
            {
                "name" => asc ? q.OrderBy(s => s.Name) : q.OrderByDescending(s => s.Name),
                "s_id" => asc ? q.OrderBy(s => s.S_Id) : q.OrderByDescending(s => s.S_Id),
                "iqaamah_id" => asc ? q.OrderBy(s => s.Iqaamah_Id) : q.OrderByDescending(s => s.Iqaamah_Id),
                "deadline" => asc ? q.OrderBy(s => s.Deadline) : q.OrderByDescending(s => s.Deadline),
                _ => asc ? q.OrderBy(s => s.Deadline) : q.OrderByDescending(s => s.Deadline),
            };

            var students = await q.ToListAsync();
            ViewBag.SortBy = sortBy;
            ViewBag.Asc = asc;
            return View(students);
        }
    }
}