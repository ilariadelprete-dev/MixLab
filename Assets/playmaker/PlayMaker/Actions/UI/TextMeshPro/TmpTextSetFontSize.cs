#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the font size of a TextMeshPro component.")]
    public class TmpTextSetFontSize : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The font size to set.")]
        public FsmFloat fontSize;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;
        private float originalFontSize;

        public override void Reset()
        {
            gameObject = null;
            fontSize = null;
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
                originalFontSize = tmpText.fontSize;
            }

            DoSetFontSize();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoSetFontSize();
        }

        private void DoSetFontSize()
        {
            if (tmpText == null) return;
            if (!fontSize.IsNone)
            {
                tmpText.fontSize = fontSize.Value;
            }
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.fontSize = originalFontSize;
            }
        }
    }
}
#endif
