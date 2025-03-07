using UnityEngine;

namespace PJH.Manager
{
    public class PlayerManager
    {
        private Player _player;

        public Player Player
        {
            get
            {
                if (!_player)
                {
                    _player = Object.FindAnyObjectByType<Player>();
                }

                return _player;
            }
        }
    }
}