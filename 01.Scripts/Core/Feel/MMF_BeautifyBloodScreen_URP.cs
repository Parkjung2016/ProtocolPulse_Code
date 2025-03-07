using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackHelp(
        "This feedback allows you to control blood frame intensity over time. It requires you have in your scene an object with a Volume " +
        "with Frame active, and a MMF_BeautifyBloodScreen_URP component.")]
    [FeedbackPath("PostProcess/Beautify Blood Screen URP")]
    [MovedFrom(false, null, "MoreMountains.Feedbacks.URP")]
    public class MMF_BeautifyBloodScreen_URP : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.PostProcessColor; }
        }

        public override bool HasCustomInspectors => true;
        public override bool HasAutomaticShakerSetup => true;
#endif

        public override float FeedbackDuration
        {
            get { return ApplyTimeMultiplier(ShakeDuration); }
            set { ShakeDuration = value; }
        }

        public override bool HasChannel => true;
        public override bool HasRandomness => true;

        [MMFInspectorGroup("BloodScreen", true, 41)]
        public float ShakeDuration = 0.2f;

        public bool ResetShakerValuesAfterShake = true;

        public bool ResetTargetValuesAfterShake = true;

        public bool RelativeValues = true;

        [MMFInspectorGroup("Alpha", true, 42)]
        /// the curve to animate the intensity on
        public AnimationCurve ShakeAlpha =
            new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        public float RemapAlphaZero = 0f;

        public float RemapAlphaOne = 1f;


        /// <summary>
        /// Triggers a bloom shake
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }

            attenuation = ComputeIntensity(attenuation, position);

            MMBeautifyBloodScreenShakeEvent_URP.Trigger(ShakeAlpha, FeedbackDuration, RemapAlphaZero,
                RemapAlphaOne,
                RelativeValues, attenuation, ChannelData, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake,
                NormalPlayDirection, ComputedTimescaleMode);
        }

        /// <summary>
        /// On stop we stop our transition
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }

            base.CustomStopFeedback(position, feedbacksIntensity);
            MMBeautifyBloodScreenShakeEvent_URP.Trigger(ShakeAlpha, FeedbackDuration, RemapAlphaZero,
                RemapAlphaOne,
                RelativeValues, channelData: ChannelData, stop: true);
        }

        /// <summary>
        /// On restore, we put our object back at its initial position
        /// </summary>
        protected override void CustomRestoreInitialValues()
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }

            MMBeautifyBloodScreenShakeEvent_URP.Trigger(ShakeAlpha, FeedbackDuration, RemapAlphaZero,
                RemapAlphaOne,
                RelativeValues, channelData: ChannelData, restore: true);
        }
    }
}