using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Rendering;

namespace MoreMountains.FeedbacksForThirdParty
{
    [RequireComponent(typeof(Volume))]
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMBeautifyBloodScreenShaker_URP")]
    public class MMBeautifyBloodScreenShaker_URP : MMShaker
    {
        public bool RelativeValues = true;

        [MMInspectorGroup("BloodScreen Alpha", true, 51)]
        public AnimationCurve ShakeAlpha =
            new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        public float RemapAlphaZero = 0f;

        public float RemapAlphaOne = 1f;


        protected Beautify.Universal.Beautify _beautify;
        protected float _initialAlpha;
        protected float _originalShakeDuration;
        protected bool _originalRelativeAlpha;
        protected AnimationCurve _originalShakeAlpha;
        protected float _originalRemapAlphaZero;
        protected float _originalRemapAlphaOne;

        protected override void Initialization()
        {
            base.Initialization();
            this.gameObject.GetComponent<Volume>().profile.TryGet(out _beautify);
        }

        /// <summary>
        /// Shakes values over time
        /// </summary>
        protected override void Shake()
        {
            float newAlpha =
                ShakeFloat(ShakeAlpha, RemapAlphaZero, RemapAlphaOne, RelativeValues, _initialAlpha);
            Color color = _beautify.frameColor.value;
            color.a = newAlpha;
            _beautify.frameColor.Override(color);
        }

        /// <summary>
        /// Collects initial values on the target
        /// </summary>
        protected override void GrabInitialValues()
        {
            _initialAlpha = _beautify.frameColor.value.a;
        }

        /// <summary>
        /// When we get the appropriate event, we trigger a shake
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="duration"></param>
        /// <param name="amplitude"></param>
        /// <param name="relativeAlpha"></param>
        /// <param name="attenuation"></param>
        /// <param name="channel"></param>
        public virtual void OnBloodScreenShakeEvent(AnimationCurve alpha, float duration, float remapMin,
            float remapMax,
            bool relativeAlpha = false,
            float attenuation = 1.0f, MMChannelData channelData = null, bool resetShakerValuesAfterShake =
                true, bool resetTargetValuesAfterShake = true,
            bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop =
                false, bool restore = false)
        {
            if (!CheckEventAllowed(channelData) || (!Interruptible && Shaking))
            {
                return;
            }

            if (stop)
            {
                Stop();
                return;
            }

            if (restore)
            {
                ResetTargetValues();
                return;
            }

            _resetShakerValuesAfterShake = resetShakerValuesAfterShake;
            _resetTargetValuesAfterShake = resetTargetValuesAfterShake;

            if (resetShakerValuesAfterShake)
            {
                _originalShakeDuration = ShakeDuration;
                _originalShakeAlpha = ShakeAlpha;
                _originalRemapAlphaZero = RemapAlphaZero;
                _originalRemapAlphaOne = RemapAlphaOne;
                _originalRelativeAlpha = RelativeValues;
            }

            if (!OnlyUseShakerValues)
            {
                TimescaleMode = timescaleMode;
                ShakeDuration = duration;
                ShakeAlpha = alpha;
                RemapAlphaZero = remapMin * attenuation;
                RemapAlphaOne = remapMax * attenuation;
                RelativeValues = relativeAlpha;
                ForwardDirection = forwardDirection;
            }

            Play();
        }

        /// <summary>
        /// Resets the target's values
        /// </summary>
        protected override void ResetTargetValues()
        {
            base.ResetTargetValues();
            Color color = _beautify.frameColor.value;
            color.a = _initialAlpha;
            _beautify.frameColor.Override(color);
        }

        /// <summary>
        /// Resets the shaker's values
        /// </summary>
        protected override void ResetShakerValues()
        {
            base.ResetShakerValues();
            ShakeDuration = _originalShakeDuration;
            ShakeAlpha = _originalShakeAlpha;
            RemapAlphaZero = _originalRemapAlphaZero;
            RemapAlphaOne = _originalRemapAlphaOne;
            RelativeValues = _originalRelativeAlpha;
        }

        /// <summary>
        /// Starts listening for events
        /// </summary>
        public override void StartListening()
        {
            base.StartListening();
            MMBeautifyBloodScreenShakeEvent_URP.Register(OnBloodScreenShakeEvent);
        }

        /// <summary>
        /// Stops listening for events
        /// </summary>
        public override void StopListening()
        {
            base.StopListening();
            MMBeautifyBloodScreenShakeEvent_URP.Unregister(OnBloodScreenShakeEvent);
        }
    }

    /// <summary>
    /// An event used to trigger vignette shakes
    /// </summary>
    public struct MMBeautifyBloodScreenShakeEvent_URP
    {
        static private event Delegate OnEvent;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInitialization()
        {
            OnEvent = null;
        }

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        public delegate void Delegate(AnimationCurve alpha, float duration, float remapMin, float remapMax,
            bool relativeAlpha = false,
            float attenuation = 1.0f, MMChannelData channelData = null, bool resetShakerValuesAfterShake = true,
            bool resetTargetValuesAfterShake = true,
            bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false,
            bool restore = false);

        static public void Trigger(AnimationCurve alpha, float duration, float remapMin, float remapMax,
            bool relativeAlpha = false,
            float attenuation = 1.0f, MMChannelData channelData = null, bool resetShakerValuesAfterShake = true,
            bool resetTargetValuesAfterShake = true,
            bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false,
            bool restore = false)
        {
            OnEvent?.Invoke(alpha, duration, remapMin, remapMax, relativeAlpha, attenuation, channelData,
                resetShakerValuesAfterShake,
                resetTargetValuesAfterShake, forwardDirection, timescaleMode, stop, restore);
        }
    }
}