using System.Collections;
using System.Collections.Generic;

namespace Stubs
{
    public class ContextOsawariTargetList : IEnumerable<List<AbstractOsawari>>
    {
        public List<AbstractOsawari> GetOsawariTargets(OsawariContext context)
        {
            return new List<AbstractOsawari>();
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