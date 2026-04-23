#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the line type of a TMP_InputField (SingleLine, MultiLineSubmit, MultiLineNewline).")]
    public class TmpInputFieldSetLineType : ComponentAction<TMP_InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_InputField))]
        [Tooltip("The GameObject with the TMP_InputField component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [ObjectType(typeof(TMP_InputField.LineType))]
        [Tooltip("Line type: SingleLine, MultiLineSubmit, MultiLineNewline.")]
        public FsmEnum lineType;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        private TMP_InputField inputField;
        private TMP_InputField.LineType originalValue;

        public override void Reset()
        {
            gameObject = null;
            lineType = TMP_InputField.LineType.SingleLine;
            resetOnExit = null;
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
                originalValue = inputField.lineType;
            }

            DoSetValue();
            Finish();
        }

        private void DoSetValue()
        {
            if (inputField == null) return;
            inputField.lineType = (TMP_InputField.LineType)lineType.Value;
        }

        public override void OnExit()
        {
            if (inputField == null) return;
            if (resetOnExit.Value)
            {
                inputField.lineType = originalValue;
            }
        }
    }
}
#endif
