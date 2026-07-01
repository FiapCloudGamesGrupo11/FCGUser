using System.ComponentModel.DataAnnotations;

namespace UserAPI.Application.DTOs.Request
{
    public class UserRequestView
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
