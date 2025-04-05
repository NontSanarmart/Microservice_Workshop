using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Dtos.Request
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "กรุณาระบุ email")]
        [EmailAddress(ErrorMessage = "รูปแบบ email ไม่ถูกต้อง")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "กรุณาระบุ password")]
        public string Password { get; set; } = null!;
    }
}