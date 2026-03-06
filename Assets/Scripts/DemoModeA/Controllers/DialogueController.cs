using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace DemoModeA
{
    public class DialogueController : MonoBehaviour, IDialogueGenerator
    {
        [SerializeField] private float _randomExtendChanceMin = 0.1f;
        [SerializeField] private float _randomExtendChanceMax = 0.2f;
        private static readonly IReadOnlyDictionary<string, string> EmojiTokenMap = new Dictionary<string, string>
        {
            { "🙂", "[Happy]" },
            { "😉", "[Wink]" },
            { "😊", "[Happy]" },
            { "✨", "[Sparkle]" },
            { "🎉", "[Celebrate]" },
            { "🥳", "[Celebrate]" },
            { "🚀", "[Rocket]" },
            { "😪", "[Sleepy]" },
            { "😴", "[Sleepy]" },
            { "😒", "[Annoyed]" },
            { "🙄", "[EyeRoll]" },
        };

        public string GenerateReply(string userInput, EmotionProfile profile)
        {
            if (profile == null) return "";
            var core = buildCoreReply(userInput, profile);
            core = applyPunctuation(core, profile.PunctuationStyle);
            core = injectEmoji(core, profile);
            return core;
        }

        public async Task<string> GenerateReplyAsync(string userInput, EmotionProfile profile)
        {
            await Task.Yield();
            return GenerateReply(userInput, profile);
        }

        private string buildCoreReply(string userInput, EmotionProfile profile)
        {
            var builder = new StringBuilder();
            switch (profile.AttentionMode)
            {
                case AttentionMode.Reject:
                    builder.Append("No.");
                    return builder.ToString();
                case AttentionMode.Random:
                    var r = Random.value;
                    var p = Mathf.Lerp(_randomExtendChanceMin, _randomExtendChanceMax, Random.value);
                    if (r < p)
                    {
                        builder.Append("By the way, ");
                    }
                    break;
            }
            switch (profile.AffirmationBias)
            {
                case AffirmationBias.Positive:
                    builder.Append("Okay");
                    break;
                case AffirmationBias.Neutral:
                    builder.Append("Got it");
                    break;
                case AffirmationBias.Negative:
                    builder.Append("Hmm");
                    break;
            }
            if (profile.TextLengthBias == TextLengthBias.Short)
            {
                builder.Append(".");
            }
            else if (profile.TextLengthBias == TextLengthBias.Medium)
            {
                builder.Append(", let me try.");
            }
            else
            {
                builder.Append(", let me explain in more detail—you can add more context too.");
            }
            return builder.ToString();
        }

        private string applyPunctuation(string input, PunctuationStyle style)
        {
            if (string.IsNullOrEmpty(input)) return input;
            switch (style)
            {
                case PunctuationStyle.Minimal:
                    return input.Replace("!", "").Replace(".", "");
                case PunctuationStyle.Exclamatory:
                    if (!input.EndsWith("!")) return input.TrimEnd('.') + "!";
                    return input;
                case PunctuationStyle.EllipsisHeavy:
                    return input.TrimEnd('.') + "...";
                default:
                    return input;
            }
        }

        private string injectEmoji(string input, EmotionProfile profile)
        {
            if (profile.EmojiPool == null || profile.EmojiPool.Length == 0) return input;
            if (profile.EmojiDensity <= 0f) return input;
            if (Random.value > profile.EmojiDensity) return input;
            var idx = Mathf.FloorToInt(Random.value * profile.EmojiPool.Length) % profile.EmojiPool.Length;
            return input + " " + toEmojiToken(profile.EmojiPool[idx]);
        }

        private static string toEmojiToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return "";
            token = token.Trim();
            if (token.Length >= 2 && token[0] == '[' && token[token.Length - 1] == ']') return token;
            if (EmojiTokenMap.TryGetValue(token, out var mapped)) return mapped;
            return token;
        }
    }
}
