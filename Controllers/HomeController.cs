using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using examPrep.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace examPrep.Controllers;
// Name this anything you want with the word "Attribute" at the end
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        // Check to see if we got back null
        if(userId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Auth", "Home", null);
        }
    }
}



public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;

    public HomeController(ILogger<HomeController> logger,MyContext context)
    {
        _context= context;
        _logger = logger;
    }
    
    [SessionCheck]
    public IActionResult Index()
    {
        
        ViewBag.aktivitet = _context.Aktivitetet.Include(e=>e.Creator).Include(e=>e.Pjesemarresit).ToList();
        return View();
    }
    [HttpGet("Auth")]
    public IActionResult Auth(){
        return View();
    }
    [HttpPost("Register")]
    public IActionResult Register(User useriNgaForm){
        if (ModelState.IsValid)
        {
             PasswordHasher<User> Hasher = new PasswordHasher<User>();   
            // Updating our newUser's password to a hashed version         
            useriNgaForm.Password = Hasher.HashPassword(useriNgaForm, useriNgaForm.Password);            
            //Save your user object to the database 
            _context.Add(useriNgaForm);
            _context.SaveChanges(); 
            return RedirectToAction("Auth");
        }
        return View("Auth");

    }
    [HttpPost("Login")]
    public IActionResult Login(Login useriNgaForm){
        if (ModelState.IsValid)
        {
            User? userInDb = _context.Users.FirstOrDefault(u => u.Username == useriNgaForm.LoginUsername); 
            if(userInDb == null)        
        {            
            // Add an error to ModelState and return to View!            
            ModelState.AddModelError("LoginUsername", "Invalid Username");            
            return View("Auth");        
        } 
        PasswordHasher<Login> hasher = new PasswordHasher<Login>();                    
        // Verify provided password against hash stored in db        
        var result = hasher.VerifyHashedPassword(useriNgaForm, userInDb.Password, useriNgaForm.LoginPassword);      

            if(result == 0)        
        {            
                ModelState.AddModelError("LoginPassword", "Invalid Password");            
            return View("Auth");    
        } 

        HttpContext.Session.SetInt32("UserId", userInDb.UserId);

            return RedirectToAction("Index");
        }
        return View("Auth");
    }
    [HttpGet("LogOut")]
    public IActionResult LogOut(){
        HttpContext.Session.Clear();
        return RedirectToAction("Auth");
    }
    [SessionCheck]
    [HttpGet("Aktiviteti")]
    public IActionResult Aktiviteti(){
        return View();
    }
    [SessionCheck]
    [HttpPost("AddAktiviteti")]
    public IActionResult AddAktiviteti(Aktiviteti NgaForm){
        if (ModelState.IsValid)
        {          
            NgaForm.UserId=HttpContext.Session.GetInt32("UserId");
            //Save your user object to the database 
            _context.Add(NgaForm);
            _context.SaveChanges(); 
            return RedirectToAction("Index");
        }
        return View("Aktiviteti");

    }
    [HttpGet("Details/{id}")]
    public IActionResult Details(int id){
        ViewBag.UserId=HttpContext.Session.GetInt32("UserId");
        ViewBag.aktivitetiNgaDb = _context.Aktivitetet.Include(e=>e.Creator).Include(e=>e.Pjesemarresit).FirstOrDefault(e=> e.AktivitetiId==id);

        return View();
    }
     [HttpPost("Merrpjese/{id}")]
    public IActionResult MerrPjese( Pjesemarrje pjesemarrje,int id){
        pjesemarrje.AktivitetiId = id;
        pjesemarrje.UserId=HttpContext.Session.GetInt32("UserId");
         _context.Add(pjesemarrje);
        _context.SaveChanges(); 
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
