#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Listens for the onValueChanged event of a TMP_InputField. Stores the new text and/or sends a PlayMaker event. The event string data also contains the new value.")]
    public class TmpInputFieldOnValueChangedEvent : ComponentAction<TMP_InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_InputField))]
        [Tooltip("The GameObject with the TMP_InputField component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Event to send when the value changes.")]
        public FsmEvent sendEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the new text value.")]
        public FsmString newValue;

        private TMP_InputField inputField;

        public override void Reset()
        {
            gameObject = null;
            eventTarget = null;
            sendEvent = null;
            newValue = null;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                inputField = cachedComponent;
                if (inputField != null)
                {
                    inputField.onValueChanged.AddListener(OnValueChanged);
                }
            }
        }

        public override void OnExit()
        {
            if (inputField != null)
            {
                inputField.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        private void OnValueChanged(string value)
        {
            newValue.Value = value;
            Fsm.EventData.StringData = value;
            SendEvent(eventTarget, sendEvent);
        }
    }
}
#endif
