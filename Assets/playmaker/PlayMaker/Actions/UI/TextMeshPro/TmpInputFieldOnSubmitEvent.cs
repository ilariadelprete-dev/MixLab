#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Listens for the onSubmit event of a TMP_InputField (fired when the user presses Enter/Submit). Stores the submitted text and/or sends a PlayMaker event.")]
    public class TmpInputFieldOnSubmitEvent : ComponentAction<TMP_InputField>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_InputField))]
        [Tooltip("The GameObject with the TMP_InputField component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Event to send when the input is submitted.")]
        public FsmEvent sendEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the submitted text value.")]
        public FsmString submittedValue;

        private TMP_InputField inputField;

        public override void Reset()
        {
            gameObject = null;
            eventTarget = null;
            sendEvent = null;
            submittedValue = null;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                inputField = cachedComponent;
                if (inputField != null)
                {
                    inputField.onSubmit.AddListener(OnSubmit);
                }
            }
        }

        public override void OnExit()
        {
            if (inputField != null)
            {
                inputField.onSubmit.RemoveListener(OnSubmit);
            }
        }

        private void OnSubmit(string value)
        {
            submittedValue.Value = value;
            Fsm.EventData.StringData = value;
            SendEvent(eventTarget, sendEvent);
        }
    }
}
#endif
