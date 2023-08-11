namespace ServerYourWorldMMORPG.Services.Interfaces
{
    public interface ICommandService
    {
        Task ProcessCommandAsync(string input);
        Task InitializeAsync();
    }
}
