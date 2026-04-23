#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the selected value (zero-based index) of a TMP_Dropdown component.")]
    public class TmpDropdownSetValue : ComponentAction<TMP_Dropdown>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Dropdown))]
        [Tooltip("The GameObject with the TMP_Dropdown component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The index to select (zero-based).")]
        public FsmInt value;

        [Tooltip("Send notification callbacks. If false, the value changes silently.")]
        public FsmBool sendCallback;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        private TMP_Dropdown dropdown;
        private int originalValue;

        public override void Reset()
        {
            gameObject = null;
            value = 0;
            sendCallback = true;
            resetOnExit = null;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                dropdown = cachedComponent;
            }

            if (dropdown != null)
            {
                originalValue = dropdown.value;
            }

            DoSetValue();
            Finish();
        }

        private void DoSetValue()
        {
            if (dropdown == null) return;
            if (!value.IsNone)
            {
                if (sendCallback.Value)
                {
                    dropdown.value = value.Value;
                }
                else
                {
                    dropdown.SetValueWithoutNotify(value.Value);
                }
            }
        }

        public override void OnExit()
        {
            if (dropdown == null) return;
            if (resetOnExit.Value)
            {
                dropdown.SetValueWithoutNotify(originalValue);
            }
        }
    }
}
#endif
