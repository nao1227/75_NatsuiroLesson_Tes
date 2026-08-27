using System.Collections;
using System.Collections.Generic;

namespace Stubs
{
    public class ContextOsawariTargetList : IEnumerable<List<AbstractOsawari>>
    {
        private List<AbstractOsawari> _registeredTargets = new List<AbstractOsawari>();

           public void RegisterTargets(List<AbstractOsawari> targets)
    {
        _registeredTargets = targets;
    }
        public List<AbstractOsawari> GetOsawariTargets(OsawariContext context)
        {
            return _registeredTargets;
        }

        public IEnumerator<List<AbstractOsawari>> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public System.Collections.Generic.List<System.Collections.Generic.List<AbstractOsawari>> GetOsawariTargetsOfNotInContext(OsawariContext context)
        {
            return new System.Collections.Generic.List<System.Collections.Generic.List<AbstractOsawari>>();
        }
    }
}