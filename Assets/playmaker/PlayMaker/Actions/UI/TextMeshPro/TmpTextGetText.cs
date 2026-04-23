#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Gets the text of a TextMeshPro component.")]
    public class TmpTextGetText : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the text value in a string variable.")]
        public FsmString text;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;

        public override void Reset()
        {
            gameObject = null;
            text = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                tmpText = cachedComponent;
            }

            DoGetText();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoGetText();
        }

        private void DoGetText()
        {
            if (tmpText == null) return;
            text.Value = tmpText.text;
        }
    }
}
#endif
