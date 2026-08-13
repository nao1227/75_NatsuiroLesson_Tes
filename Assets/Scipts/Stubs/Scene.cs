using UniRx;

namespace Stubs
{
    public class Scene
    {
        public BoolReactiveProperty IsModalWindowOpen = new BoolReactiveProperty(false);
        public BoolReactiveProperty IsResultWindowOpen = new BoolReactiveProperty(false);
    }
}