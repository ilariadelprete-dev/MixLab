#if PLAYMAKER_TMPRO
using TMPro;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the color of a TextMeshPro component. You can set the full color or individual channels (R, G, B, A).")]
    public class TmpTextSetColor : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The full color. Leave to None to use individual channel values.")]
        public FsmColor color;

        [Tooltip("Red channel (0-1). Leave to None for no effect.")]
        public FsmFloat red;

        [Tooltip("Green channel (0-1). Leave to None for no effect.")]
        public FsmFloat green;

        [Tooltip("Blue channel (0-1). Leave to None for no effect.")]
        public FsmFloat blue;

        [Tooltip("Alpha channel (0-1). Leave to None for no effect.")]
        public FsmFloat alpha;

        [Tooltip("Reset to original color when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;
        private Color originalColor;

        public override void Reset()
        {
            gameObject = null;
            color = null;
            red = new FsmFloat { UseVariable = true };
            green = new FsmFloat { UseVariable = true };
            blue = new FsmFloat { UseVariable = true };
            alpha = new FsmFloat { UseVariable = true };
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
                originalColor = tmpText.color;
            }

            DoSetColor();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoSetColor();
        }

        private void DoSetColor()
        {
            if (tmpText == null) return;

            var c = tmpText.color;

            if (!color.IsNone)
            {
                c = color.Value;
            }

            if (!red.IsNone)   c.r = red.Value;
            if (!green.IsNone) c.g = green.Value;
            if (!blue.IsNone)  c.b = blue.Value;
            if (!alpha.IsNone) c.a = alpha.Value;

            tmpText.color = c;
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.color = originalColor;
            }
        }
    }
}
#endif
