using EmailProvider.Interfaces;
using EmailProvider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skills_hub.core.Repository.User;
using skills_hub.domain.Models.LessonTypes;
using skills_hub.persistence;

namespace skills_hub.Controllers;

public class HomeController : Controller
{
    private readonly IMailService _mailService;
    private readonly IUserService _userService;
    private readonly ApplicationDbContext _context;

    public HomeController(IMailService mailService, IUserService userService,
     ApplicationDbContext context)
    {
        _mailService = mailService;
        _userService = userService;
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userService.GetCurrentUserAsync();
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Item", "Account", new { id = user.Id });
            }

            ViewBag.TotalTeachers = _context.Teachers.Count();
            ViewBag.TotalStudents = _context.Students.Count();
            ViewBag.ActiveTotalTeachers = _context.Teachers.Where(x => x.IsDeleted == false).Count();
            ViewBag.ActiveTotalStudents = _context.Students.Where(x => x.IsDeleted == false).Count();
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.CountClasses = //_context.Teachers.Include(x => x.Lessons).Count();
            ViewBag.CountMails = _context.EmailMessages.Count();

            return View("IndexСRM", user);
        }
        return View();

    }

    [Route("thanks")]
    public IActionResult Thanks()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "CRM");
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Courses()
    {
        //var items = await _lessonTypeService.GetAll().ToListAsync();
        //var result = items.Where(x => x.ParentId == null || (x.ParentId != null && x.ParentId == Guid.Empty) && !string.IsNullOrEmpty(x.DisplayName)).ToList();
        return PartialView(new List<LessonType>());
    }


    [HttpPost]
    public async Task<IActionResult> SendMessage(SendingMessage msg)
    {

        //if (!ModelState.IsValid) return View("Index",msg);

        await _mailService.SendEmailAsync(msg);
        var message = new domain.Models.EmailMessage() { Data = msg.Data, Email = msg.Email, Date = msg.Date, Name = msg.Name, Phone = msg.Phone };

        return Redirect("~/thanks");
    }
}
