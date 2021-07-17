using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Reflection
{
    [System.Serializable]
    public class Data
    {
        public int No;          //ステージ番号
        public int Bounce;      //反射回数
        public int Life;        //生成回数
        public int Star;        //★の数
        public int[,] Stage;    //ステージ番号
        public Sprite sprite;   //ステージ画像

        /// <summary>
        /// ステージ画像の生成
        /// </summary>
        /// <param name="textures">番号に対応したテクスチャ</param>
        public void SpriteCreate(Texture2D[] textures)
        {
            //テクスチャの作成
            Texture2D texture = new Texture2D(400, 400, TextureFormat.RGBA32, false);

            for (int w = 0; w < texture.width; w++)//縦
            {
                for (int h = 0; h < texture.height; h++)//横
                {
                    if (Stage[w / 20, h / 20] != 0)
                        texture.SetPixel(w, Mathf.Abs(h - texture.height), textures[Stage[w / 20, h / 20] - 1].GetPixel((w % 20), (h % 20)));
                    else
                        texture.SetPixel(w, Mathf.Abs(h - texture.height), Color.grey);
                }
            }

            texture.Apply();
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }

    /// <summary>
    /// ステージ番号
    /// </summary>
    public enum StageNo
    {
        Non = 0,            //何も置かない
        Nomal = 1,          //ノーマル
        Color_Red = 2,      //赤色
        Color_Blue = 3,     //青色
        Color_Green = 4,    //緑色
        Goal_Red = 5,       //赤色ゴール
        Goal_Blue = 6,      //青色ゴール
        Goal_Green = 7,     //緑色ゴール
        Grass_Red = 8,      //赤色ガラス
        Grass_Blue = 9,     //青色ガラス
        Grass_Green = 10,   //緑色ガラス
        Star = 11,          //☆
        Frag = 12           //Start
    }

    public class StageData : MonoBehaviour
    {
        //ステージのオブジェクトのテクスチャ
        [SerializeField] public Texture2D[] _mapTextures = null;

        public static List<Data> _data = new List<Data>();

        private void Awake()
        {
            /*テキストデータからマップデータの取得*/
            _data.Clear();//Dataの初期化

            for (int i = 0; i < 10; i++)
            {
                string filePath = Application.dataPath + "/Resources/" + "Stage" + "_" + i + ".txt";

                if (!File.Exists(filePath))
                    StageFileCreate(filePath,i);//ファイルの生成

                //textDataをDataに変更する
                _data.Add(DataChange_TextToData(File.ReadAllLines(filePath)));

            }
        }

        /// <summary>
        /// file'path'が存在しないときにファイルの生成と書き込み
        /// </summary>
        private void StageFileCreate(string path,int No)
        {
            List<string> text = new List<string>();
            text.Add(No.ToString());//StageNo
            text.Add("0");//Life
            text.Add("0");//Bounce
            text.Add("0");//Star

            //ステージ番号
            for (int n1 = 4; n1 < 24; n1++)
            {
                List<string> te = new List<string>();
                for (int n2 = 0; n2 < 20; n2++)
                {
                    if (n1 == 4 && n2 == 0)
                        te.Add(((int)StageNo.Frag).ToString());
                    else
                        te.Add(((int)StageNo.Non).ToString());
                }
                text.Add(string.Join(",", te.ToArray()));
            }

            File.WriteAllLines(path, text.ToArray());

        }

        /// <summary>
        /// データをテキストからデータに変更
        /// </summary>
        private Data DataChange_TextToData(string[] text)
        {
            Data data = new Data
            {
                No = int.Parse(text[0]),       //ステージ番号
                Life = int.Parse(text[1]),     //生成回数
                Bounce = int.Parse(text[2]),   //反射回数
                Star = int.Parse(text[3])     //星
            };

            //ステージの記入
            int[,] stage = new int[20, 20];
            for (int n1 = 4; n1 < 4 + 20; n1++)
            {
                string[] part = text[n1].Split(',');
                for (int n2 = 0; n2 < 20; n2++)
                {
                    stage[n1 - 4, n2] = int.Parse(part[n2]);
                }
            }
            data.Stage = stage;

            //spriteの作成
            data.SpriteCreate(_mapTextures);
            return data;
        }

        /// <summary>
        /// データをテキストにします
        /// </summary>
        public void Data_to_Text(Data data)
        {
            /*指定したパスにテキストのデータを入れます*/
            string filePath = Application.dataPath + "/Resources/" + "Stage" + "_" + data.No.ToString() + ".txt";

            var file = File.Create(filePath);//ファイルを作成します
            file.Close();//作成したファイルを書き込み用に閉じます

            List<string> text = new List<string>();
            text.Add(data.No.ToString());         //ステージ番号
            text.Add(data.Life.ToString());       //生成回数
            text.Add(data.Bounce.ToString());     //反射回数
            text.Add(data.Star.ToString());       //星

            ///*ステージデータ*/
            for (int n1 = 4; n1 < 4 + 20; n1++)
            {
                List<string> te = new List<string>();
                for (int n2 = 0; n2 < 20; n2++)
                    te.Add(data.Stage[n1 - 4, n2].ToString());

                text.Add(string.Join(",", te.ToArray()));
            }

            File.WriteAllLines(filePath, text.ToArray());//書き込みをします
        }
    }
}
