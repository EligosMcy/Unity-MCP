using System;

namespace DemoModeA
{
    public interface IVoiceInputProvider
    {
        event Action<string> OnTranscript;
        void StartRecording();
        void StopRecording();
        bool IsRecording { get; }
    }
}
