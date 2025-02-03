using Microsoft.AspNetCore.Mvc;

namespace skills_hub.Controllers;

public class AccountController : Controller
{
    public async Task<IActionResult> Item()
    {
        return View("~/Views/Account/Item/Item.cshtml"); 
    }

    public async Task<IActionResult> Create()
    {
        return View("Create");
    }
    public async Task<IActionResult> SignIn()
    {
        return View("SignIn");
    }
}
