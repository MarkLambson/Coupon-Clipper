using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using BeltExam.Models;

namespace BeltExam.Controllers;

[SessionCheck]
public class CouponController : Controller
{
    private readonly ILogger<CouponController> _logger;
    private MyContext db;

    public CouponController(ILogger<CouponController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

// view all
    [HttpGet("coupons")]
    public IActionResult Index()
    {
        List<Coupon> allCoupons = db.Coupons.Include(coupon => coupon.AllAssociations)
        .Include(coupon => coupon.User).ToList();
        return View("AllCoupons", allCoupons);
    }


// new coupon form
    [HttpGet("coupons/new")]
    public IActionResult NewCoupon()
    {
        return View("NewCoupon");
    }


// create coupon
    [HttpPost("coupons/create")]
    public IActionResult CreateCoupon(Coupon newCoupon)
    {
        if (!ModelState.IsValid)
        {
            return View("NewCoupon");
        }

        newCoupon.UserId = (int)HttpContext.Session.GetInt32("UUID");

        db.Coupons.Add(newCoupon);
        db.SaveChanges();

        return RedirectToAction("Index");
    }


// use coupon
    [HttpPost("coupons/{id}")]
    public IActionResult UseCoupon(int id)
    {
        Association newAssociation = new Association()
        {
            UserId = HttpContext.Session.GetInt32("UUID"),
            CouponId = id
        };

        db.Associations.Add(newAssociation);

        db.SaveChanges();

        return RedirectToAction("Index");
    }


// privacy
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


// session check
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? userId = context.HttpContext.Session.GetInt32("UUID");
        // Check to see if we got back null
        if (userId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Index", "User", null);
        }
    }
}