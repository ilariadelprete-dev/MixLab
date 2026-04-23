#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Enables or disables auto-sizing on a TextMeshPro component. Optionally sets the minimum and maximum font size.")]
    public class TmpTextSetAutoSizing : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Enable auto font sizing.")]
        public FsmBool enableAutoSizing;

        [Tooltip("Minimum font size when auto-sizing is active. Leave to None to keep current value.")]
        public FsmFloat fontSizeMin;

        [Tooltip("Maximum font size when auto-sizing is active. Leave to None to keep current value.")]
        public FsmFloat fontSizeMax;

        [Tooltip("Reset to original values when exiting this state.")]
        public FsmBool resetOnExit;

        private TMP_Text tmpText;
        private bool originalEnabled;
        private float originalMin;
        private float originalMax;

        public override void Reset()
        {
            gameObject = null;
            enableAutoSizing = true;
            fontSizeMin = new FsmFloat { UseVariable = true };
            fontSizeMax = new FsmFloat { UseVariable = true };
            resetOnExit = null;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                tmpText = cachedComponent;
            }

            if (tmpText != null)
            {
                originalEnabled = tmpText.enableAutoSizing;
                originalMin = tmpText.fontSizeMin;
                originalMax = tmpText.fontSizeMax;
            }

            DoSetValue();
            Finish();
        }

        private void DoSetValue()
        {
            if (tmpText == null) return;

            if (!enableAutoSizing.IsNone)
            {
                tmpText.enableAutoSizing = enableAutoSizing.Value;
            }

            if (!fontSizeMin.IsNone)
            {
                tmpText.fontSizeMin = fontSizeMin.Value;
            }

            if (!fontSizeMax.IsNone)
            {
                tmpText.fontSizeMax = fontSizeMax.Value;
            }
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.enableAutoSizing = originalEnabled;
                tmpText.fontSizeMin = originalMin;
                tmpText.fontSizeMax = originalMax;
            }
        }
    }
}
#endif
