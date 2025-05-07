using AtonInternshipAssigment.Models;
using AtonInternshipAssigment.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AtonInternshipAssigment.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository) 
        { 
            _userRepository = userRepository;
        }

    }
}
