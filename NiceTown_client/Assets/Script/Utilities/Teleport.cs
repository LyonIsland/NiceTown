using UnityEngine;
using System.Collections;

namespace Client.Transition
{
    public class CoroutineExample : MonoBehaviour
    {
        private string sceneName;
        public Vector3 positionToGo;
        private void Awake()
        {
            sceneName = transform.name;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                EventHandler.CallTransitionEvent(sceneName, positionToGo);
            }
        }
    }
}

