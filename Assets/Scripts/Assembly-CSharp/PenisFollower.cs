using System.Threading;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Rendering;
using UnityEngine;

public class PenisFollower : MonoBehaviour
{
	public CubismRenderer PenisCubismRenderer;

	public CubismRenderer VaginaCubismRenderer;

	public CubismRenderer ManBodyCubismRenderer;

	private float _pistonValue = -2f;

	public float YRatio;

	[SerializeField]
	private float PositionYFactor = -0.08f;

	[SerializeField]
	private float ScaleMin = 0.5f;

	private MeshRenderer _mesh;

	private void Start()
	{
		Bounds bounds = PenisCubismRenderer.Mesh.bounds;
		base.transform.position = new Vector3(bounds.center.x, bounds.center.y + PositionYFactor, -1f);
		_mesh = GetComponent<MeshRenderer>();
		Follow(this.GetCancellationTokenOnDestroy()).Forget();
	}

	private async UniTask Follow(CancellationToken token)
	{
		bool follow = true;
		while (follow)
		{
			if (token.IsCancellationRequested)
			{
				follow = false;
			}
			_mesh.enabled = _pistonValue > -2f;
			Bounds bounds = PenisCubismRenderer.Mesh.bounds;
			Bounds bounds2 = VaginaCubismRenderer.Mesh.bounds;
			Bounds bounds3 = ManBodyCubismRenderer.Mesh.bounds;
			float a = bounds.center.y + bounds.size.y / 2f;
			float b = bounds2.center.y + bounds2.size.y / 2f;
			float b2 = bounds3.center.y + bounds3.size.y / 2f;
			float a2 = bounds.center.y - bounds.size.y / 2f;
			float num = Mathf.Min(a, b);
			float num2 = Mathf.Max(a2, b2);
			float num3 = (num + num2) / 2f;
			base.transform.position = new Vector3(bounds.center.x, num3 + PositionYFactor, -1f);
			base.transform.localScale = new Vector3(0.05f, (num - num2) * YRatio, 1f);
			await UniTask.Yield(token);
		}
	}

	public void SetPistonValue(float val)
	{
		_pistonValue = val;
	}
}
