using System;
using UnityEngine;

namespace MyScriptableObjectClass
{
    [CreateAssetMenu(menuName = "ScriptableObject/AudioData")]
    [Serializable]
    public class AudioData : ScriptableObject
    {
        [SerializeField] private AudioClip[] _audioSEClips;
        [SerializeField] private AudioClip[] _audioBGMClips;

        public AudioClip[] AudioSEClips => _audioSEClips;
        public AudioClip[] AudioBGMClips => _audioBGMClips;
    }
}
