// Controllers/HomeController.cs - UPDATED with two sections
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisManager.Data;
using ThesisManager.ViewModels;

namespace ThesisManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Public front page - showing theses in two sections
        public async Task<IActionResult> Index(string filter = "all")
        {
            var currentYear = DateTime.Now.Year;

            var query = _db.Students
                .Where(s => s.Status == "graduated")
                .Include(s => s.Supervisor)
                .Include(s => s.Thesis)
                    .ThenInclude(t => t!.Track)
                .Select(s => new PublicThesisViewModel
                {
                    StudentName = s.FullName,
                    StudentId = s.StudentId,
                    DegreeType = s.Thesis!.DegreeType,
                    TrackName = s.Thesis.Track != null ? s.Thesis.Track.Name : null,
                    TitleAr = s.Thesis.TitleAr,
                    TitleEn = s.Thesis.TitleEn,
                    AbstractAr = s.Thesis.AbstractAr,
                    SupervisorName = s.Supervisor != null ? s.Supervisor.FullName : null,
                    DefenseDate = s.Thesis.DefenseDate,
                    GraduationDate = s.ActualGraduationDate,
                    GraduationYear = s.ActualGraduationDate.HasValue ? s.ActualGraduationDate.Value.Year : null
                })
                .OrderByDescending(t => t.GraduationDate);

            var theses = await query.ToListAsync();

            // Split into two lists
            ViewBag.CurrentYearTheses = theses.Where(t => t.GraduationYear == currentYear).ToList();
            ViewBag.AllTheses = theses.ToList();
            ViewBag.Filter = filter;

            return View(theses);
        }
    }
}