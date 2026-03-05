using System.Threading.Tasks;

namespace DemoModeA
{
    public interface IDialogueGenerator
    {
        string GenerateReply(string userInput, EmotionProfile profile);
        Task<string> GenerateReplyAsync(string userInput, EmotionProfile profile);
    }
}
