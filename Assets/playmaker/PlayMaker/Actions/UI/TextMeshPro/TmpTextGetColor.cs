#if PLAYMAKER_TMPRO
using TMPro;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Gets the color of a TextMeshPro component, storing the full color and/or individual channels.")]
    public class TmpTextGetColor : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the full color.")]
        public FsmColor color;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the red channel (0-1).")]
        public FsmFloat red;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the green channel (0-1).")]
        public FsmFloat green;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the blue channel (0-1).")]
        public FsmFloat blue;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the alpha channel (0-1).")]
        public FsmFloat alpha;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;

        public override void Reset()
        {
            gameObject = null;
            color = null;
            red = null;
            green = null;
            blue = null;
            alpha = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                tmpText = cachedComponent;
            }

            DoGetColor();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoGetColor();
        }

        private void DoGetColor()
        {
            if (tmpText == null) return;

            var c = tmpText.color;

            if (!color.IsNone)  color.Value  = c;
            if (!red.IsNone)    red.Value    = c.r;
            if (!green.IsNone)  green.Value  = c.g;
            if (!blue.IsNone)   blue.Value   = c.b;
            if (!alpha.IsNone)  alpha.Value  = c.a;
        }
    }
}
#endif
