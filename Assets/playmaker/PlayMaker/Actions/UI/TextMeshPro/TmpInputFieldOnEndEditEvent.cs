#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Listens for the onEndEdit event of a TMP_InputField (fired when the user finishes editing, e.g. presses Enter or loses focus). Stores the final text and/or sends a PlayMaker event.")]
    public class TmpInputFieldOnEndEditEvent : ComponentAction<TMP_InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_InputField))]
        [Tooltip("The GameObject with the TMP_InputField component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Event to send when editing ends.")]
        public FsmEvent sendEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the final text value.")]
        public FsmString finalValue;

        private TMP_InputField inputField;

        public override void Reset()
        {
            gameObject = null;
            eventTarget = null;
            sendEvent = null;
            finalValue = null;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                inputField = cachedComponent;
                if (inputField != null)
                {
                    inputField.onEndEdit.AddListener(OnEndEdit);
                }
            }
        }

        public override void OnExit()
        {
            if (inputField != null)
            {
                inputField.onEndEdit.RemoveListener(OnEndEdit);
            }
        }

        private void OnEndEdit(string value)
        {
            finalValue.Value = value;
            Fsm.EventData.StringData = value;
            SendEvent(eventTarget, sendEvent);
        }
    }
}
#endif
