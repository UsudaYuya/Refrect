using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Reflection.Stage
{
    public class StageController : MonoBehaviour
    {
        //ステージセレクトで選んだデータ
        public static Data Data
        {
            get { return data; }
            set { data = value; }
        }
        private static Data data = null;

        private enum STATE { PLAY, CLEAR, FAILURE }//0:プレイ前,1:プレイ,2:クリアー,3:失敗
        private STATE _state;

        [SerializeField, Header("プレイヤーのPrefab")] private GameObject _playerPrefab = null;
        private int _bounce = 0;
        private int _life = 0;
        private int _starCount = 0;

        [SerializeField, Header("生成回数のImage")] private Image[] _lifeImage = null;
        [SerializeField, Header("反射回数のImage")] private Image[] _bounceImage = null;
        [SerializeField, Header("星のImage")] private Image[] _starImage= null;

        [SerializeField, Header("ステージ")] private Stage _stage = null;

        [SerializeField, Header("クリア時の星")] private Image[] _stars = null;

        [SerializeField, Header("成功UI")] private GameObject _clearUI = null;
        [SerializeField, Header("失敗UI")] private GameObject _failureUI = null;

        [SerializeField, Header("0:ReStart 1:NextStage 2:StageSelect ")]
        private Button[] _buttons = null;

        [SerializeField] private RectTransform _cursor = null;

        [SerializeField, Header("決定音")] private AudioClip _submit = null;
        [SerializeField, Header("失敗音")] private AudioClip _select = null;
        [SerializeField, Header("反射音")] private AudioClip _refrect = null;

        Player _player;

        AudioSource _audio;

        private void Start()
        {
            if (data == null)
                data = StageData._data[0];

            _audio = this.GetComponent<AudioSource>();

            Debug.Log(data.Stage[0, 0]);
            //ステージの生成
            _stage.StageCreate(data);

            //プレイヤーの生成
            _player = Instantiate(_playerPrefab, Stage.Instance.PlayerPosGet(), Quaternion.identity).GetComponent<Player>();

            //数値のリセット
            _bounce = data.Bounce;//反射回数
            _life = data.Life;  //生成回数

            //UIのリセット
            //Lifeの回数の表記
            for (int i = 0; i < _lifeImage.Length; i++)
            {
                //現在のLifeよりも多いか削減してたら減らす
                if (_life - 1 < i)
                    _lifeImage[i].gameObject.SetActive(false);
                else
                    _lifeImage[i].gameObject.SetActive(true);
            }

            //Bounceの回数表記
            for (int i = 0; i < _bounceImage.Length; i++)
            {
                //現在のLifeよりも多いか削減してたら減らす
                if (_bounce - 1 < i)
                    _bounceImage[i].gameObject.SetActive(false);
                else
                    _bounceImage[i].gameObject.SetActive(true);
            }

            Stage.Instance.StageCollisionGet(Color.white);
        }

        private void Update()
        {
            switch (_state)
            {
                case STATE.PLAY: Play(); break;
                case STATE.CLEAR: Clear(); break;
                case STATE.FAILURE: Failure(); break;
            }
        }

        /// <summary>
        /// 反射
        /// </summary>
        /// <param name="no"></param>
        /// <param name="color"></param>
        public void Bounce(int no, Color color)
        {
            _bounce--;

            _audio.PlayOneShot(_refrect);
            if (_bounce == -1)
            {
                //色変更
                if (no == (int)StageNo.Goal_Red) if (color == Color.red) Clear();
                if (no == (int)StageNo.Goal_Blue) if (color == Color.blue) Clear();
                if (no == (int)StageNo.Goal_Green) if (color == Color.green) Clear();

                Destroy(_player.gameObject);
            }

            //Bounceの回数表記
            for (int i = 0; i < _bounceImage.Length; i++)
            {
                //現在のLifeよりも多いか削減してたら減らす
                if (_bounce - 1 < i)
                    _bounceImage[i].gameObject.SetActive(false);
                else
                    _bounceImage[i].gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 星の数
        /// </summary>
        public void Star()
        {
            _starCount++;

            for (int i = 0; i < _starImage.Length; i++)
            {
                //現在のLifeよりも多いか削減してたら減らす
                if (_starCount - 1 < i)
                    _starImage[i].gameObject.SetActive(false);
                else
                    _starImage[i].gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// プレイ中
        /// </summary>
        private void Play()
        {
            //プレイヤーが存在しない場合
            if (_player == null)
            {
                //プレイヤーの生成
                _player = Instantiate(_playerPrefab, _stage.PlayerPosGet(), Quaternion.identity).GetComponent<Player>();
                //生成回数の変更
                _life--;
                //生成回数の回数表記
                for (int i = 0; i < _lifeImage.Length; i++)
                {
                    //現在のLifeよりも多いか削減してたら減らす
                    if (_life - 1 < i)
                        _lifeImage[i].gameObject.SetActive(false);
                    else
                        _lifeImage[i].gameObject.SetActive(true);
                }

                //反射回数の変更
                _bounce = data.Bounce;
                //Bounceの回数表記
                for (int i = 0; i < _bounceImage.Length; i++)
                {
                    //現在のLifeよりも多いか削減してたら減らす
                    if (_bounce - 1 < i)
                        _bounceImage[i].gameObject.SetActive(false);
                    else
                        _bounceImage[i].gameObject.SetActive(true);
                }

                if (_life == -1)
                    _state = STATE.FAILURE;
            }
        }

        /// <summary>
        /// クリア
        /// </summary>
        private void Clear()
        {
            if (!_clearUI.activeSelf)
            {
                _state = STATE.CLEAR;
                _clearUI.SetActive(true);//UIの表示

                _cursor.gameObject.SetActive(true);
                //次のステージが存在しない場合NextStageを表示しない
                if (data.No + 1 > StageData._data.Count)
                {
                    _buttons[0].gameObject.SetActive(true);//ReStart
                    _buttons[2].gameObject.SetActive(true);//StageSelect
                }
                else
                {
                    _buttons[1].gameObject.SetActive(true);//nextStage
                    _buttons[2].gameObject.SetActive(true);//StageSelect
                }

                for (int i = 0; i < _starImage.Length; i++)
                {
                    //現在のLifeよりも多いか削減してたら減らす
                    if (_starCount - 1 < i)
                        _stars[i].gameObject.SetActive(false);
                    else
                        _stars[i].gameObject.SetActive(true);
                }

                StageData._data[data.No].Star = data.Star;//☆の数の変

                if (_starCount >= data.Star)
                {
                    Debug.Log("star");
                    data.Star = _starCount;
                    StageData._data[data.No] = data;//Dataの変更
                    GameObject.FindObjectOfType<StageData>().Data_to_Text(data);//Dataのセーブ
                }
            }

            //ボタンが選択されていない場合
            if (EventSystem.current.currentSelectedGameObject == null)
                _buttons[2].Select();//初期ボタンを選択する

            //ボタンの位置の設定
            _cursor.anchoredPosition =
                EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().anchoredPosition;
        }

        /// <summary>
        /// 失敗
        /// </summary>
        private void Failure()
        {
            if (!_failureUI.activeSelf)
            {
                _failureUI.SetActive(true);//UIの表示

                //次のステージが存在しない場合NextStageを表示しない
                _buttons[0].gameObject.SetActive(true);//ReStart
                _buttons[2].gameObject.SetActive(true);//StageSelect

                StageData._data[data.No].Star = data.Star;//☆の数の変
            }

            //ボタンが選択されていない場合
            if (EventSystem.current.currentSelectedGameObject == null)
                _buttons[2].Select();//初期ボタンを選択する

            //ボタンの位置の設定
            _cursor.anchoredPosition =
                EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().anchoredPosition;
        }

        //--------------------------------------
        //　　　　　　ボタンの処理
        //--------------------------------------

        /// <summary>
        /// 選択されているボタンの変更時
        /// </summary>
        public void SelectButtonChange()
        {
            _audio.PlayOneShot(_select);
        }

        /// <summary>
        /// 同じステージ
        /// </summary>
        public void ReStart()
        {
            GameController.Instance.AudioShotPlay(_submit);
            StageController.Data = StageData._data[data.No];
            SceneManager.LoadScene("Stage");
        }

        /// <summary>
        /// 次のステージ
        /// </summary>
        public void NextStage()
        {
            GameController.Instance.AudioShotPlay(_submit);
            StageController.Data = StageData._data[data.No + 1];
            SceneManager.LoadScene("Stage");
        }

        /// <summary>
        /// ステージセレクトへ移動
        /// </summary>
        public void StageSelect()
        {
            GameController.Instance.AudioShotPlay(_submit);
            SceneManager.LoadScene("StageSelect");
        }
    }
}