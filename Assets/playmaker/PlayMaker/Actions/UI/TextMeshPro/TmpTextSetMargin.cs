#if PLAYMAKER_TMPRO
using TMPro;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the margin of a TextMeshPro component (Left, Top, Right, Bottom).")]
    public class TmpTextSetMargin : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Left margin. Leave to None for no effect.")]
        public FsmFloat left;

        [Tooltip("Top margin. Leave to None for no effect.")]
        public FsmFloat top;

        [Tooltip("Right margin. Leave to None for no effect.")]
        public FsmFloat right;

        [Tooltip("Bottom margin. Leave to None for no effect.")]
        public FsmFloat bottom;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;
        private Vector4 originalMargin;

        public override void Reset()
        {
            gameObject = null;
            left = new FsmFloat { UseVariable = true };
            top = new FsmFloat { UseVariable = true };
            right = new FsmFloat { UseVariable = true };
            bottom = new FsmFloat { UseVariable = true };
            resetOnExit = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                tmpText = cachedComponent;
            }

            if (tmpText != null)
            {
                originalMargin = tmpText.margin;
            }

            DoSetValue();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoSetValue();
        }

        private void DoSetValue()
        {
            if (tmpText == null) return;

            var m = tmpText.margin;

            if (!left.IsNone)   m.x = left.Value;
            if (!top.IsNone)    m.y = top.Value;
            if (!right.IsNone)  m.z = right.Value;
            if (!bottom.IsNone) m.w = bottom.Value;

            tmpText.margin = m;
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.margin = originalMargin;
            }
        }
    }
}
#endif
