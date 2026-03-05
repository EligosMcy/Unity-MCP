using System;
using UnityEngine;

namespace DemoModeA
{
    public class VoiceInputProvider : MonoBehaviour, IVoiceInputProvider
    {
        public event Action<string> OnTranscript;
        private bool _isRecording;
        public bool IsRecording => _isRecording;

        public void StartRecording()
        {
            _isRecording = true;
        }

        public void StopRecording()
        {
            _isRecording = false;
        }

        private void Update()
        {
            if (!_isRecording) return;
        }
    }
}
