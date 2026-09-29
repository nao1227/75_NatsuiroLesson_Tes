namespace Live2D.Cubism.Framework
{
	public interface ICubismUpdatable
	{
		int ExecutionOrder { get; }

		bool NeedsUpdateOnEditing { get; }

		bool HasUpdateController { get; set; }

		void OnLateUpdate();
	}
}
