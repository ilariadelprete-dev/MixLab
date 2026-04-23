#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Gets the character limit of a TMP_InputField component.")]
    public class TmpInputFieldGetCharacterLimit : ComponentAction<TMP_InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_InputField))]
        [Tooltip("The GameObject with the TMP_InputField component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the character limit value.")]
        public FsmInt characterLimit;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_InputField inputField;

        public override void Reset()
        {
            gameObject = null;
            characterLimit = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                inputField = cachedComponent;
            }

            DoGetValue();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoGetValue();
        }

        private void DoGetValue()
        {
            if (inputField == null) return;
            characterLimit.Value = inputField.characterLimit;
        }
    }
}
#endif
