using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflection.Stage
{
    public class Stage : MonoBehaviour
    {
        public static Stage Instance;
        private void Awake() { Instance = this; }

        [SerializeField, Tooltip("ステージ素材")] private GameObject[] mapItem = null;

        private Vector3 _fragPos = Vector3.zero;

        Data _data;

        /// <summary>
        /// ステージ作成
        /// </summary>
        public void StageCreate(Data data)
        {
            _data = data;
            //オブジェクトの配置
            for (int x = 0; x < 20; x++)
            {
                for (int z = 0; z < 20; z++)
                {
                    if (data.Stage[x, z] != (int)StageNo.Non && data.Stage[x, z] != (int)StageNo.Frag)
                    {
                        Instantiate(mapItem[data.Stage[x, z] - 1], new Vector3(9.5f - Mathf.Abs(x - 19), 0, 9.5f - z), Quaternion.identity).transform.SetParent(this.transform);
                    }
                    else if (data.Stage[x, z] == (int)StageNo.Frag)
                    {
                        _fragPos = new Vector3(9.5f - Mathf.Abs(x - 19), 0, 9.5f - z);
                    }
                }
            }
        }

        public Vector3 PlayerPosGet() { return _fragPos; }

        public class Line { public Vector3 start; public Vector3 end; public Vector3 normal;public bool non = false; }
        List<Line> lines = new List<Line>();
        /// <summary>
        /// ステージの接触判定を行うLineの取得
        /// </summary>
        /// <param name="color">プレイヤーの色</param>
        public void StageCollisionGet(Color color)
        {
            lines = new List<Line>();//Lineの初期化
            ///縦線
            for (int x = 0; x < 20; x++)
            {
                //左
                Vector3 startPos = Vector3.zero; Vector3 endPos;
                for (int z = 0; z < 20; z++)
                {
                    //ラインを設定できるか確認
                    if (DataChack(_data.Stage[x, z], color) && x != 0 && !DataChack(_data.Stage[x - 1, z], color))
                    {
                        Vector3 center = new Vector3(9.5f - Mathf.Abs(x - 19), 0, 9.5f - z);
                        center += Vector3.right * -0.5f + Vector3.forward * 0.5f;//左上
                        if (startPos == Vector3.zero)
                            startPos = center;
                    }
                    else if (startPos != Vector3.zero)
                    {
                        Vector3 center = new Vector3(9.5f - Mathf.Abs(x - 19), 0, 9.5f - z);
                        center += Vector3.right * -0.5f + Vector3.forward * 0.5f;//左上
                        endPos = center;

                        lines.Add(new Line { start = startPos, end = endPos, normal = Vector3.left });
                        startPos = Vector3.zero;//Startの初期化
                    }
                }
                if (startPos != Vector3.zero)
                {
                    Vector3 center = new Vector3(9.5f - Mathf.Abs(x - 19), 0, 9.5f - 19);
                    center += Vector3.right * -0.5f + Vector3.forward * -0.5f;
                    endPos = center;

                    lines.Add(new Line { start = startPos, end = endPos, normal = Vector3.left });
                    Debug.DrawLine(startPos, endPos, Color.green);
                    Debug.DrawRay(startPos, Vector3.up * 2, Color.red);
                    startPos = Vector3.zero;
                }

                //右
                startPos = Vector3.zero; endPos = Vector3.zero;
                for (int z = 0; z < 20; z++)
                {
                    if (DataChack(_data.Stage[x, z], color) && x != 19 && !DataChack(_data.Stage[x + 1, z], color))
                    {
                        Vector3 center = new Vector3(9.5f - Mathf.Abs(x - 19), 0, 9.5f - z);
                        //右上
                        center += Vector3.right * 0.5f + Vector3.forward * 0.5f;
                        if (startPos == Vector3.zero)
                            startPos = center;
                    }
                    else if (startPos != Vector3.zero)
                    {
                        Vector3 center = new Vector3(9.5f - Mathf.Abs(x - 19), 0, 9.5f - z);
                        center += Vector3.right * 0.5f + Vector3.forward * 0.5f;
                        endPos = center;

                        lines.Add(new Line { start = startPos, end = endPos, normal = Vector3.right });
                        startPos = Vector3.zero;
                    }
                }
                if (startPos != Vector3.zero)
                {
                    Vector3 center = new Vector3(9.5f - Mathf.Abs(x - 19), 0, 9.5f - 19);
                    center += Vector3.right * 0.5f + Vector3.forward * -0.5f;
                    endPos = center;

                    lines.Add(new Line { start = startPos, end = endPos, normal = Vector3.right });
                    startPos = Vector3.zero;
                }
            }

            ///横線
            for (int z = 0; z < 20; z++)
            {
                //上
                Vector3 startPos = Vector3.zero; Vector3 endPos = Vector3.zero;
                for (int x = 0; x < 20; x++)
                {
                    if (DataChack(_data.Stage[x, z], color) && z != 0 && !DataChack(_data.Stage[x, z - 1], color))
                    {
                        Vector3 center = new Vector3(9.5f - Mathf.Abs(x - 19), 0, 9.5f - z);
                        center += Vector3.right * -0.5f + Vector3.forward * 0.5f;

                        if (startPos == Vector3.zero)
                            startPos = center;
                    }
                    else if (startPos != Vector3.zero)
                    {
                        Vector3 center = new Vector3(9.5f - Mathf.Abs(x - 19), 0, 9.5f - z);
                        center += Vector3.right * -0.5f + Vector3.forward * 0.5f;
                        endPos = center;

                        lines.Add(new Line { start = startPos, end = endPos, normal = Vector3.forward });
                        startPos = Vector3.zero;
                    }
                }
                if (startPos != Vector3.zero)
                {
                    Vector3 center = new Vector3(9.5f - Mathf.Abs(10 - 19), 0, 9.5f - z);
                    center += Vector3.right * -0.5f + Vector3.forward * 0.5f;
                    endPos = center;

                    lines.Add(new Line { start = startPos, end = endPos, normal = Vector3.forward });
                    startPos = Vector3.zero;
                }

                //下
                startPos = Vector3.zero; endPos = Vector3.zero;

                for (int x = 0; x < 20; x++)
                {
                    if (DataChack(_data.Stage[x, z], color) && z != 19 && !DataChack(_data.Stage[x, z + 1], color))
                    {
                        Vector3 center = new Vector3(9.5f - Mathf.Abs(x - 19), 0, 9.5f - z);
                        center += Vector3.right * -0.5f + Vector3.forward * -0.5f;

                        if (startPos == Vector3.zero)
                            startPos = center;
                    }
                    else if (startPos != Vector3.zero)
                    {
                        Vector3 center = new Vector3(9.5f - Mathf.Abs(x - 19), 0, 9.5f - z);
                        center += Vector3.right * -0.5f + Vector3.forward * -0.5f;
                        endPos = center;

                        lines.Add(new Line { start = startPos, end = endPos, normal = Vector3.back });
                        startPos = Vector3.zero;
                    }
                }
                if (startPos != Vector3.zero)
                {
                    Vector3 center = new Vector3(9.5f - Mathf.Abs(19 - 19), 0, 9.5f - z);
                    center += Vector3.right * -0.5f + Vector3.forward * -0.5f;
                    endPos = center;

                    lines.Add(new Line { start = startPos, end = endPos, normal = Vector3.back });
                    startPos = Vector3.zero;
                }
            }

            //上
            lines.Add(new Line
            {
                start = new Vector3(-10, 0, 10),//左上
                end = new Vector3(10, 0, 10),//右上
                normal = Vector3.back,
                non = true
            }); 
            //右
            lines.Add(new Line
            {
                start = new Vector3(10, 0, 10),//右上
                end = new Vector3(10, 0, -10),//右下
                normal = Vector3.left,
                non = true
            });
            //下
            lines.Add(new Line
            {
                start = new Vector3(10, 0, -10),//右下
                end = new Vector3(-10, 0, -10),//左下
                normal = Vector3.forward,
                non = true
            });
            //左
            lines.Add(new Line
            {
                start = new Vector3(-10, 0, -10),//左下
                end = new Vector3(-10, 0, 10),//左上
                normal = Vector3.right,
                non = true
            });
        }

        /// <summary>
        /// 線を設定できるか
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        private bool DataChack(int no, Color color)
        {
            int n = -1;
            if (color == Color.red) n = (int)StageNo.Grass_Red;
            if (color == Color.blue) n = (int)StageNo.Grass_Blue;
            if (color == Color.green) n = (int)StageNo.Grass_Green;

            if (no != (int)StageNo.Non && no != (int)StageNo.Frag && no != (int)StageNo.Star && no != n)
                return true;
            return false;
        }

        /// <summary>
        /// playerとlinesの接触判定を返す
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Line Contact(Transform player)
        {
            foreach (Line line in lines)
            {
                if (LineContact(line, player.position))
                {
                    return line;
                }
            }
            return new Line();
        }

        /// <summary>
        /// pos から最短距離のオブジェクト
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int MinObject(Vector3 pos)
        {
            float min = float.MaxValue;
            int minNo = 0;
            for (int x = 0; x < 20; x++)
            {
                for (int z = 0; z < 20; z++)
                {
                    if (_data.Stage[x, z] != (int)StageNo.Frag &&
                        _data.Stage[x, z] != (int)StageNo.Star &&
                        _data.Stage[x, z] != (int)StageNo.Non)
                    {
                        Vector3 p = new Vector3(9.5f - Mathf.Abs(x - 19), 0, 9.5f - z);

                        if (Vector3.Distance(p, pos) < min)
                        {
                            minNo = _data.Stage[x, z];
                            min = Vector3.Distance(p, pos);
                        }
                    }
                }
            }


            return minNo;
        }

        /// <summary>
        /// 線と円の当たり判定
        /// </summary>
        /// <param name="line"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool LineContact(Line line, Vector3 player)
        {
            Vector3 ap = player - line.start;
            Vector3 ab = line.end - line.start;
            Vector3 bp = player - line.end;
            Vector3 normalAB = ab.normalized;

            float lenAX = Vector3.Dot(normalAB, ap);
            float shortestDistance = 0.0f;

            if (lenAX < 0)
                shortestDistance = ap.magnitude;
            else if (lenAX > ab.magnitude)
                shortestDistance = bp.magnitude;
            else
                shortestDistance = Mathf.Abs(Vector3.Cross(normalAB, ap).magnitude);
            return shortestDistance < 0.5f;
        }
    }
}