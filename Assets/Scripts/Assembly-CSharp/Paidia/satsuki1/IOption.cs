using UniRx;

namespace Paidia.satsuki1
{
	public interface IOption
	{
		IReactiveProperty<BaseScene.MenuPhase> GetMenuPhase();

		void SetActiveSceneModalWindowVisible(bool visible);

		bool IsLoaded();
	}
}
