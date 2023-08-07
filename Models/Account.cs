using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ServerYourWorldMMORPG.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [NotNull]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Password { get; set; }

        // Navigation property for the characters associated with this account
        public List<Character> Characters { get; set; } = new List<Character>();
    }
}
