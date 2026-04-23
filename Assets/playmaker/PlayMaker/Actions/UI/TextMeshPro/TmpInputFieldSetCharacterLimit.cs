#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the character limit of a TMP_InputField component. Set to 0 for no limit.")]
    public class TmpInputFieldSetCharacterLimit : ComponentAction<TMP_InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_InputField))]
        [Tooltip("The GameObject with the TMP_InputField component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Maximum number of characters. 0 = no limit.")]
        public FsmInt characterLimit;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        private TMP_InputField inputField;
        private int originalValue;

        public override void Reset()
        {
            gameObject = null;
            characterLimit = 0;
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
                originalValue = inputField.characterLimit;
            }

            DoSetValue();
            Finish();
        }

        private void DoSetValue()
        {
            if (inputField == null) return;
            if (!characterLimit.IsNone)
            {
                inputField.characterLimit = characterLimit.Value;
            }
        }

        public override void OnExit()
        {
            if (inputField == null) return;
            if (resetOnExit.Value)
            {
                inputField.characterLimit = originalValue;
            }
        }
    }
}
#endif
