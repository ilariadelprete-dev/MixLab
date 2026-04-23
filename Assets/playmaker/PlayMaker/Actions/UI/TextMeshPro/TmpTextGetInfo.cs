#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Gets read-only information from a TextMeshPro component: character count, word count, line count, page count, and rendered text.")]
    public class TmpTextGetInfo : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Total number of characters (including invisible ones).")]
        public FsmInt characterCount;

        [UIHint(UIHint.Variable)]
        [Tooltip("Number of words.")]
        public FsmInt wordCount;

        [UIHint(UIHint.Variable)]
        [Tooltip("Number of lines.")]
        public FsmInt lineCount;

        [UIHint(UIHint.Variable)]
        [Tooltip("Number of pages.")]
        public FsmInt pageCount;

        [UIHint(UIHint.Variable)]
        [Tooltip("The rendered/parsed text (after rich-text tags processing).")]
        public FsmString renderedText;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;

        public override void Reset()
        {
            gameObject = null;
            characterCount = null;
            wordCount = null;
            lineCount = null;
            pageCount = null;
            renderedText = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                tmpText = cachedComponent;
            }

            DoGetInfo();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoGetInfo();
        }

        private void DoGetInfo()
        {
            if (tmpText == null) return;

            var info = tmpText.textInfo;

            if (!characterCount.IsNone) characterCount.Value = info.characterCount;
            if (!wordCount.IsNone)      wordCount.Value      = info.wordCount;
            if (!lineCount.IsNone)      lineCount.Value      = info.lineCount;
            if (!pageCount.IsNone)      pageCount.Value      = info.pageCount;
            if (!renderedText.IsNone)   renderedText.Value   = tmpText.GetParsedText();
        }
    }
}
#endif
