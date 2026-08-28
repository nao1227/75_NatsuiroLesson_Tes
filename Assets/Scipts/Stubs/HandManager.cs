using System.Collections.Generic;

namespace Stubs
{
    public class HandManager
    {
        public bool IsAnyHandEmpty = true;
        public bool IsGrabbingAny = false;

        public bool IsGrabbing(AbstractOsawari target)
        {
            return false;
        }

        public List<Hand> GetHandGrabbing(AbstractOsawari target) { return new List<Hand>(); }
        public Hand GetHandToUse() { return new Hand(); }
        public void Grab(HandType type, AbstractOsawari target) { }
        public void Release(HandType type) { }
        public void MoveHand(HandType type, ParameterValue value) { }
    }
}