#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the maximum number of visible words in a TextMeshPro component.")]
    public class TmpTextSetMaxVisibleWords : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Maximum number of visible words. Set to 99999 to show all.")]
        public FsmInt maxVisibleWords;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;
        private int originalValue;

        public override void Reset()
        {
            gameObject = null;
            maxVisibleWords = 99999;
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
                originalValue = tmpText.maxVisibleWords;
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
            if (!maxVisibleWords.IsNone)
            {
                tmpText.maxVisibleWords = maxVisibleWords.Value;
            }
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.maxVisibleWords = originalValue;
            }
        }
    }
}
#endif
