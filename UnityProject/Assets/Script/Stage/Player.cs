using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reflection.Stage
{
    public class Player : MonoBehaviour
    {
        private readonly float SPEED = 8.0f;

        private Vector3 _angle;//移動方向
        private Vector3 _normal;//以前接触した法線
        private bool _play;

        Rigidbody _rigid;
        LineRenderer _line;
        private void Start()
        {
            _rigid = this.GetComponent<Rigidbody>();
            _line = this.GetComponent<LineRenderer>();
            _play = false;
            this.GetComponent<MeshRenderer>().material.color = Color.white;
        }

        private void Update()
        {
            if (_play)
            {
                this._rigid.velocity = _angle.normalized * SPEED;//移動

                Stage.Line line = Stage.Instance.Contact(this.transform);//接触判定

                if (line.normal != Vector3.zero && _normal != line.normal)
                {
                    int no = -1;
                    if (!line.non)
                    {
                        //接触したオブジェクトの番号
                        no = Stage.Instance.MinObject(this.transform.position);

                        //色変更
                        if (no == (int)StageNo.Color_Red) ColorChange(Color.red);
                        if (no == (int)StageNo.Color_Blue) ColorChange(Color.blue);
                        if (no == (int)StageNo.Color_Green) ColorChange(Color.green);
                    }
                    GameObject.FindObjectOfType<StageController>().Bounce
                        (no, this.GetComponent<MeshRenderer>().material.color);//反射処理

                    _normal = line.normal;//反射したときのNomalの記録
                    _angle = Vector3.Reflect(_angle, line.normal);
                }
                return;
            }

            //発射方向
            if (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) != Vector3.zero)
                _angle = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            //移動ラインの表示
            _line.positionCount = 2;
            _line.SetPosition(0, this.transform.position);
            _line.SetPosition(1, this.transform.position + _angle * 5.0f);

            //決定キー離したとき
            if (Input.GetButtonUp("Submit") && _angle != Vector3.zero)
            {
                _rigid.AddForce(_angle * 4);
                _play = true;
            }
        }

        /// <summary>
        /// 色の変更
        /// </summary>
        /// <param name="color"></param>
        private void ColorChange(Color color)
        {
            this.GetComponent<MeshRenderer>().material.color = color;
            Stage.Instance.StageCollisionGet(color);
        }
    }
}