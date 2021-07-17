using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reflection.Title
{
    /// <summary>
    /// タイトル　　決定キーでセレクトステージに移動する
    /// </summary>
    public class Title : MonoBehaviour
    {
        [Header("Scene変更の効果音")]
        [SerializeField] private AudioClip _sceneChangeAudio = null;
        private void Update()
        {
            //決定キーでセレクトステージに移動する
            if (Input.GetButtonDown("Submit"))
            {
                GameController.Instance.AudioShotPlay(_sceneChangeAudio);
                SceneManager.LoadScene("StageSelect");
            }
        }
    }
}
