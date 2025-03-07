using System;
using UnityEngine;

namespace PJH.Manager
{
    public class LevelManager
    {
        public void Init()
        {
            NextLevelUpXP = 2;
            CurrentXP = 0;
            CurrentLevel = 0;
        }

        public event Action<int> OnChangedXP;
        public event Action<int> OnChangedNextLevelUpXP;
        public event Action<int> OnChangedLevel;
        public event Action OnLevelUp;
        private int _currentLevel;
        private int _currentXP;
        private int _nextLevelUpXP;

        public int CurrentXP
        {
            get => _currentXP;
            set
            {
                _currentXP = value;
                if (_currentXP >= _nextLevelUpXP)
                {
                    _currentXP = 0;
                    NextLevelUpXP = GetExponentialXP(_nextLevelUpXP, 1.5f);
                    CurrentLevel++;
                    OnLevelUp?.Invoke();
                }

                OnChangedXP?.Invoke(_currentXP);
            }
        }

        public int NextLevelUpXP
        {
            get => _nextLevelUpXP;
            set
            {
                _nextLevelUpXP = value;
                OnChangedNextLevelUpXP?.Invoke(_nextLevelUpXP);
            }
        }


        public int CurrentLevel
        {
            get => _currentLevel;
            set
            {
                _currentLevel = value;
                OnChangedLevel?.Invoke(value);
            }
        }


        public void AddXP(int value)
        {
            CurrentXP += value;
        }

        public static int GetExponentialXP(int baseXP, float growthRate)
        {
            return Mathf.RoundToInt(baseXP * growthRate);
        }
    }
}