#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Gets the text alignment of a TextMeshPro component.")]
    public class TmpTextGetAlignment : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [ObjectType(typeof(TextAlignmentOptions))]
        [Tooltip("Store the alignment in an enum variable.")]
        public FsmEnum alignment;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;

        public override void Reset()
        {
            gameObject = null;
            alignment = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                tmpText = cachedComponent;
            }

            DoGetAlignment();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoGetAlignment();
        }

        private void DoGetAlignment()
        {
            if (tmpText == null) return;
            alignment.Value = tmpText.alignment;
        }
    }
}
#endif
