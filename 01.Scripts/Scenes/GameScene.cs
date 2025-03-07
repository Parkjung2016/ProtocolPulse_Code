using System;
using PJH.Manager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PJH.Scene
{
    public class GameScene : BaseScene
    {
        public bool isGameStarted;
        [SerializeField] private Button _optionBtn;
        public bool IsPaused { get; private set; }
        public event Action<bool> OnPaused;

        protected override void Awake()
        {
            base.Awake();
            FadeManager.FadeOut(1.2f);

            _optionBtn.onClick.AddListener(async () =>
            {
                if (IsPaused) return;
                SetPaused(true);
                ClickOptionBtn();
            });
        }

        private void Start()
        {
            Managers.PlayerManager.Player.Input.EnablePlayerInput(false);
            Managers.LevelManager.Init();
        }

        public void SetPaused(bool isPaused)
        {
            if (isGameStarted)
                Managers.PlayerManager.Player.Input.EnablePlayerInput(!isPaused);
            IsPaused = isPaused;
            Time.timeScale = isPaused ? 0 : 1;
            Managers.FMODSound.SetPaused(isPaused);
            OnPaused?.Invoke(isPaused);
        }

        private void Update()
        {
            if (!isGameStarted && Keyboard.current.spaceKey.wasPressedThisFrame && !BeatTracker.Instance.isPlayingMusic)
            {
                isGameStarted = true;
                Enemy.KillCount = 0;
                Managers.PlayerManager.Player.Input.EnablePlayerInput(true);

                BeatTracker.Instance.SetMusicTrack(BeatTracker.Instance.eventToPlay);
                BeatTracker.Instance.PlayMusicTrack();
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Managers.FMODSound.PlayButtonClickSound();

                SetPaused(!IsPaused);
            }
        }

        public void ClickOptionBtn()
        {
            Managers.FMODSound.PlayButtonClickSound();
        }
    }
}