using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflection.StageCreate
{
    public class UICursor : MonoBehaviour
    {
        [Header("コントローラーでの移動速度")]
        [SerializeField] private float _controllerCursorSpeed = 10.0f;

        [Header("マウスを動かす最大高さ")]
        [SerializeField] private float _cursorHeight = 5.0f;
        [Header("マウスを動かす横幅")]
        [SerializeField] private float _cursorWight = 5.0f;

        RectTransform _rect;
        // Start is called before the first frame update
        void Start()
        {
            _rect = this.GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            float ver = Input.GetAxis("Vertical");
            float hor = Input.GetAxis("Horizontal");
            /*コントローラーの移動*/


            /*x軸移動*/
            if (hor > 0)
            {
                if (_rect.anchoredPosition.x < _cursorWight)
                    _rect.anchoredPosition += Vector2.right * hor * _controllerCursorSpeed * Time.deltaTime;
            }
            else
            {
                if (_rect.anchoredPosition.x > -_cursorWight)
                    _rect.anchoredPosition += Vector2.right * hor * _controllerCursorSpeed * Time.deltaTime;
            }

            /*y軸移動*/
            if (ver > 0)
            {
                if (_rect.anchoredPosition.y < _cursorHeight)
                    _rect.anchoredPosition += Vector2.up * ver * _controllerCursorSpeed * Time.deltaTime;
            }
            else
            {
                if (_rect.anchoredPosition.y > -_cursorHeight)
                    _rect.anchoredPosition += Vector2.up * ver * _controllerCursorSpeed * Time.deltaTime;
            }
        }
    }
}