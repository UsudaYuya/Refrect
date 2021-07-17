using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Reflection
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;
        private void Awake()
        {
            if (Instance == null || Instance == this)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else 
            {
                Destroy(this.gameObject);
            }
        }

        AudioSource _audio;
        private void Start()
        {
            _audio = this.GetComponent<AudioSource>();
        }

        /// <summary>
        /// 効果音を鳴らす
        /// </summary>
        /// <param name="clip"></param>
        public void AudioShotPlay(AudioClip clip) 
        {
            _audio.GetComponent<AudioSource>().PlayOneShot(clip);
        }
    }
}