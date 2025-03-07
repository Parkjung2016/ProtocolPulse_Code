using PJH.Core;
using PJH.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PJH.Manager
{
    public class SceneManagerEx
    {
        private BaseScene _currentScene;

        public BaseScene CurrentScene
        {
            get
            {
                if (_currentScene == null)
                    _currentScene = Object.FindAnyObjectByType<BaseScene>();
                return _currentScene;
            }
        }


        public void LoadScene(Define.EScene type)
        {
            SceneManager.LoadScene(type.ToString());
        }
    }
}