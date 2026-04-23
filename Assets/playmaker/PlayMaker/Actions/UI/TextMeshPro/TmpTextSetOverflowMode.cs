#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the text overflow mode of a TextMeshPro component (Overflow, Ellipsis, Masking, Truncate, ScrollRect, Page, Linked).")]
    public class TmpTextSetOverflowMode : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [ObjectType(typeof(TextOverflowModes))]
        [Tooltip("Overflow mode: Overflow, Ellipsis, Masking, Truncate, ScrollRect, Page, Linked.")]
        public FsmEnum overflowMode;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        private TMP_Text tmpText;
        private TextOverflowModes originalValue;

        public override void Reset()
        {
            gameObject = null;
            overflowMode = TextOverflowModes.Overflow;
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
                originalValue = tmpText.overflowMode;
            }

            DoSetValue();
            Finish();
        }

        private void DoSetValue()
        {
            if (tmpText == null) return;
            tmpText.overflowMode = (TextOverflowModes)overflowMode.Value;
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.overflowMode = originalValue;
            }
        }
    }
}
#endif
