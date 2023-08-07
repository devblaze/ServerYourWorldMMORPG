using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace ServerYourWorldMMORPG.Models
{
    public class Character
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string CharacterName { get; set; }

        // Foreign key to the Account that owns this character
        public int AccountId { get; set; }

        // Navigation property for the account that owns this character
        public Account Account { get; set; }
    }
}
