#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Enables or disables word wrapping on a TextMeshPro component.")]
    public class TmpTextSetWordWrapping : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Enable word wrapping.")]
        public FsmBool enableWordWrapping;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        private TMP_Text tmpText;
        private bool originalValue;

        public override void Reset()
        {
            gameObject = null;
            enableWordWrapping = true;
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
                originalValue = tmpText.enableWordWrapping;
            }

            DoSetValue();
            Finish();
        }

        private void DoSetValue()
        {
            if (tmpText == null) return;
            if (!enableWordWrapping.IsNone)
            {
                tmpText.enableWordWrapping = enableWordWrapping.Value;
            }
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.enableWordWrapping = originalValue;
            }
        }
    }
}
#endif
