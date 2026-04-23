#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the content type of a TMP_InputField (Standard, Autocorrected, IntegerNumber, DecimalNumber, AlphanumericWithSymbols, Name, EmailAddress, Password, Pin, Custom).")]
    public class TmpInputFieldSetContentType : ComponentAction<TMP_InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_InputField))]
        [Tooltip("The GameObject with the TMP_InputField component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [ObjectType(typeof(TMP_InputField.ContentType))]
        [Tooltip("Content type (Standard, IntegerNumber, DecimalNumber, Password, etc.).")]
        public FsmEnum contentType;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        private TMP_InputField inputField;
        private TMP_InputField.ContentType originalValue;

        public override void Reset()
        {
            gameObject = null;
            contentType = TMP_InputField.ContentType.Standard;
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
                originalValue = inputField.contentType;
            }

            DoSetValue();
            Finish();
        }

        private void DoSetValue()
        {
            if (inputField == null) return;
            inputField.contentType = (TMP_InputField.ContentType)contentType.Value;
        }

        public override void OnExit()
        {
            if (inputField == null) return;
            if (resetOnExit.Value)
            {
                inputField.contentType = originalValue;
            }
        }
    }
}
#endif
