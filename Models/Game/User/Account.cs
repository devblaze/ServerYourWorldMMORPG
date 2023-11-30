using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ServerYourWorldMMORPG.Models.Game.User
{
	public class Account
	{
		[Key]
		public Guid Id { get; set; }

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

		public List<Character> Characters { get; set; } = new List<Character>();
	}
}
