using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BeltExam.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BeltExam.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private MyContext db;

    public UserController(ILogger<UserController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }


//index
    [HttpGet("")]
    public IActionResult Index()
    {
        return View("Index");
    }


// register
    [HttpPost("register")]
    public IActionResult Register(User user)
    {
        if (!ModelState.IsValid)
        {
            return Index();
        }

        PasswordHasher<User> hashbrowns = new PasswordHasher<User>();
        user.Password = hashbrowns.HashPassword(user, user.Password);

        db.Users.Add(user);
        db.SaveChanges();

        HttpContext.Session.SetInt32("UUID", user.UserId);
        HttpContext.Session.SetString("Username", user.Username);
        return RedirectToAction("Index", "Coupon");
    }


// login
    [HttpPost("login")]
    public IActionResult Login(LoginUser user)
    {
        if (!ModelState.IsValid)
        {
            return Index();
        }

        User? dbUser = db.Users.FirstOrDefault(u => u.Email == user.LoginEmail);

        if (dbUser == null)
        {
            ModelState.AddModelError("LoginEmail", "not found");
            return Index();
        }

        PasswordHasher<LoginUser> hashbrowns = new PasswordHasher<LoginUser>();
        PasswordVerificationResult pwCompareResult = hashbrowns.VerifyHashedPassword(user, dbUser.Password, user.LoginPassword);

        if (pwCompareResult == 0)
        {
            ModelState.AddModelError("LoginPassword", "invalid password");
            return Index();
        }

        HttpContext.Session.SetInt32("UUID", dbUser.UserId);
        HttpContext.Session.SetString("Username", dbUser.Username);
        return RedirectToAction("Index", "Coupon");
    }


// logout
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

// VIEW USER, M2M
    [SessionCheck]
    [HttpGet("users/{id}")]
    public IActionResult ViewUser(int id)
    {
        User? user = db.Users.Include(user => user.AllAssociations)
        .ThenInclude(association => association.Coupon).Include(user => user.Coupons).FirstOrDefault(user => user.UserId == id); //then include not needed

        if (user == null)
        {
            return RedirectToAction("Index");
        }

        return View("ViewUser", user);
    }


// Privacy
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}