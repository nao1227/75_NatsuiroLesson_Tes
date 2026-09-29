namespace Paidia.satsuki1
{
	public class SceneContextManager : SingletonManager<SceneContextManager>
	{
		public SceneContext CurrentSceneContext;

		public bool BlackScreenFadeInEnabled;

		public bool IsDayRefreshed;

		public bool PlayOP;

		public bool IsSaveEnabled = true;

		public bool PlayingMovie;

		public bool AllowOsawari;

		public bool AllowUtage = true;

		public bool IsFocusOn = true;

		public bool EjaculationPlus;

		public SceneName FreeHTargetScene;

		public int FreeHCloth;

		public int FreeHUnderwear;

		public int LastOpenedSaveLoadPage;

		public bool AllowUtageInput
		{
			get
			{
				if (AllowUtage)
				{
					return IsFocusOn;
				}
				return false;
			}
		}
	}
}
