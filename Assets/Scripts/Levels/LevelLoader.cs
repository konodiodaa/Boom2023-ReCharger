using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace Levels{
    public class LevelLoader: MonoBehaviour{

        public string prefix = "Lv-";

        public CanvasGroup shade;

        private void Awake(){
            StartCoroutine(shade.Fade(0.2f));
        }

        public void LoadLevel(int id){
            SceneManager.LoadScene($"{prefix}{id.ToString()}", LoadSceneMode.Single);
        }

        public void LoadLevelWithTween(int id){
            StartCoroutine(TweenAndLoad(id));
        }

        private IEnumerator TweenAndLoad(int id){
            yield return shade.Emerge(0.2f);
            LoadLevel(id);
        }
    }
}