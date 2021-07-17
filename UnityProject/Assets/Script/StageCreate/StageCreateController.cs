using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Reflection.StageCreate
{
    public class StageCreateController : MonoBehaviour
    {
        //変更するデータ
        public static Data _data;

        //Base = 全体処理 Create = アイテムを置く Save=セーブ処理 Back = SeleCtStageに戻る
        private enum STATE { Base = 0, Save = 1, Back = 2, Create = 3 }
        private STATE _state = STATE.Base;

        [Header("初期ボタン　0: Base 1:Save 2: Back")]
        [SerializeField] private Button[] _startButton = null;

        [Header("セーブするUIの親")]
        [SerializeField] private GameObject _saveParent = null;
        [Header("Titleに戻るUIの親")]
        [SerializeField] private GameObject _backParent = null;


        [Header("オブジェクト取得用カーソル")]
        [SerializeField] private RectTransform _cursor = null;
        [Header("ステージ設定用カーソル")]
        [SerializeField] private RectTransform _stageCursor = null;
        [Header("ステージのUI")]
        [SerializeField] private GameObject _stageUI = null;

        [Header("生成回数のテキスト")]
        [SerializeField] private Text _lifeText = null;
        [Header("反射回数のテキスト")]
        [SerializeField] private Text _bounceText = null;

        [Header("決定効果音")]
        [SerializeField] private AudioClip _submitAudio = null;
        [Header("セレクト効果音")]
        [SerializeField] private AudioClip _selectAudio = null;
        [Header("アイテム設置音")]
        [SerializeField] private AudioClip _putAudio = null;

        private int _playItemNo = 0;//設定するアイテム番号
        private bool operation = true;//操作を行うかどうか

        AudioSource _audio;
        StageData _stageData;
        UICursor _uiCursor;//カーソル

        private void Start()
        {
            //データが存在しない場合データを取得する
            if (_data == null)
                _data = StageData._data[0];

            _audio = Camera.main.GetComponent<AudioSource>();
            _stageData = GameObject.FindObjectOfType<StageData>();
            _uiCursor = _stageCursor.GetComponent<UICursor>();
            _uiCursor.enabled = false;//カーソルが動かないようにする

            _saveParent.SetActive(false);//セーブUIの非表示
            _backParent.SetActive(false);//SelectStageBackUIの非表示

            _stageUI.GetComponent<Image>().sprite = _data.sprite;//Stageの表示

            //初期ボタンを選択状態にする
            _startButton[0].Select();
        }

        private void Update()
        {
            //ボタンが選択されていない場合
            if (EventSystem.current.currentSelectedGameObject == null && _state != STATE.Create)
            {
                _startButton[(int)_state].Select();//初期ボタンを選択する
                return;
            }

            //ステージ作成状態の場合
            if (_state == STATE.Create)
            {
                if (Input.GetButtonUp("Submit"))
                    operation = true;//操作可能状態に変更する

                if (operation)
                {
                    if (Input.GetButton("Submit"))
                        StageItemPut();//アイテムを置く

                    //オブジェクト選択画面に戻る
                    if (Input.GetButtonDown("Cancel"))
                    {
                        _state = STATE.Base;//オブジェクト選択状態に戻る
                        _uiCursor.enabled = false;//ボタンのカーソルが動かないようにする

                        //ボタンの処理を行わないようにする
                        foreach (Button button in GameObject.FindObjectsOfType<Button>())
                            button.enabled = true;
                    }
                }
            }
            else
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

        /// <summary>
        /// アイテムの設置
        /// </summary>
        private void StageItemPut()
        {
            RectTransform rect = _stageUI.GetComponent<RectTransform>();
            //カーソルとステージ画像の距離
            Vector2 dis = (Vector2)_stageUI.transform.position - (Vector2)_uiCursor.transform.position;

            //x軸
            float dis_x = Mathf.Abs(dis.x - (rect.sizeDelta.x / 2));
            int w = (int)dis_x / ((int)rect.sizeDelta.x / 20);

            //y軸
            float dis_y = Mathf.Abs(dis.y + (rect.sizeDelta.y / 2));
            int h = (int)dis_y / ((int)rect.sizeDelta.y / 20);

            ///
            ///_data[w,h] = ItemNoに変更する
            ///ItemNoが旗の場合旗の移動　☆の場合は数を取得して三つ以内なら設置以上なら設置しない
            ///旗の位置には何も設置を行わない

            //設置位置が旗の場合と設置するオブジェクトが変わらない場合設置処理を終了
            if (_data.Stage[w, h] == (int)StageNo.Frag || _data.Stage[w, h] == _playItemNo)
                return;

            //旗の設置を行った場合
            if (_playItemNo == (int)StageNo.Frag)
            {
                //前に置いていた旗の削除
                for (int n1 = 0; n1 < 20; n1++)
                    for (int n2 = 0; n2 < 20; n2++)
                        if (_playItemNo == _data.Stage[n1, n2])
                            _data.Stage[n1, n2] = (int)StageNo.Non;
            }

            _audio.PlayOneShot(_putAudio);//設置音
            _data.Stage[w, h] = _playItemNo;//番号の設定


            //☆の数が多かった場合設置を行わない
            if (_playItemNo == (int)StageNo.Star)
            {
                int starCount = 0;
                //☆の数を確認
                for (int n1 = 0; n1 < 20; n1++)
                    for (int n2 = 0; n2 < 20; n2++)
                        if ((int)StageNo.Star == _data.Stage[n1, n2])
                            starCount++;

                //☆の数が多かった場合☆を削除する
                if (starCount >= 4)
                    _data.Stage[w, h] = (int)StageNo.Non;//☆の削除
            }
            _data.SpriteCreate(_stageData._mapTextures);//Spriteの変更
            _stageUI.GetComponent<Image>().sprite = _data.sprite;//ImageのSpriteの更新
        }


        /// <summary>
        /// ボタンの選択変更時の効果音
        /// </summary>
        public void SelectButtonChange()
        {
            _audio.PlayOneShot(_selectAudio);
        }

        #region Base or Create

        /// <summary>
        /// 設置するオブジェクト番号を取得
        /// </summary>
        public void PutObjectGet(int no)
        {
            _audio.PlayOneShot(_submitAudio);

            _playItemNo = no;//番号の設定

            /*アイテムを置く状態に変更する*/
            _state = STATE.Create;

            _uiCursor.enabled = true;//カーソルが動くようにする
            _stageUI.GetComponent<Image>().sprite = _data.sprite;//画像の変更
            //ボタンの処理を行わないようにする
            foreach (Button button in GameObject.FindObjectsOfType<Button>())
                button.enabled = false;

            operation = false;
        }

        /// <summary>
        /// SaveStateに変更する
        /// </summary>
        public void SaveStart()
        {
            _audio.PlayOneShot(_submitAudio);
            _state = STATE.Save;//stateの変更

            //SaveUIの表示
            _saveParent.SetActive(true);
            _startButton[(int)_state].Select();//開始ボタンの設定

            //生成回数の表示
            _lifeText.text = _data.Life.ToString();
            //反射回数の表示
            _bounceText.text = _data.Bounce.ToString();
        }

        /// <summary>
        /// SelectStageに戻る
        /// </summary>
        public void BackStart()
        {
            _audio.PlayOneShot(_submitAudio);
            //stateの変更
            _state = STATE.Back;
            //ボタンの設定
            _startButton[(int)_state].Select();
            //BackImageの表示
            _backParent.SetActive(true);
        }
        #endregion

        #region　セーブ
        /// <summary>
        /// 生成回数の追加
        /// </summary>
        /// <param name="life"></param>
        public void LifeAdd(int life)
        {
            if (_data.Life + life > 7)
            {
                _audio.PlayOneShot(_selectAudio);
                return;
            }
            _audio.PlayOneShot(_submitAudio);
            _data.Life += life;//生成回数の変更
            _lifeText.text = _data.Life.ToString();
        }

        /// <summary>
        /// 反射回数の追加
        /// </summary>
        /// <param name="bounce"></param>
        public void BounceAdd(int bounce)
        {
            if (_data.Bounce + bounce > 7)
            {
                _audio.PlayOneShot(_selectAudio);
                return;
            }
            _audio.PlayOneShot(_submitAudio);
            _data.Bounce += bounce;//反射回数の変更
            _bounceText.text = _data.Bounce.ToString();
        }

        /// <summary>
        /// セーブ
        /// </summary>
        public void Save()
        {
            ///セーブできるか確認

            int starCount = 0;//星の数
            int goalCount = 0;
            for (int n1 = 0; n1 < 20; n1++)
            {
                for (int n2 = 0; n2 < 20; n2++)
                {
                    if ((int)StageNo.Star == _data.Stage[n1, n2])
                        starCount++;
                    else if ((int)StageNo.Frag == _data.Stage[n1, n2])
                        goalCount++;
                }
            }

            //Save条件を満たしていない場合
            if (starCount != 3 || goalCount != 1)
            {
                _audio.PlayOneShot(_selectAudio);
                Debug.Log("starCount" + " = " + starCount+","+"GoalCount" +" = " + goalCount);
                return;
            }



            //決定効果音
            GameController.Instance.AudioShotPlay(_submitAudio);
            ColorChange(Color.red);
            StageData._data[_data.No] = _data;//データの更新

            ColorChange(Color.green);
            _stageData.Data_to_Text(_data);//テキストに更新
            SceneManager.LoadScene("StageSelect");//ステージを移動する
        }
        [SerializeField] private Image image = null;
        private void ColorChange(Color color)
        {
            image.color = color;
        }
   
   

        /// <summary>
        /// 戻る
        /// </summary>
        public void Back()
        {
            _state = STATE.Base;
            _audio.PlayOneShot(_submitAudio);
            _saveParent.SetActive(false);
            _startButton[(int)_state].Select();
        }
        #endregion

        #region　戻る処理

        /// <summary>
        /// セレクトステージに戻る
        /// </summary>
        public void YES()
        {
            GameController.Instance.AudioShotPlay(_submitAudio);
            SceneManager.LoadScene("StageSelect");
        }

        /// <summary>
        /// Base状態に変更する
        /// </summary>
        public void NO()
        {
            _state = STATE.Base;
            _audio.PlayOneShot(_submitAudio);
            _backParent.SetActive(false);
            _startButton[(int)_state].Select();
        }

        #endregion
    }
}
