using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisManager.Data;
using ThesisManager.Models;

namespace ThesisManager.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public StudentsController(ApplicationDbContext db) => _db = db;

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewBag.NumOfSteps = _db.NumOfSteps.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Student s)
        {
            if (!ModelState.IsValid) return View(s);
            _db.Students.Add(s);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        // details: show student and related drafts/dissertations/defences
        public async Task<IActionResult> Details(int id)
        {
            var student = await _db.Students
                .Include(s => s.NumOfStep)
                .Include(s => s.Drafts).ThenInclude(d => d.StudentGuide)
                .Include(s => s.Dissertations).ThenInclude(d => d.Supervisor)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();
            ViewBag.Supervisors = await _db.Supervisors.ToListAsync();
            ViewBag.StudentGuides = await _db.StudentGuides.ToListAsync();
            return View(student);
        }

        // add draft
        [HttpPost]
        public async Task<IActionResult> AddDraft(int studentId, string title, int studentGuideId, DateTime submissionDate, DateTime newDeadline)
        {
            var draft = new DissertationDraft
            {
                Title = title,
                StudentId = studentId,
                StudentGuideId = studentGuideId,
                SubmissionDate = submissionDate
            };
            _db.DissertationDrafts.Add(draft);

            // update student's deadline (as requested)
            var student = await _db.Students.FindAsync(studentId);
            if (student != null) student.Deadline = newDeadline;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = studentId });
        }

        // add dissertation
        [HttpPost]
        public async Task<IActionResult> AddDissertation(int studentId, string title, int supervisorId, DateTime submissionDate, DateTime newDeadline)
        {
            var diss = new Dissertation
            {
                Title = title,
                StudentId = studentId,
                SupervisorId = supervisorId,
                SubmissionDate = submissionDate
            };
            _db.Dissertations.Add(diss);

            var student = await _db.Students.FindAsync(studentId);
            if (student != null) student.Deadline = newDeadline;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = studentId });
        }

        // add defence - expects dissertationId and will create a defence record
        [HttpPost]
        public async Task<IActionResult> AddDefence(int studentId, int dissertationId)
        {
            var defence = new ThesisDefence { DissertationId = dissertationId };
            _db.ThesisDefences.Add(defence);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = studentId });
        }
    }
}