#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets only the alpha channel of a TextMeshPro component's color.")]
    public class TmpTextSetAlpha : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Alpha value (0 = fully transparent, 1 = fully opaque).")]
        public FsmFloat alpha;

        [Tooltip("Reset to original alpha when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;
        private float originalAlpha;

        public override void Reset()
        {
            gameObject = null;
            alpha = 1f;
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
                originalAlpha = tmpText.alpha;
            }

            DoSetAlpha();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoSetAlpha();
        }

        private void DoSetAlpha()
        {
            if (tmpText == null) return;
            if (!alpha.IsNone)
            {
                tmpText.alpha = alpha.Value;
            }
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.alpha = originalAlpha;
            }
        }
    }
}
#endif
