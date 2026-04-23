#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Listens for the onValueChanged event of a TMP_Dropdown. Stores the selected index and text, and/or sends a PlayMaker event. The event int data contains the new index.")]
    public class TmpDropdownOnValueChangedEvent : ComponentAction<TMP_Dropdown>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Dropdown))]
        [Tooltip("The GameObject with the TMP_Dropdown component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Event to send when the selected value changes.")]
        public FsmEvent sendEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the new selected index.")]
        public FsmInt selectedIndex;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the new selected text.")]
        public FsmString selectedText;

        private TMP_Dropdown dropdown;

        public override void Reset()
        {
            gameObject = null;
            eventTarget = null;
            sendEvent = null;
            selectedIndex = null;
            selectedText = null;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                dropdown = cachedComponent;
                if (dropdown != null)
                {
                    dropdown.onValueChanged.AddListener(OnValueChanged);
                }
            }
        }

        public override void OnExit()
        {
            if (dropdown != null)
            {
                dropdown.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        private void OnValueChanged(int index)
        {
            selectedIndex.Value = index;

            if (!selectedText.IsNone && dropdown.options.Count > index)
            {
                selectedText.Value = dropdown.options[index].text;
            }

            Fsm.EventData.IntData = index;
            SendEvent(eventTarget, sendEvent);
        }
    }
}
#endif
