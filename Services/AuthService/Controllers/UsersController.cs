using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-zone")]
        public IActionResult AdminZone()
        {
            return Ok(new { response = "You are Admin" });
        }

        [Authorize(Roles = "User")]
        [HttpGet("user-zone")]
        public IActionResult UserZone()
        {
            return Ok(new { response = "You are User" });
        }
    }
}