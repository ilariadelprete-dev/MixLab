#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the maximum number of visible lines in a TextMeshPro component.")]
    public class TmpTextSetMaxVisibleLines : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Maximum number of visible lines. Set to 99999 to show all.")]
        public FsmInt maxVisibleLines;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;
        private int originalValue;

        public override void Reset()
        {
            gameObject = null;
            maxVisibleLines = 99999;
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
                originalValue = tmpText.maxVisibleLines;
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
            if (!maxVisibleLines.IsNone)
            {
                tmpText.maxVisibleLines = maxVisibleLines.Value;
            }
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.maxVisibleLines = originalValue;
            }
        }
    }
}
#endif
