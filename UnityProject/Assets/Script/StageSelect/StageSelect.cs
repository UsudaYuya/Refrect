using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Reflection.StageSelect
{
    public class StageSelect : MonoBehaviour
    {
        private enum STATE { STAGE, MODE }//STATE = ステージ選択　MODE = モード選択
        private STATE _state = STATE.STAGE;

        [Header("MODE選択UI")]
        [SerializeField] private GameObject _modeUIParent = null;

        [Header("開始時の選択されているボタン　0 = STAGE 1 = MODE")]
        [SerializeField] private Button[] StartButton = null;

        [Header("カーソル")]
        [SerializeField] private RectTransform _cursor = null;

        [Header("ステージを表示するImage")]
        [SerializeField] private Image _stageImage = null;
        [Header("生成回数のテキスト")]
        [SerializeField] private Text _lifeText = null;
        [Header("反射回数のテキスト")]
        [SerializeField] private Text _bounceText = null;
        [Header("☆")]
        [SerializeField] private GameObject[] _starImage = null;

        [Header("変更時の効果音")]
        [SerializeField] private AudioClip _changeAudio = null;
        [Header("選択時の効果音")]
        [SerializeField] private AudioClip _submitAudio = null;

        //ロードするステージ番号
        private int StageNo = 0;

        AudioSource _audio;
        private void Start()
        {
            _audio = Camera.main.GetComponent<AudioSource>();
            //モード選択のUIの非表示
            _modeUIParent.SetActive(false);
        }

        private void Update()
        {
            //ボタンが選択されていない場合
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                StartButton[(int)_state].Select();//初期ボタンに選択する
                return;
            }
            if (_state == STATE.STAGE)
            {
                //カーソルのターゲット先
                RectTransform cursorTarget =
                    EventSystem.current.currentSelectedGameObject
                    .GetComponent<RectTransform>();

                _cursor.SetParent(cursorTarget);
                //選択されている場合カーソルの位置を設定する
                _cursor.anchoredPosition = Vector2.zero;
                //カーソルの大きさの設定
                _cursor.sizeDelta = cursorTarget.sizeDelta * 1.1f;
            }
        }

        /*
         * 
         * ボタン処理
         *
         */

        /// <summary>
        /// 選択されているボタンの変更
        /// </summary>
        public void SelectButtonChange()
        {
            //選択時効果音
            _audio.PlayOneShot(_changeAudio);
        }

        /// <summary>
        /// ステージ番号の選択
        /// </summary>
        /// <param name="No"></param>
        public void StageNum(int No)
        {
            //決定効果音
            _audio.PlayOneShot(_submitAudio);

            StageNo = No;//ステージ番号

            _state = STATE.MODE;//選択状態の変更

            _modeUIParent.SetActive(true);//Mode選択の表示

            StartButton[(int)_state].Select();//ボタンの選択

            /*UIの設定*/
            _stageImage.sprite = StageData._data[No].sprite;//ステージ画像
            _lifeText.text = StageData._data[No].Life.ToString();//生成回数
            _bounceText.text = StageData._data[No].Bounce.ToString();//反射回数

            //☆の数
            for (int i = 0; i < 3; i++)
                _starImage[i].SetActive(StageData._data[No].Star > i);
        }

        /// <summary>
        /// タイトルに戻る
        /// </summary>
        public void TitleBack()
        {
            GameController.Instance.AudioShotPlay(_submitAudio);//効果音
            //タイトルシーンに移動
            SceneManager.LoadScene("Title");
        }

        /// <summary>
        /// StateをSTAGEに戻す
        /// </summary>
        public void Back()
        {
            _state = STATE.STAGE;
            //モード選択の非表示
            _modeUIParent.SetActive(false);
            //初期ボタン
            StartButton[(int)_state].Select();
        }

        /// <summary>
        /// ステージシーンをロードする
        /// </summary>
        public void StageLoad()
        {
            GameController.Instance.AudioShotPlay(_submitAudio);//効果音
            Stage.StageController.Data = StageData._data[StageNo];//ステージデータを送る
            SceneManager.LoadScene("Stage");//ステージへ移動
        }

        /// <summary>
        /// ステージ作成をロードする
        /// </summary>
        public void CreateLoad()
        {
            GameController.Instance.AudioShotPlay(_submitAudio);//効果音
            StageCreate.StageCreateController._data = StageData._data[StageNo];//ステージデータを送る
            SceneManager.LoadScene("StageCreate");//ステージ作成へ移動
        }
    }
}
