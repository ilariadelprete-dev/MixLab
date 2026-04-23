#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the text of a TextMeshPro component (works with both TextMeshProUGUI and TextMeshPro 3D).")]
    public class TmpTextSetText : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.TextArea)]
        [Tooltip("The text to set.")]
        public FsmString text;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;
        private string originalText;

        public override void Reset()
        {
            gameObject = null;
            text = null;
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
                originalText = tmpText.text;
            }

            DoSetText();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoSetText();
        }

        private void DoSetText()
        {
            if (tmpText == null) return;
            tmpText.text = text.Value;
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.text = originalText;
            }
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName("TMP SetText", text);
        }
#endif
    }
}
#endif
