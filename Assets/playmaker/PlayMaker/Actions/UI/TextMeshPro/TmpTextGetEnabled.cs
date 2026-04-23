#if PLAYMAKER_TMPRO
using TMPro;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TextMeshPro")]
    [Tooltip("Gets whether a TextMeshPro component is enabled.")]
    public class TmpTextGetEnabled : ComponentAction<TMP_Text>
    {
        [RequiredField]
        [CheckForComponent(typeof(TMP_Text))]
        [Tooltip("The GameObject with the TextMeshPro component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the enabled state.")]
        public FsmBool enabled;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private TMP_Text tmpText;

        public override void Reset()
        {
            gameObject = null;
            enabled = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                tmpText = cachedComponent;
            }

            DoGetValue();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoGetValue();
        }

        private void DoGetValue()
        {
            if (tmpText == null) return;
            enabled.Value = tmpText.enabled;
        }
    }
}
#endif
