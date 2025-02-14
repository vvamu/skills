using AutoMapper;
using EmailProvider.Interfaces;
using EmailProvider.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using skills_hub.core.DTO;
using skills_hub.core.repository;
using skills_hub.core.Repository.User;
using skills_hub.domain.Models;
using skills_hub.domain.Models.Groups;
using skills_hub.domain.Models.User;
using skills_hub.Helpers.SearchModels;
using skills_hub.persistence;
using System.Text.RegularExpressions;
using Group = skills_hub.domain.Models.Groups.Group;

namespace skills_hub.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IGroupService _groupService;
    //private readonly ISalaryService _salaryService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMailService _mailService;
    //private readonly ILessonService _lessonService;
    //private readonly IApplicationUserBaseUserInfoService _applicationUserBaseUserInfoService;

    //private readonly INotificationService _notificationService;

    public AccountController(IUserService userService,
        ApplicationDbContext context, IMapper mapper, IGroupService groupService//, ISalaryService salaryService
        , UserManager<ApplicationUser> userManager//, ILessonService lessonService, IApplicationUserBaseUserInfoService applicationUserBaseUserInfoService,
        //INotificationService notificationService
        , IMailService mailService)
    {
        _userService = userService;
        _context = context;
        _mapper = mapper;
        _groupService = groupService;
        //_salaryService = salaryService;
        _userManager = userManager;
        //_lessonService = lessonService;
        //_applicationUserBaseUserInfoService = applicationUserBaseUserInfoService;
        _mailService = mailService;
        //_notificationService = notificationService;

    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult SignIn()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn(UserLoginDTO? user)
    {
        try
        {
            if (!ModelState.IsValid) { ModelState.AddModelError("", ModelState.Values.ToString()); return View(); }
            var userDb = await _userService.SignInAsync(user);
            if (userDb == null) return View();
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(); }

    }
    [HttpGet]
    [AllowAnonymous]
    public async new Task<IActionResult> SignOut()
    {
        await _userService.SignOutAsync();

        return RedirectToAction("Index", "Home");

    }

    #region Get

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        //var users = await _userService.GetAllAsync();
        HttpContext.Session.SetString("page", "index");

        return View();

    }
    [HttpGet]
    public async Task<IActionResult> Item(Guid itemId, Guid id)
    {


        ApplicationUser? user;
        if (itemId == Guid.Empty) itemId = id;

        user = await _userService.GetAsync(itemId);
        if (user == null) user = await _userService.GetCurrentUserAsync();
        HttpContext.Session.SetString("page", "item");

        //try
        //{
        //    if (user.UserTeacher != null)
        //    {

        //        user.UserTeacher.MonthCalculatedPriceCalculatedPrice = await _salaryService.GetTeacherSalaryAsync(user.UserTeacher);
        //        user.UserTeacher.TotalCalculatedPrice = await _salaryService.GetTeacherSalaryAsync(user.UserTeacher, true);
        //    }
        //    if (user.UserStudent != null)
        //    {
        //        user.UserStudent.CurrentCalculatedPrice = await _salaryService.GetStudentSalaryAsync(user.UserStudent);
        //        user.UserStudent.TotalCalculatedPrice = await _salaryService.GetStudentSalaryAsync(user.UserStudent, true);
        //    }
        //}
        //catch (Exception ex) { }

        return View(user);
    }




    [HttpPost]
    [Route("/Account/UsersTableList")]

    public async Task<IActionResult> UsersTableList(UserFilterModel filters, OrderModel order)
    {
        var users = await _userService.GetItems();
        users = await FilterMaster.FilterUsers(users, filters, order);
        List<ApplicationUser> list = new List<ApplicationUser>();



        if (!string.IsNullOrEmpty(filters.UserRole))
        {
            foreach (var i in users)
            {
                if ((await _userManager.IsInRoleAsync(i, filters.UserRole)))
                {
                    var us = await _userService.GetAsync(i.Id);
                    list.Add(us);
                }
            }

        }
        else
        {
            list = await users.ToListAsync();
        }
        //HttpContext.Session.SetString("page", "index");
        return PartialView("_UsersTableList", list);
    }

    [HttpGet]
    public async Task<IActionResult> UsersList()
    {
        var users = await _userService.GetItems();
        //HttpContext.Session.SetString("page", "index");
        return PartialView("_UsersList", await users.ToListAsync());
    }

    #endregion



    [HttpGet]
    public async Task<IActionResult> Create(Guid id)
    {
        var user = await _userService.GetCreateDTOByIdAsync(id);
        if (user == null) return View(new UserCreateDTO());
        return View(user);

    }
    [HttpPost]
    public async Task<IActionResult> Create(UserCreateDTO userCreateModel)
    {
        ApplicationUser user;

        try
        {
            if (userCreateModel.Id != Guid.Empty)
            {
                var u = await _userService.UpdateAsync(userCreateModel);
                user = u;
            }
            else
            {
                var u = await _userService.CreateAsync(userCreateModel);
                user = u;
                if (userCreateModel.IsStudent)
                {
                    if (userCreateModel.IsTeacher) HttpContext.Session.SetString("isTeacher", "true");
                    return RedirectToAction("Create", "Student", new { id = user.Id });
                }
                if (userCreateModel.IsTeacher) return RedirectToAction("Create", "Teachers", new { id = user.Id });
            }

        }
        catch (Exception ex) { ModelState.AddModelError("", ex.Message); return View(userCreateModel); }

        return RedirectToAction("Item", new { itemId = user.Id });
        //return View();
    }
    /*
    [HttpPost]
    public async Task<IActionResult> UpdateMainBaseUserInfo(BaseUserInfo baseUserInfo)
    {

    }
    */
    public async Task<IActionResult> WorkingSchedule(Guid? id)
    {
        if (id == null) return View(await _context.Users.FirstOrDefaultAsync());

        var user = await _userService.GetAsync((Guid)id);
        return View(user);

    }

    public async Task<IActionResult> Restore(Guid id)
    {
        var user = await _userService.GetAsync(id);
        user = await _userService.Restore(user);

        var returnUrl = HttpContext.Session.GetString("page");

        switch (returnUrl)
        {
            case "index": return RedirectToAction("Index");
            case "item": return RedirectToAction("Item", new { id = user.Id });
            default: return RedirectToAction("Index", "CRM");
        }
    }

    public async Task<IActionResult> SoftDelete(Guid id)
    {
        var user = await _userService.GetAsync(id);
        await _userService.DeleteAsync(user);
        var returnUrl = HttpContext.Session.GetString("page");

        switch (returnUrl)
        {
            case "index": return RedirectToAction("Index");
            case "item": return RedirectToAction("Item", new { id = user.Id });
            default: return RedirectToAction("Index", "CRM");
        }

    }

    public async Task<IActionResult> HardDelete(Guid id)
    {
        var user = await _userService.GetAsync(id);
        await _userService.DeleteAsync(user,true);
        var returnUrl = HttpContext.Session.GetString("page");

        switch (returnUrl)
        {
            case "index": return RedirectToAction("Index");
            case "item": return RedirectToAction("Item", new { id = user.Id });
            default: return RedirectToAction("Index", "CRM");
        }

    }

    [HttpGet]
    [Route("/Account/GetNotifications")]
    public async Task<IActionResult> GetNotifications()
    {
        var notifications = await _userService.GetCurrentUserNotifications();
        return PartialView("_Chat", notifications.OrderByDescending(x => x.DateCreated).ToList());

        //return Json(JsonSerializerToAjax.GetJsonByIQueriable(notifications));
    }

    [HttpPost]
    [Route("/Account/RemoveCurrentUserNotifications")]
    public async Task<IActionResult> RemoveCurrentUserNotifications()
    {
        var notifications = await _userService.GetCurrentUserNotifications();
        //await _notificationService.RemoveCurrentUserNotificationsAsync();
        return PartialView("_Chat", new List<NotificationMessage>());
    }

    [HttpPost]
    [Route("/Account/RemoveAllNotifications")]
    public async Task<IActionResult> RemoveAllNotifications()
    {
        var notifications = await _userService.GetCurrentUserNotifications();
        //await _notificationService.RemoveAllNotificationsAsync();
        var result = notifications.OrderByDescending(x => x.DateCreated).ToList() ?? new List<NotificationMessage>();
        return PartialView("_Chat", result);

    }

    [HttpGet]
    [Route("/Account/GetEmails")]
    public async Task<IActionResult> GetEmails(Guid? id)
    {
        List<EmailMessage> emails = new List<EmailMessage>();
        List<SendingMessage> sending = new List<SendingMessage>();
        var user = await _userService.GetCurrentUserAsync();


        if (!User.IsInRole("Admin")) id = user.Id;
        try
        {


            if (id != null && id != Guid.Empty)
            {
                emails = await _context.EmailMessages.Where(x => x.SenderId == id).ToListAsync();

            }
            else
            {
                emails = await _context.EmailMessages.ToListAsync();
            }
            if (emails.Count() == 0)
            {
                emails.Add(new EmailMessage() { SenderId = user.Id });
            }
        }
        catch (Exception ex) { }
        emails = emails.OrderByDescending(x => x.DateCreated).ToList();


        return PartialView("_ChatAdmin", emails ?? new List<EmailMessage>());
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(SendingMessage msg)
    {
        //await _mailService.SendEmailAsync(msg);
        var user = await _userService.GetCurrentUserAsync();
        var message = new EmailMessage() { Data = msg.Data, Email = msg.Email, Date = msg.Date, Name = msg.Name, Phone = msg.Phone, SenderId = user.Id };
        if (message == null) throw new ArgumentNullException(nameof(message));
        await _context.EmailMessages.AddAsync(message);
        await _context.SaveChangesAsync();
        return Ok(msg);
    }





    [HttpPost]
    public async Task<IActionResult> GetStudentGroupsByUser(Guid id, GroupFilterModel filters, OrderModel order)
    {
        var user = await _userService.GetAsync(id);
        if (user.UserStudent == null) return PartialView("_StudentGroups", (user, new List<domain.Models.Groups.Group>(), new List<Lesson>()));

        var groups = user.UserStudent.Groups;
        List<domain.Models.Groups.Group> studentGroups = new List<Group>();
        foreach (var gro in groups)
        {
            var group = await _groupService.GetLastValueAsync(gro.GroupId);
            studentGroups.Add(group);
        }

        var ress = await FilterMaster.FilterGroups(studentGroups.AsQueryable(), filters, order);
        var rerer = ress.ToList();

        var otherLessons = _context.LessonStudents.Where(x => x.StudentId == user.UserStudent.Id);//.Where(x=>)
        /*var otherLessons = _lessonService.GetAll()
            .Where(x => x.ArrivedStudents.Select(x => x.StudentId).Contains(user.UserStudent.Id))
            .Where(x => !studentGroups.Select(x => x.Id).Contains(x.GroupId));*/



        return PartialView("_StudentGroups", (user, rerer, new List<Lesson>()));

    }

    [HttpPost]
    public async Task<IActionResult> GetTeacherGroupsByUser(Guid id, GroupFilterModel filters, OrderModel order)
    {
        var gr = _groupService.GetItems();
        var user = await _userService.GetAsync(id);

        if (user.UserTeacher == null) return PartialView("_TeacherGroups", (user, new List<Group>(), new List<Lesson>()));

        var groups = user.UserTeacher.Groups.Where(x => x.Group != null && !x.Group.IsDeleted).ToList();
        List<Group> teacherGroups = new List<Group>();
        foreach (var gro in groups)
        {
            var group = await _groupService.GetLastValueAsync(gro.GroupId);
            teacherGroups.Add(group);
        }


        var ress = await FilterMaster.FilterGroups(teacherGroups.AsQueryable(), filters, order);
        var result = ress.ToList();
        /*
        var otherLessons = _lessonService.GetAll()
           .Where(x => x.Teacher.Id == user.UserTeacher.Id)
           .Where(x => !teacherGroups.Select(x => x.Id).Contains(x.Group.Id)).ToList();*/


        return PartialView("_TeacherGroups", (user, result, new List<Lesson>()));

    }



    


}
