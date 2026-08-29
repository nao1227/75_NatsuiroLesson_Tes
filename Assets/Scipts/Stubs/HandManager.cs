using System.Collections.Generic;
namespace Stubs
{
    public class HandManager
    {
        public bool IsAnyHandEmpty = true;
        public bool IsGrabbingAny = false;

        public bool IsGrabbing(global::AbstractOsawari target)   // ← global:: がついているか確認
        {
            return false;
        }

        public List<Hand> GetHandGrabbing(global::AbstractOsawari target) { return new List<Hand>(); }
        public Hand GetHandToUse() { return new Hand(); }
        public void Grab(HandType type, global::AbstractOsawari target) { }
        public void Release(HandType type) { }
        public void MoveHand(HandType type, ParameterValue value) { }
    }
}