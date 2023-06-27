using IdentityExploration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// 13. All code in this file are for no use! Delete this file, it is just for testing. 
namespace IdentityExploration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly DataContext _context;
        public TestController(DataContext context) 
        { 
            _context = context;
        }

        [HttpGet]
        [Route("/getAllUsers")]
        public IActionResult GetALlUsers()
        {
            List<Employee> employees = _context.Users.ToList();
            if (employees == null) return Ok("Error");
            return Ok(employees);
        }
    }
}
