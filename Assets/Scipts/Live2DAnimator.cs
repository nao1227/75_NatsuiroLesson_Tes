using UnityEngine;

namespace Paidia.satsuki1
{
    public class Live2DAnimator : MonoBehaviour
    {
        public Animator Animator;

        public void SetBool(string name, bool on)
        {
            Debug.Log($"[Live2DAnimator] SetBool {name} = {on}");  // 学習用。動作確認後に消してOK
            if (Animator != null) Animator.SetBool(name, on);
        }
    }
}