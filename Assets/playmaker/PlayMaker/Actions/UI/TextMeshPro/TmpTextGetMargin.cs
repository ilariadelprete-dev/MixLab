#if PLAYMAKER_TMPRO
using TMPro;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Gets the margin values (Left, Top, Right, Bottom) of a TextMeshPro component.")]
    public class TmpTextGetMargin : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the left margin.")]
        public FsmFloat left;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the top margin.")]
        public FsmFloat top;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the right margin.")]
        public FsmFloat right;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the bottom margin.")]
        public FsmFloat bottom;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;

        public override void Reset()
        {
            gameObject = null;
            left = null;
            top = null;
            right = null;
            bottom = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                tmpText = cachedComponent;
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
            if (tmpText == null) return;

            var m = tmpText.margin;

            if (!left.IsNone)   left.Value   = m.x;
            if (!top.IsNone)    top.Value    = m.y;
            if (!right.IsNone)  right.Value  = m.z;
            if (!bottom.IsNone) bottom.Value = m.w;
        }
    }
}
#endif
