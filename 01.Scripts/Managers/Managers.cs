namespace PJH.Manager
{
    public static class Managers
    {
        public static AddressableManager Addressable { get; } = new();
        public static SceneManagerEx Scene { get; } = new();
        public static PlayerManager PlayerManager { get; } = new();
        public static LevelManager LevelManager { get; } = new();
        public static FMODSoundManager FMODSound { get; } = new();
    }
}