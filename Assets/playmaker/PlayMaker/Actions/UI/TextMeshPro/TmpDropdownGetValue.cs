#if PLAYMAKER_TMPRO
using TMPro;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Gets the selected value (index and text) from a TMP_Dropdown component.")]
    public class TmpDropdownGetValue : ComponentAction<TMP_Dropdown>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Dropdown))]
        [Tooltip("The GameObject with the TMP_Dropdown component.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the selected index (zero-based).")]
        public FsmInt selectedIndex;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the selected option text.")]
        public FsmString selectedText;

        [ObjectType(typeof(Sprite))]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the selected option sprite/image.")]
        public FsmObject selectedImage;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Dropdown dropdown;

        public override void Reset()
        {
            gameObject = null;
            selectedIndex = null;
            selectedText = null;
            selectedImage = null;
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
            if (dropdown == null || dropdown.options.Count == 0) return;

            var idx = dropdown.value;

            if (!selectedIndex.IsNone) selectedIndex.Value = idx;
            if (!selectedText.IsNone)  selectedText.Value  = dropdown.options[idx].text;
            if (!selectedImage.IsNone) selectedImage.Value = dropdown.options[idx].image;
        }
    }
}
#endif
