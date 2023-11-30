using Microsoft.EntityFrameworkCore;
using ServerYourWorldMMORPG.Database;
using ServerYourWorldMMORPG.Models.Game.User;

namespace ServerYourWorldMMORPG.Services.Application
{
	public class AccountService
	{
		private readonly ApplicationDbContext _context;

		public AccountService(ApplicationDbContext context)
		{
			_context = context;
		}

		public bool CanAddCharacterToAccount(Guid accountId)
		{
			var account = _context.Accounts.Include(a => a.Characters)
				.FirstOrDefault(a => a.Id == accountId);
			return account != null && account.Characters.Count < 4;
		}

		public void AddCharacterToAccount(Guid accountId, Character character)
		{
			if (CanAddCharacterToAccount(accountId))
			{
				var account = _context.Accounts.Find(accountId);
				account.Characters.Add(character);
				_context.SaveChanges();
			}
			else
			{
				// Handle the case where the account already has 4 characters
				// This could be logging an error, throwing an exception, etc.
			}
		}
	}
}
