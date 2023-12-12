using System;
using UnityEngine;

namespace MyScriptableObjectClass
{
    [CreateAssetMenu(menuName = "ScriptableObject/AudioData")]
    [System.Serializable]
    public class AudioData : ScriptableObject
    {
        [SerializeField] AudioClip[] _audioSEClips;
        [SerializeField] AudioClip[] _audioBGMClips;

        public AudioClip[] AudioSEClips => _audioSEClips;
        public AudioClip[] AudioBGMClips => _audioBGMClips;
    }

}


