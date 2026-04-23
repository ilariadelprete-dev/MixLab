#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the placeholder text of a TMP_InputField component.")]
    public class TmpInputFieldSetPlaceholder : ComponentAction<TMP_InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_InputField))]
        [Tooltip("The GameObject with the TMP_InputField component.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.TextArea)]
        [Tooltip("The placeholder text to set.")]
        public FsmString placeholderText;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        private TMP_InputField inputField;
        private string originalText;

        public override void Reset()
        {
            gameObject = null;
            placeholderText = null;
            resetOnExit = null;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                inputField = cachedComponent;
            }

            if (inputField != null && inputField.placeholder != null)
            {
                var ph = inputField.placeholder as TMP_Text;
                if (ph != null) originalText = ph.text;
            }

            DoSetValue();
            Finish();
        }

        private void DoSetValue()
        {
            if (inputField == null || inputField.placeholder == null) return;

            var ph = inputField.placeholder as TMP_Text;
            if (ph != null)
            {
                ph.text = placeholderText.Value;
            }
        }

        public override void OnExit()
        {
            if (inputField == null || inputField.placeholder == null) return;
            if (resetOnExit.Value)
            {
                var ph = inputField.placeholder as TMP_Text;
                if (ph != null) ph.text = originalText;
            }
        }
    }
}
#endif
