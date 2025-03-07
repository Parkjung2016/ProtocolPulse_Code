using FMOD.Studio;
using FMODUnity;
using PJH.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace PJH.Scene
{
    public class TitleScene : BaseScene
    {
        [SerializeField] private EventReference _titleBGM;
        public EventInstance titleInstance;

        protected override void Awake()
        {
            Managers.FMODSound.SetSFXVolume(1);
            Managers.FMODSound.SetMusicVolume(1);
            titleInstance = RuntimeManager.CreateInstance(_titleBGM);
            titleInstance.start();
        }

        public void InputStartBtn()
        {
            Managers.FMODSound.PlayButtonClickSound();

            titleInstance.stop(STOP_MODE.ALLOWFADEOUT);
            titleInstance.release();
            FadeManager.FadeIn(1, () => { SceneManager.LoadScene(1); });
        }


        public void InputExitBtn()
        {
            Managers.FMODSound.PlayButtonClickSound();
            titleInstance.stop(STOP_MODE.IMMEDIATE);
            titleInstance.release();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}