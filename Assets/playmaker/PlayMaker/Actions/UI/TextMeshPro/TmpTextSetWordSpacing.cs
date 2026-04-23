#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Sets the word spacing of a TextMeshPro component (in em units).")]
    public class TmpTextSetWordSpacing : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Word spacing in em units.")]
        public FsmFloat wordSpacing;

        [Tooltip("Reset to original value when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;
        private float originalValue;

        public override void Reset()
        {
            gameObject = null;
            wordSpacing = 0f;
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
                originalValue = tmpText.wordSpacing;
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
            if (!wordSpacing.IsNone)
            {
                tmpText.wordSpacing = wordSpacing.Value;
            }
        }

        public override void OnExit()
        {
            if (tmpText == null) return;
            if (resetOnExit.Value)
            {
                tmpText.wordSpacing = originalValue;
            }
        }
    }
}
#endif
