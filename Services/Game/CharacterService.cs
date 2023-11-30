using Microsoft.EntityFrameworkCore;
using ServerYourWorldMMORPG.Database;
using ServerYourWorldMMORPG.Models.Game.User;
using System.Text.Json;

namespace ServerYourWorldMMORPG.Services.Game
{
	public class CharacterService
	{
		private readonly ApplicationDbContext _dbContext;

		public CharacterService(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<Character> CreateCharacterAsync(Character character)
		{
			_dbContext.Characters.Add(character);
			await _dbContext.SaveChangesAsync();
			return character;
		}

		public async Task<Character?> UpdateCharacterAsync(Guid id, Character updatedCharacter)
		{
			var character = await _dbContext.Characters.FirstOrDefaultAsync(c => c.Id == id);
			if (character != null)
			{
				character.Name = updatedCharacter.Name;
				character.Level = updatedCharacter.Level;
				// Update other properties as needed

				_dbContext.Characters.Update(character);
				await _dbContext.SaveChangesAsync();
			}
			return character; // Return the updated character or null if not found
		}

		public async Task<bool> DeleteCharacterAsync(Guid id)
		{
			var character = await _dbContext.Characters.FirstOrDefaultAsync(c => c.Id == id);
			if (character != null)
			{
				_dbContext.Characters.Remove(character);
				await _dbContext.SaveChangesAsync();
				return true; // Return true if deletion is successful
			}
			return false; // Return false if the character was not found
		}

		public static string SerializeCharacterToJson(Character character)
		{
			return JsonSerializer.Serialize(character);
		}

		public static Character? DeserializeCharacterFromJson(string json)
		{
			return JsonSerializer.Deserialize<Character>(json);
		}
	}
}
