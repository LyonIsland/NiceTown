using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client.Transition
{
    public class TransitionManager : MonoBehaviour
    {
        public string startSceneName = string.Empty;

        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }


        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }
        private void Start()
        {
            StartCoroutine(LoadSceneSetActive(startSceneName));
        }

        private void OnTransitionEvent(string targetScene, Vector3 targetPos)
        {
            StartCoroutine(Transition(targetScene, targetPos));
        }

        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.UnloadSceneAsync(SceneManager.sceneCount - 1);
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            SceneManager.SetActiveScene(newScene);
        }

        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            yield return LoadSceneSetActive(sceneName);
        }
    }
}

