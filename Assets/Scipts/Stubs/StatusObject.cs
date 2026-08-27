using UnityEngine;

namespace Stubs
{
    public class StatusObject : MonoBehaviour
    {
        public TemporaryStatus TemporaryStatus = new TemporaryStatus();
        public PersistantStatus PersistantStatus = new PersistantStatus();
    }
}