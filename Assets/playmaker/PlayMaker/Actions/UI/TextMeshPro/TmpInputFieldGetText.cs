#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Gets the current text value from a TMP_InputField component.")]
    public class TmpInputFieldGetText : ComponentAction<TMP_InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_InputField))]
        [Tooltip("The GameObject with the TMP_InputField component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the text value.")]
        public FsmString text;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_InputField inputField;

        public override void Reset()
        {
            gameObject = null;
            text = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                inputField = cachedComponent;
            }

            DoGetText();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoGetText();
        }

        private void DoGetText()
        {
            if (inputField == null) return;
            text.Value = inputField.text;
        }
    }
}
#endif
