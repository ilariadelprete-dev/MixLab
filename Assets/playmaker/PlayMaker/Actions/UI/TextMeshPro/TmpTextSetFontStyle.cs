#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the font style of a TextMeshPro component (Bold, Italic, Underline, Strikethrough, etc.).")]
    public class TmpTextSetFontStyle : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [ObjectType(typeof(FontStyles))]
        [Tooltip("The font style to apply. FontStyles is a flags enum: Normal, Bold, Italic, Underline, LowerCase, UpperCase, SmallCaps, Strikethrough, Superscript, Subscript, Highlight.")]
        public FsmEnum fontStyle;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        private TMP_Text tmpText;
        private FontStyles originalStyle;

        public override void Reset()
        {
            gameObject = null;
            fontStyle = FontStyles.Normal;
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
                originalStyle = tmpText.fontStyle;
            }

            DoSetFontStyle();
            Finish();
        }

        private void DoSetFontStyle()
        {
            if (tmpText == null) return;
            tmpText.fontStyle = (FontStyles)fontStyle.Value;
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.fontStyle = originalStyle;
            }
        }
    }
}
#endif
