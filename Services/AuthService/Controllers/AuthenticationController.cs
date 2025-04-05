using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AuthService.Context;
using AuthService.Dtos.Request;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IConfiguration configuaration;

        public AuthenticationController(DataContext _context, IConfiguration _configuaration)
        {
            configuaration = _configuaration;
            context = _context;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await context.Users.AnyAsync(a => a.Email == request.Email))
                return BadRequest("มี email ในระบบแล้ว");

            var hmac = new HMACSHA512();
            var user = new User
            {
                Email = request.Email,

                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
                Role = "User"
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return Ok(new { message = "ลงทะเบียนสำเร็จ" });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await context.Users.FirstOrDefaultAsync(w => w.Email == request.Email);
            if (user == null)
                return Unauthorized(new { message = "ไม่พบผู้ใช้" });

            if (!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized(new { message = "รหัสผ่านไม่ถูกต้อง" });

            var token = CreateToken(user);
            return Ok(new { token = token });
        }

        private bool VerifyPassword(string password, byte[] hash, byte[] salt)
        {
            var hmac = new HMACSHA512(salt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computeHash.SequenceEqual(hash);
        }

        private string CreateToken(User user)
        {
            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("role", user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuaration["Jwt:Key"]!));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuaration["Jwt:Issuer"],
                audience: configuaration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}