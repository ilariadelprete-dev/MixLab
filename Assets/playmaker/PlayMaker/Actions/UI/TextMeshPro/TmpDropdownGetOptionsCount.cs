#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Gets the total number of options in a TMP_Dropdown component.")]
    public class TmpDropdownGetOptionsCount : ComponentAction<TMP_Dropdown>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Dropdown))]
        [Tooltip("The GameObject with the TMP_Dropdown component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the options count.")]
        public FsmInt count;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Dropdown dropdown;

        public override void Reset()
        {
            gameObject = null;
            count = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                dropdown = cachedComponent;
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
            if (dropdown == null) return;
            count.Value = dropdown.options.Count;
        }
    }
}
#endif
