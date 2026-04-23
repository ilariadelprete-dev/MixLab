#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the text alignment of a TextMeshPro component.")]
    public class TmpTextSetAlignment : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [ObjectType(typeof(TextAlignmentOptions))]
        [Tooltip("The text alignment. E.g. TopLeft, TopCenter, TopRight, Left, Center, Right, BottomLeft, etc.")]
        public FsmEnum alignment;

        [Tooltip("Reset to original alignment when exiting this state.")]
        public FsmBool resetOnExit;

        private TMP_Text tmpText;
        private TextAlignmentOptions originalAlignment;

        public override void Reset()
        {
            gameObject = null;
            alignment = TextAlignmentOptions.Left;
            resetOnExit = null;
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
                originalAlignment = tmpText.alignment;
            }

            DoSetAlignment();
            Finish();
        }

        private void DoSetAlignment()
        {
            if (tmpText == null) return;
            tmpText.alignment = (TextAlignmentOptions)alignment.Value;
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.alignment = originalAlignment;
            }
        }
    }
}
#endif
