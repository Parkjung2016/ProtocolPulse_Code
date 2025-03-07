using FMOD.Studio;
using FMODUnity;

public class FMODSoundManager
{
    private Bus _mainBus;
    private Bus _musicBus;
    private Bus _sfxBus;


    private readonly string btnCLickPath = "event:/ButtonClick";

    public FMODSoundManager()
    {
        _mainBus = RuntimeManager.GetBus("bus:/Main");
        _musicBus = RuntimeManager.GetBus("bus:/Main/Music");
        _sfxBus = RuntimeManager.GetBus("bus:/Main/SFX");
    }

    public void SetPaused(bool isPaused)
    {
        _mainBus.setPaused(isPaused);
    }

    public void SetMusicVolume(float volume)
    {
        _musicBus.setVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        _sfxBus.setVolume(volume);
    }

    public void PlayButtonClickSound()
    {
        RuntimeManager.PlayOneShot(btnCLickPath);
    }
}