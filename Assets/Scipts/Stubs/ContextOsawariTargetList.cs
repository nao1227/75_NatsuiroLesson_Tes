using System.Collections;
using System.Collections.Generic;

namespace Stubs
{
    public class ContextOsawariTargetList : IEnumerable<List<global::AbstractOsawari>>
    {
        private List<global::AbstractOsawari> _registeredTargets = new List<global::AbstractOsawari>();

        public void RegisterTargets(List<global::AbstractOsawari> targets)
        {
            _registeredTargets = targets;
        }

        public List<global::AbstractOsawari> GetOsawariTargets(OsawariContext context)
        {
            return _registeredTargets;
        }

        public IEnumerator<List<global::AbstractOsawari>> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<List<global::AbstractOsawari>> GetOsawariTargetsOfNotInContext(OsawariContext context)
        {
            return new List<List<global::AbstractOsawari>>();
        }
    }
}