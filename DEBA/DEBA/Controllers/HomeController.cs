using DEBA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UserCredentialsModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BC=BCrypt.Net.BCrypt;
using System.Security.Cryptography;

namespace DEBA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserCredentialsContext _context;
        
        int ID = 0;
        private object viewdata;
        private int name;

        public string UserCredentials { get; private set; }

        public HomeController(ILogger<HomeController> logger, UserCredentialsContext context)
        {
            _logger = logger;
            _context = context;
            
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(UserCredentialsController users)
        {
           
            var User = await _context.UserCredentials.Where( i=>
                i.UserNumber == users.UserNumber 
                ).FirstOrDefaultAsync();
           
            
            if (User == null || !verify(users.PinNumber,User.PinNumber)) 
            {

                TempData["Message"] = "Wrong User Number or Pin Number. Please try again";
                return RedirectToAction("Index");
            }

            
            ViewData["LastName"] = Encryption.Decrypt(User.LastName, User.PinNumber);
            ViewData["FirstName"] = Encryption.Decrypt(User.FirstName, User.PinNumber);
            ViewData["ExpirationDate"] = Encryption.Decrypt(User.ExpirationDate, User.PinNumber);
            ViewData["BankBalance"] = Encryption.Decrypt(User.BankBalance, User.PinNumber); 

          

         
            ViewData["ID"] = User.Id;
            return View("Deba_2");
        }

        public IActionResult Privacy()
        {   
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> DEBA1_5(UserCredentialsController users)
        {
            var details = await _context.UserCredentials.Where(i => i.Id == users.Id).FirstOrDefaultAsync();
            return RedirectToAction("DEBA_2");
        }
        
        public async Task<IActionResult> DEBA_2()

        {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public bool verify(string text, string hash) { return BC.Verify(text, hash); }
    }
}


