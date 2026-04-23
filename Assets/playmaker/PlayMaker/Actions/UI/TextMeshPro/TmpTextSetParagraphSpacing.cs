#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the paragraph spacing of a TextMeshPro component (in em units).")]
    public class TmpTextSetParagraphSpacing : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Paragraph spacing in em units.")]
        public FsmFloat paragraphSpacing;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;
        private float originalValue;

        public override void Reset()
        {
            gameObject = null;
            paragraphSpacing = 0f;
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
                originalValue = tmpText.paragraphSpacing;
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
            if (!paragraphSpacing.IsNone)
            {
                tmpText.paragraphSpacing = paragraphSpacing.Value;
            }
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.paragraphSpacing = originalValue;
            }
        }
    }
}
#endif
