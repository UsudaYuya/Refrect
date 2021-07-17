using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflection.Stage
{
    public class Star : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                GameObject.FindObjectOfType<StageController>().Star();
                Destroy(this.gameObject);
            }
        }
    }
}
