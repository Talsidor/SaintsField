using UnityEngine;

namespace SaintsField.Samples.Scripts
{
    public class AnimSubStateMachineExample : MonoBehaviour
    {
        [AnimatorState, OnValueChanged(nameof(OnChanged))]
        public AnimatorState state;

        // This does not have a `animationClip`, thus it won't include a resource target when serialized: only pure data.
        [AnimatorState, OnValueChanged(nameof(OnChanged))]
        public AnimatorStateBase stateBase;

        [AnimatorState, OnValueChanged(nameof(OnChanged))]
        public string stateName;

        private void OnChanged(object changedValue) => Debug.Log(changedValue);
    }
}
