using DEBA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UserCredentialsModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BC=BCrypt.Net.BCrypt;
using System.Security.Cryptography;

// Initialize the variables.
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

        // Post request handler used for the login. It logs the user in reading the credentials
        [HttpPost]
        public async Task<IActionResult> Index(UserCredentialsController users)
        {
            var user = await _context.UserCredentials.FirstOrDefaultAsync(i => i.UserNumber == users.UserNumber);

            if (user == null || !verify(users.PinNumber, user.PinNumber))
            {
                // Authentication failed, display alert popup
                ViewData["error"] = "<script>showAlert();</script>";
                return View("Index");
            }
            else
            {
                // Authentication succeeded, decrypt user data and proceed to the next view
                ViewData["LastName"] = Encryption.Decrypt(user.LastName, user.PinNumber);
                ViewData["FirstName"] = Encryption.Decrypt(user.FirstName, user.PinNumber);
                ViewData["ExpirationDate"] = Encryption.Decrypt(user.ExpirationDate, user.PinNumber);
                ViewData["BankBalance"] = Encryption.Decrypt(user.BankBalance, user.PinNumber);
                ViewData["ID"] = user.Id;
                return View("Deba_2");
            }
        }
     

        public IActionResult Privacy()
        {   
            return View();
        }
      // Displays the user information from the database
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
        // Displays an error message 
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //Verifying the password
        public bool verify(string text, string hash) { return BC.Verify(text, hash); }
    }
}


