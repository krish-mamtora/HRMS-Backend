using HRMS_Backend.Services.Email;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers.Email
{
    public class EmailController : Controller
    {
        public readonly IEmailService _service;
        public EmailController(IEmailService service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var receiver = "fakir15156@deposin.com";
            var subject = "Test";
            var message = "Helloo";
            await _service.SendEmailAsync(receiver, subject, message);
            return View();
        }
    }
}
