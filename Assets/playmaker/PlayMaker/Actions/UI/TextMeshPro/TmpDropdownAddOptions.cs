#if PLAYMAKER_TMPRO
using System.Collections.Generic;
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Adds text options to a TMP_Dropdown component from a string array variable.")]
    public class TmpDropdownAddOptions : ComponentAction<TMP_Dropdown>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Dropdown))]
        [Tooltip("The GameObject with the TMP_Dropdown component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [ArrayEditor(VariableType.String)]
        [Tooltip("Array of option labels to add.")]
        public FsmArray options;

        [Tooltip("Clear existing options before adding new ones.")]
        public FsmBool clearFirst;

        private TMP_Dropdown dropdown;

        public override void Reset()
        {
            gameObject = null;
            options = null;
            clearFirst = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                dropdown = cachedComponent;
            }

            DoAddOptions();
            Finish();
        }

        private void DoAddOptions()
        {
            if (dropdown == null || options == null) return;

            if (clearFirst.Value)
            {
                dropdown.ClearOptions();
            }

            var list = new List<string>();
            foreach (var item in options.Values)
            {
                if (item != null)
                {
                    list.Add(item.ToString());
                }
            }

            dropdown.AddOptions(list);
        }
    }
}
#endif
