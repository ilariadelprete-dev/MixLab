#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Activates a TMP_InputField, bringing focus to it and showing the keyboard on mobile.")]
    public class TmpInputFieldActivate : ComponentAction<TMP_InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_InputField))]
        [Tooltip("The GameObject with the TMP_InputField component.")]
        public FsmOwnerDefault gameObject;

        private TMP_InputField inputField;

        public override void Reset()
        {
            gameObject = null;
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
                inputField.ActivateInputField();
            }

            Finish();
        }
    }
}
#endif
