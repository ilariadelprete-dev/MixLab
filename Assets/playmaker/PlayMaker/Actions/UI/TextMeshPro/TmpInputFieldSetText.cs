#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the text of a TMP_InputField component.")]
    public class TmpInputFieldSetText : ComponentAction<TMP_InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_InputField))]
        [Tooltip("The GameObject with the TMP_InputField component.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.TextArea)]
        [Tooltip("The text to set.")]
        public FsmString text;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_InputField inputField;
        private string originalText;

        public override void Reset()
        {
            gameObject = null;
            text = null;
            resetOnExit = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                inputField = cachedComponent;
            }

            if (inputField != null)
            {
                originalText = inputField.text;
            }

            DoSetText();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoSetText();
        }

        private void DoSetText()
        {
            if (inputField == null) return;
            inputField.text = text.Value;
        }

        public override void OnExit()
        {
            if (inputField == null) return;
            if (resetOnExit.Value)
            {
                inputField.text = originalText;
            }
        }
    }
}
#endif
