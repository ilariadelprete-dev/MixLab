#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Enables or disables a TextMeshPro component.")]
    public class TmpTextSetEnabled : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Enable or disable the component.")]
        public FsmBool enabled;

        [Tooltip("Reset to original state when exiting this state.")]
        public FsmBool resetOnExit;

        private TMP_Text tmpText;
        private bool originalEnabled;

        public override void Reset()
        {
            gameObject = null;
            enabled = true;
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
                originalEnabled = tmpText.enabled;
            }

            DoSetValue();
            Finish();
        }

        private void DoSetValue()
        {
            if (tmpText == null) return;
            if (!enabled.IsNone)
            {
                tmpText.enabled = enabled.Value;
            }
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.enabled = originalEnabled;
            }
        }
    }
}
#endif
