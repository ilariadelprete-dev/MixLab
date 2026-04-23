#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Gets whether a TMP_InputField currently has focus.")]
    public class TmpInputFieldGetIsFocused : ComponentAction<TMP_InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_InputField))]
        [Tooltip("The GameObject with the TMP_InputField component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store whether the input field is focused.")]
        public FsmBool isFocused;

        [Tooltip("Event to send when the field is focused.")]
        public FsmEvent focusedEvent;

        [Tooltip("Event to send when the field is not focused.")]
        public FsmEvent notFocusedEvent;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_InputField inputField;

        public override void Reset()
        {
            gameObject = null;
            isFocused = null;
            focusedEvent = null;
            notFocusedEvent = null;
            everyFrame = true;
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

            isFocused.Value = inputField.isFocused;

            if (inputField.isFocused)
            {
                Fsm.Event(focusedEvent);
            }
            else
            {
                Fsm.Event(notFocusedEvent);
            }
        }
    }
}
#endif
