using Cysharp.Threading.Tasks;
using FMODUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BeatTracker : MonoSingleton<BeatTracker>
{
    [SerializeField] private EventReference _perfectBeatTrackSound;
    public EventReference eventToPlay;
    public bool isPlayingMusic = false;
    private int masterSampleRate;
    private double currentSamples = 0;
    private double currentTime = 0f;

    public float gameTime = 0;

    private ulong dspClock;

    private double beatInterval = 0f;

    private double tempoTrackDSPStartTime;
    public readonly List<Func<UniTask>> OnBeatMovementFunc = new(), OnBeatAttackFunc = new();
    public event Action OnPlayerMovement, OnEnemyMovement;
    public event Action OnBeatTiming;

    private double lastFixedBeatTime = -2;
    private double lastFixedBeatDSPTime = -2;
    private FMOD.Studio.PLAYBACK_STATE musicPlayState;
    private FMOD.Studio.PLAYBACK_STATE lastMusicPlayState;

    public event Action OnSuccessCheckInteract, OnFailureCheckInteract;

    [StructLayout(LayoutKind.Sequential)]
    public class TimelineInfo
    {
        public int beatPosition = 0;
        public float currentTempo = 0;
        public float lastTempo = 0;
    }

    public TimelineInfo timelineInfo = null;

    private GCHandle timelineHandle;

    private FMOD.Studio.EVENT_CALLBACK beatCallback;
    private FMOD.Studio.EventDescription descriptionCallback;

    private FMOD.Studio.EventInstance currentMusicTrack;

    private double beatWindow = 0.2f;

    public bool CheckCanInteract()
    {
        if (lateFlag)
        {
            RuntimeManager.PlayOneShot(_perfectBeatTrackSound);
            OnSuccessCheckInteract?.Invoke();
            return true;
        }

        OnFailureCheckInteract?.Invoke();
        return false;
    }

    private void AssignMusicCallbacks()
    {
        timelineInfo = new TimelineInfo();
        beatCallback = BeatEventCallback;

        timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
        currentMusicTrack.setUserData(GCHandle.ToIntPtr(timelineHandle));
        currentMusicTrack.setCallback(beatCallback,
            FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        currentMusicTrack.getDescription(out descriptionCallback);
        descriptionCallback.getLength(out _);


        RuntimeManager.CoreSystem.getSoftwareFormat(out masterSampleRate, out _, out _);
    }

    public void SetMusicTrack(EventReference newTrack)
    {
        FMOD.Studio.EventDescription description;

        if (isPlayingMusic)
        {
            isPlayingMusic = false;

            currentMusicTrack.getDescription(out description);
            description.unloadSampleData();

            currentMusicTrack.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        currentMusicTrack = RuntimeManager.CreateInstance(newTrack);

        currentMusicTrack.getDescription(out description);

        description.loadSampleData();
    }

    public void PlayMusicTrack()
    {
        isPlayingMusic = true;

        currentMusicTrack.start();

        Instance.AssignMusicCallbacks();
    }

    private void SetTrackStartInfo()
    {
        UpdateDSPClock();

        tempoTrackDSPStartTime = currentTime;
        lastFixedBeatTime = 0f;
        lastFixedBeatDSPTime = currentTime;
    }

    private void UpdateDSPClock()
    {
        currentMusicTrack.getChannelGroup(out FMOD.ChannelGroup newChanGrp);
        newChanGrp.getDSPClock(out dspClock, out _);

        currentSamples = dspClock;
        currentTime = currentSamples / masterSampleRate; // ��¥ �ð�
    }

    public float GetBeatInterval()
    {
        return (float)beatInterval;
    }

    public float GetBeatWindow()
    {
        return (float)beatWindow;
    }

    private void Update()
    {
        if (isPlayingMusic)
            gameTime += Time.deltaTime;

        currentMusicTrack.getPlaybackState(out musicPlayState);

        if (lastMusicPlayState != FMOD.Studio.PLAYBACK_STATE.PLAYING &&
            musicPlayState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            SetTrackStartInfo();
        }

        lastMusicPlayState = musicPlayState;

        if (musicPlayState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            return;
        }

        UpdateDSPClock();

        CheckTempoMarkers();

        if (beatInterval == 0f)
        {
            return;
        }

        CheckNextBeat();
    }

    private bool lateFlag = false;

    private void CheckNextBeat()
    {
        float fixedSongPosition = (float)(currentTime - tempoTrackDSPStartTime);

        if (fixedSongPosition >= lastFixedBeatTime + beatInterval)
        {
            float r = Mathf.Repeat(fixedSongPosition, (float)beatInterval);

            lastFixedBeatTime = (fixedSongPosition - r);
            lastFixedBeatDSPTime = (currentTime - r);
            OnBeatTiming?.Invoke();
        }

        if (fixedSongPosition >= lastFixedBeatTime + (beatInterval - beatWindow / 2) && !lateFlag)
        {
            lateFlag = true;
        }

        if (fixedSongPosition >= lastFixedBeatTime + beatWindow / 2 &&
            fixedSongPosition < lastFixedBeatTime + (beatInterval - beatWindow / 2) && lateFlag)
        {
            lateFlag = false;
            DoFixedBeat().Forget();
        }
    }

    //
    private async UniTask DoFixedBeat()
    {
        List<UniTask> list;
        OnPlayerMovement?.Invoke();
        OnEnemyMovement?.Invoke();

        if (OnBeatMovementFunc.Count > 0)
        {
            list = OnBeatMovementFunc.Select(x => x()).ToList();
            await UniTask.WhenAll(list);
            await UniTask.Delay(100);
        }

        if (OnBeatAttackFunc.Count > 0)
        {
            list = OnBeatAttackFunc.Select(x => x()).ToList();
            await UniTask.WhenAll(list);
        }
    }

    private bool CheckTempoMarkers()
    {
        if (!Mathf.Approximately(timelineInfo.currentTempo, timelineInfo.lastTempo))
        {
            SetTrackTempo();
            return true;
        }

        return false;
    }

    private void SetTrackTempo()
    {
        currentMusicTrack.getTimelinePosition(out var currentTimelinePos);

        float offset = (currentTimelinePos - timelineInfo.beatPosition) / 1000f;

        tempoTrackDSPStartTime = currentTime - offset;

        lastFixedBeatTime = 0f;
        lastFixedBeatDSPTime = tempoTrackDSPStartTime;

        timelineInfo.lastTempo = timelineInfo.currentTempo;

        beatInterval = 60f / timelineInfo.currentTempo;
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr,
        IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        FMOD.RESULT result = instance.getUserData(out var timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                {
                    var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr,
                        typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                    timelineInfo.beatPosition = parameter.position;
                    timelineInfo.currentTempo = parameter.tempo;
                }
                    break;
            }
        }

        return FMOD.RESULT.OK;
    }

    private void OnDestroy()
    {
        currentMusicTrack.setUserData(IntPtr.Zero);
        currentMusicTrack.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        currentMusicTrack.release();
        if (timelineHandle.IsAllocated)
            timelineHandle.Free();
    }
}