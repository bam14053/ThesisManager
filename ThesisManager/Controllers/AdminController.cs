// Controllers/AdminController.cs - COMPLETE UPDATE with all new features
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ThesisManager.Data;
using ThesisManager.Models;
using ThesisManager.ViewModels;

namespace ThesisManager.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Students list
        public async Task<IActionResult> Students(string sortBy = "deadline")
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
            var userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

            ViewBag.UserRole = userRole;

            var query = _db.Students
                .Where(s => s.Status == "active")
                .Include(s => s.Tutor)
                .Include(s => s.Supervisor)
                .Include(s => s.Thesis)
                    .ThenInclude(t => t!.Track)
                .AsQueryable();

            // Filter by role
            if (userRole == "tutor")
            {
                query = query.Where(s => s.TutorId == userId);
            }
            else if (userRole == "supervisor")
            {
                query = query.Where(s => s.SupervisorId == userId);
            }

            // Sorting
            query = sortBy switch
            {
                "name" => query.OrderBy(s => s.FullName),
                "student_id" => query.OrderBy(s => s.StudentId),
                "deadline" => query.OrderBy(s => s.CurrentDeadline),
                _ => query.OrderBy(s => s.CurrentDeadline)
            };

            var students = await query
                .Select(s => new StudentProgressViewModel
                {
                    Id = s.Id,
                    StudentId = s.StudentId,
                    ResidencyId = s.ResidencyId,
                    FullName = s.FullName,
                    Email = s.Email,
                    Phone = s.Phone,
                    DegreeType = s.Thesis != null ? s.Thesis.DegreeType : null,
                    TrackName = s.Thesis != null && s.Thesis.Track != null ? s.Thesis.Track.Name : null,
                    CurrentStep = s.CurrentStep,
                    StepNameAr = _db.StepDefinitions.Where(sd => sd.StepNumber == s.CurrentStep).Select(sd => sd.StepNameAr).FirstOrDefault() ?? "",
                    CurrentDeadline = s.CurrentDeadline,
                    Status = s.Status,
                    TutorName = s.Tutor != null ? s.Tutor.FullName : null,
                    SupervisorName = s.Supervisor != null ? s.Supervisor.FullName : null,
                    IdeaTitle = s.Thesis != null ? s.Thesis.IdeaTitle : null,
                    ThesisTitle = s.Thesis != null ? s.Thesis.TitleAr : null,
                    DeadlineStatus = s.CurrentDeadline < DateTime.Now ? "overdue" : 
                                    (s.CurrentDeadline.HasValue && (s.CurrentDeadline.Value - DateTime.Now).TotalDays <= 7 ? "due_soon" : "on_track"),
                    EnrollmentDate = s.EnrollmentDate,
                    ExpectedGraduationDate = s.ExpectedGraduationDate
                })
                .ToListAsync();

            return View(students);
        }

        // GET: Student Details
        public async Task<IActionResult> StudentDetails(int id)
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
            ViewBag.UserRole = userRole;
            ViewBag.IsReadOnly = userRole != "admin";

            var student = await _db.Students
                .Include(s => s.Tutor)
                .Include(s => s.Supervisor)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();

            // Check access for non-admin users
            if (userRole == "tutor" || userRole == "supervisor")
            {
                var userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
                if ((userRole == "tutor" && student.TutorId != userId) ||
                    (userRole == "supervisor" && student.SupervisorId != userId))
                {
                    return Forbid();
                }
            }

            var thesis = await _db.Theses
                .Include(t => t.Track)
                .FirstOrDefaultAsync(t => t.StudentId == id);

            // Get step history WITHOUT calculations (do them in C# after retrieval)
            var stepHistoryRaw = await _db.StepHistory
                .Where(sh => sh.StudentId == id)
                .Include(sh => sh.StepDefinition)
                .OrderBy(sh => sh.StepNumber)
                .ToListAsync();

            // Now calculate in C# (not SQL)
            var stepHistory = stepHistoryRaw.Select(sh => new StepHistoryViewModel
            {
                StepNumber = sh.StepNumber,
                StepNameAr = sh.StepDefinition!.StepNameAr,
                StartDate = sh.StartDate,
                OriginalDeadline = sh.OriginalDeadline,
                CurrentDeadline = sh.CurrentDeadline,
                CompletionDate = sh.CompletionDate,
                Status = sh.Status,
                Notes = sh.Notes,
                DaysToComplete = sh.CompletionDate.HasValue ? 
                    (int)(sh.CompletionDate.Value - sh.StartDate).TotalDays : (int?)null,
                WasLate = sh.CompletionDate.HasValue && sh.CompletionDate.Value > sh.CurrentDeadline
            }).ToList();

            /*var stepHistory = await _db.StepHistory
                .Where(sh => sh.StudentId == id)
                .Include(sh => sh.StepDefinition)
                .OrderBy(sh => sh.StepNumber)
                .Select(sh => new StepHistoryViewModel
                {
                    StepNumber = sh.StepNumber,
                    StepNameAr = sh.StepDefinition!.StepNameAr,
                    StartDate = sh.StartDate,
                    OriginalDeadline = sh.OriginalDeadline,
                    CurrentDeadline = sh.CurrentDeadline,
                    CompletionDate = sh.CompletionDate,
                    Status = sh.Status,
                    Notes = sh.Notes,
                    DaysToComplete = sh.CompletionDate.HasValue ? 
                        (int)(sh.CompletionDate.Value - sh.StartDate).TotalDays : (int?)null,
                    WasLate = sh.CompletionDate.HasValue && sh.CompletionDate.Value > sh.CurrentDeadline
                })
                .ToListAsync();
            */
            var viewModel = new StudentDetailsViewModel
            {
                Id = student.Id,
                StudentId = student.StudentId,
                ResidencyId = student.ResidencyId,
                FullName = student.FullName,
                Email = student.Email,
                Phone = student.Phone,
                CurrentStep = student.CurrentStep,
                CurrentDeadline = student.CurrentDeadline,
                Status = student.Status,
                EnrollmentDate = student.EnrollmentDate,
                ExpectedGraduationDate = student.ExpectedGraduationDate,
                TutorId = student.TutorId,
                TutorName = student.Tutor?.FullName,
                SupervisorId = student.SupervisorId,
                SupervisorName = student.Supervisor?.FullName,
                Thesis = thesis != null ? new ThesisDetailsViewModel
                {
                    Id = thesis.Id,
                    DegreeType = thesis.DegreeType,
                    TrackId = thesis.TrackId,
                    TrackName = thesis.Track?.Name,
                    IdeaTitle = thesis.IdeaTitle,
                    IdeaDescription = thesis.IdeaDescription,
                    IdeaSubmissionDate = thesis.IdeaSubmissionDate,
                    IdeaApprovalDate = thesis.IdeaApprovalDate,
                    IdeaDenialReason = thesis.IdeaDenialReason,
                    ResearchPlan = thesis.ResearchPlan,
                    PlanSubmissionDate = thesis.PlanSubmissionDate,
                    TitleAr = thesis.TitleAr,
                    TitleEn = thesis.TitleEn,
                    AbstractAr = thesis.AbstractAr,
                    AbstractEn = thesis.AbstractEn,
                    DefenseCommittee = thesis.DefenseCommittee != null ? 
                        JsonSerializer.Deserialize<List<string>>(thesis.DefenseCommittee.ToString()!) : null,
                    CommitteeAssignmentDate = thesis.CommitteeAssignmentDate,
                    DefenseDate = thesis.DefenseDate,
                    DefenseTime = thesis.DefenseTime,
                    CommitteeReadingDeadline = thesis.CommitteeReadingDeadline,
                    DefenseCompletionDate = thesis.DefenseCompletionDate,
                    CorrectionDeadline = thesis.CorrectionDeadline,
                    CorrectedSubmissionDate = thesis.CorrectedSubmissionDate,
                    FinalSubmissionDate = thesis.FinalSubmissionDate
                } : null,
                StepHistory = stepHistory,
                AllSteps = await _db.StepDefinitions.OrderBy(sd => sd.StepNumber).ToListAsync()
            };

            // Load all tutors, supervisors, and tracks for dropdowns
            ViewBag.Tutors = await _db.Users.Where(u => u.Role == "tutor" && u.IsActive).ToListAsync();
            ViewBag.Supervisors = await _db.Users.Where(u => u.Role == "supervisor" && u.IsActive).ToListAsync();
            ViewBag.Tracks = await _db.Tracks.OrderBy(t => t.Name).ToListAsync();

            return View(viewModel);
        }

        // POST: Extend Deadline
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ExtendDeadline(int studentId, int stepNumber, DateTime newDeadline)
        {
            var stepHistory = await _db.StepHistory
                .FirstOrDefaultAsync(sh => sh.StudentId == studentId && sh.StepNumber == stepNumber);

            if (stepHistory == null) return NotFound();

            stepHistory.CurrentDeadline = newDeadline;
            stepHistory.Status = "extended";
            stepHistory.UpdatedAt = DateTime.Now;

            var student = await _db.Students.FindAsync(studentId);
            if (student != null && student.CurrentStep == stepNumber)
            {
                student.CurrentDeadline = newDeadline;
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(StudentDetails), new { id = studentId });
        }

        // POST: Complete Step
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CompleteStep(int studentId, int stepNumber, DateTime? completionDate = null)
        {
            completionDate ??= DateTime.Now;

            var dateOnly = completionDate.Value.Date;

            await _db.Database.ExecuteSqlRawAsync(
                "SELECT complete_step({0}, {1}, {2}::date, {3})",
                studentId, stepNumber, dateOnly, GetCurrentUserId()
            );

            return RedirectToAction(nameof(StudentDetails), new { id = studentId });
        }

        // GET: Create Student (NO enrollment date input)
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateStudent()
        {
            ViewBag.Tutors = await _db.Users.Where(u => u.Role == "tutor" && u.IsActive).ToListAsync();
            return View();
        }

        // POST: Create Student (enrollment date = today automatically)
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateStudent(CreateStudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Tutors = await _db.Users.Where(u => u.Role == "tutor" && u.IsActive).ToListAsync();
                return View(model);
            }

            var enrollmentDate = DateTime.Now;
            var firstStepDuration = await _db.StepDefinitions
                .Where(sd => sd.StepNumber == 1)
                .Select(sd => sd.DefaultDurationDays)
                .FirstOrDefaultAsync();

            var firstDeadline = enrollmentDate.AddDays(firstStepDuration ?? 30);

            var student = new Student
            {
                StudentId = model.StudentId,
                ResidencyId = model.ResidencyId,
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                TutorId = model.TutorId,
                CurrentStep = 1,
                CurrentDeadline = firstDeadline,
                EnrollmentDate = enrollmentDate,
                Status = "active"
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            // Create first step history entry
            var stepHistory = new StepHistory
            {
                StudentId = student.Id,
                StepNumber = 1,
                StartDate = enrollmentDate,
                OriginalDeadline = firstDeadline,
                CurrentDeadline = firstDeadline,
                Status = "in_progress",
                CreatedBy = GetCurrentUserId()
            };

            _db.StepHistory.Add(stepHistory);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Students));
        }

        // GET: Create Thesis Only (for graduated students)
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateThesis()
        {
            ViewBag.Tracks = await _db.Tracks.OrderBy(t => t.Name).ToListAsync();
            ViewBag.Supervisors = await _db.Users.Where(u => u.Role == "supervisor" && u.IsActive).ToListAsync();
            return View();
        }

        // POST: Create Thesis Only
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateThesis(CreateThesisViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Tracks = await _db.Tracks.OrderBy(t => t.Name).ToListAsync();
                ViewBag.Supervisors = await _db.Users.Where(u => u.Role == "supervisor" && u.IsActive).ToListAsync();
                return View(model);
            }

            // Handle new track creation
            int? trackId = model.TrackId;
            if (!string.IsNullOrWhiteSpace(model.NewTrackName))
            {
                var newTrack = new Track { Name = model.NewTrackName };
                _db.Tracks.Add(newTrack);
                await _db.SaveChangesAsync();
                trackId = newTrack.Id;
            }

            // Create student (graduated)
            var student = new Student
            {
                StudentId = model.StudentId,
                ResidencyId = model.ResidencyId,
                FullName = model.StudentName,
                Email = model.Email,
                Phone = model.Phone,
                SupervisorId = model.SupervisorId,
                CurrentStep = 12,
                EnrollmentDate = DateTime.Now,
                ActualGraduationDate = model.DefenseDate ?? DateTime.Now,
                Status = "graduated"
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            // Create thesis
            var thesis = new Thesis
            {
                StudentId = student.Id,
                DegreeType = model.DegreeType,
                TrackId = trackId,
                TitleAr = model.TitleAr,
                TitleEn = model.TitleEn,
                AbstractAr = model.AbstractAr,
                AbstractEn = model.AbstractEn,
                DefenseDate = model.DefenseDate
            };

            _db.Theses.Add(thesis);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Students));
        }

        // POST: Add new track (AJAX)
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddTrack([FromBody] CreateTrackViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                return BadRequest("Track name is required");

            var track = new Track { Name = model.Name };
            _db.Tracks.Add(track);
            await _db.SaveChangesAsync();

            return Json(new { id = track.Id, name = track.Name });
        }

        // GET: Create Supervisor
        [Authorize(Roles = "admin")]
        public IActionResult CreateSupervisor()
        {
            return View();
        }

        // POST: Create Supervisor
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateSupervisor(CreateUserViewModel model)
        {
            model.Role = "supervisor";
            var result = await CreateUserWithRandomPassword(model);
            if (result.success)
            {
                TempData["GeneratedPassword"] = result.password;
                TempData["UserName"] = model.FullName;
                TempData["UserEmail"] = model.Email;
                return RedirectToAction("ShowGeneratedPassword");
            }
            return View(model);
        }

        // GET: Create Tutor
        [Authorize(Roles = "admin")]
        public IActionResult CreateTutor()
        {
            return View();
        }

        // POST: Create Tutor
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateTutor(CreateUserViewModel model)
        {
            model.Role = "tutor";
            var result = await CreateUserWithRandomPassword(model);
            if (result.success)
            {
                TempData["GeneratedPassword"] = result.password;
                TempData["UserName"] = model.FullName;
                TempData["UserEmail"] = model.Email;
                return RedirectToAction("ShowGeneratedPassword");
            }
            return View(model);
        }

        // POST: Add new tutor (AJAX for quick add)
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddTutor([FromBody] CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");

            model.Role = "tutor";
            var randomPassword = GenerateRandomPassword();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(randomPassword);

            var user = new User
            {
                Email = model.Email,
                PasswordHash = passwordHash,
                FullName = model.FullName,
                Phone = model.Phone,
                Role = model.Role,
                IsActive = true
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Json(new { id = user.Id, fullName = user.FullName, password = randomPassword });
        }

        // GET: Create Admin
        [Authorize(Roles = "admin")]
        public IActionResult CreateAdmin()
        {
            return View();
        }

        // POST: Create Admin
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateAdmin(CreateUserViewModel model)
        {
            model.Role = "admin";
            var result = await CreateUserWithRandomPassword(model);
            if (result.success)
            {
                TempData["GeneratedPassword"] = result.password;
                TempData["UserName"] = model.FullName;
                TempData["UserEmail"] = model.Email;
                return RedirectToAction("ShowGeneratedPassword");
            }
            return View(model);
        }

        private async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            // This method is now replaced by CreateUserWithRandomPassword
            // Kept for backwards compatibility if needed
            var result = await CreateUserWithRandomPassword(model);
            if (result.success)
            {
                return RedirectToAction(nameof(Students));
            }
            return View(model);
        }

        // GET: Users List (supervisors and tutors)
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Users(string role = "all")
        {
            var query = _db.Users.AsQueryable();

            if (role == "supervisor")
                query = query.Where(u => u.Role == "supervisor");
            else if (role == "tutor")
                query = query.Where(u => u.Role == "tutor");
            else
                query = query.Where(u => u.Role == "supervisor" || u.Role == "tutor");

            var users = await query.OrderBy(u => u.FullName).ToListAsync();

            ViewBag.Role = role;
            return View(users);
        }

        // GET: User Details
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Get students under this user
            var students = user.Role == "tutor"
                ? await _db.Students.Where(s => s.TutorId == id && s.Status == "active")
                    .Include(s => s.Thesis).ThenInclude(t => t!.Track)
                    .ToListAsync()
                : await _db.Students.Where(s => s.SupervisorId == id && s.Status == "active")
                    .Include(s => s.Thesis).ThenInclude(t => t!.Track)
                    .ToListAsync();

            var viewModel = new UserDetailsViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Students = students.Select(s => new StudentProgressViewModel
                {
                    Id = s.Id,
                    StudentId = s.StudentId,
                    FullName = s.FullName,
                    DegreeType = s.Thesis?.DegreeType,
                    TrackName = s.Thesis?.Track?.Name,
                    CurrentStep = s.CurrentStep,
                    CurrentDeadline = s.CurrentDeadline
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Edit User
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditUser(int id, string fullName, string phone, string email)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FullName = fullName;
            user.Phone = phone;
            user.Email = email;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(UserDetails), new { id });
        }

        // POST: Reset User Password
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ResetUserPassword(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Generate random password
            var randomPassword = GenerateRandomPassword();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(randomPassword);
            user.RequirePasswordReset = true;
            await _db.SaveChangesAsync();

            TempData["Success"] = "سيُطلب من المستخدم تغيير كلمة المرور عند تسجيل الدخول التالي";
            TempData["GeneratedPassword"] = randomPassword;
            TempData["UserName"] = user.FullName;
            TempData["UserEmail"] = user.Email;

            return RedirectToAction("ShowGeneratedPassword");
        }

        // GET: Show Generated Password
        [Authorize(Roles = "admin")]
        public IActionResult ShowGeneratedPassword()
        {
            if (TempData["GeneratedPassword"] == null)
                return RedirectToAction("Students");

            ViewBag.GeneratedPassword = TempData["GeneratedPassword"];
            ViewBag.UserName = TempData["UserName"];
            ViewBag.UserEmail = TempData["UserEmail"];
            
            return View();
        }

        // Helper: Generate random password
        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789!@#$%";
            var random = new Random();
            var password = new char[12];
            
            // Ensure at least one uppercase, one lowercase, one number, one special
            password[0] = chars[random.Next(0, 23)]; // uppercase
            password[1] = chars[random.Next(23, 46)]; // lowercase
            password[2] = chars[random.Next(46, 56)]; // number
            password[3] = chars[random.Next(56, chars.Length)]; // special
            
            // Fill the rest randomly
            for (int i = 4; i < 12; i++)
            {
                password[i] = chars[random.Next(chars.Length)];
            }
            
            // Shuffle the password
            return new string(password.OrderBy(x => random.Next()).ToArray());
        }

        //Helper: Create user with random password
        private async Task<(bool success, string password)> CreateUserWithRandomPassword(CreateUserViewModel model)
        {
            if (!ModelState.IsValid) return (false, string.Empty);

            var randomPassword = GenerateRandomPassword();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(randomPassword);

            var user = new User
            {
                Email = model.Email,
                PasswordHash = passwordHash,
                FullName = model.FullName,
                Phone = model.Phone,
                Role = model.Role,
                IsActive = true,
                RequirePasswordReset = true
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return (true, randomPassword);
        }

        // GET: Manage Steps
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ManageSteps()
        {
            var steps = await _db.StepDefinitions.OrderBy(s => s.StepNumber).ToListAsync();
            return View(steps);
        }

        // POST: Check if step has history (AJAX)
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CheckStepUsage(int stepNumber)
        {
            var count = await _db.StepHistory.CountAsync(sh => sh.StepNumber == stepNumber);
            return Json(new { hasHistory = count > 0, count = count });
        }

        // POST: Save Steps Changes (AJAX) - SIMPLIFIED VERSION
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> SaveStepsChanges([FromBody] SaveStepsRequest request)
    {
        try
        {
            // 1. Update existing steps
            foreach (var updatedStep in request.UpdatedSteps)
            {
                var step = await _db.StepDefinitions.FindAsync(updatedStep.StepNumber);
                if (step != null)
                {
                    step.StepNameAr = updatedStep.StepNameAr;
                    step.StepNameEn = updatedStep.StepNameEn;
                    step.DefaultDurationDays = updatedStep.DefaultDurationDays;
                }
            }

            // 2. Add new steps at the end
            if (request.NewSteps.Any())
            {
                var maxStepNumber = await _db.StepDefinitions.MaxAsync(s => s.StepNumber);
                
                foreach (var newStep in request.NewSteps)
                {
                    maxStepNumber++;
                    var stepDef = new StepDefinition
                    {
                        StepNumber = maxStepNumber,
                        StepNameAr = newStep.StepNameAr,
                        StepNameEn = newStep.StepNameEn,
                        DefaultDurationDays = newStep.DefaultDurationDays
                    };
                    _db.StepDefinitions.Add(stepDef);
                }
            }

            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { 
                success = false, 
                message = $"حدث خطأ أثناء الحفظ: {ex.Message}" 
            });
        }
    }



        private int GetCurrentUserId()
        {
            return int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
        }
    }
}