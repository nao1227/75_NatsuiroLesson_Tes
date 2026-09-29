using System.Threading;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Rendering;
using UnityEngine;

public class PenisFollowerHScene2 : MonoBehaviour
{
	public CubismRenderer PenisCubismRenderer;

	public CubismRenderer VaginaCubismRenderer;

	public CubismRenderer ManBodyCubismRenderer;

	[SerializeField]
	private float PositionYFactor = -0.08f;

	[SerializeField]
	private float ScaleMin = 0.5f;

	public float PositionXFactor;

	public float ScaleX = 1f;

	private void Start()
	{
		Bounds bounds = PenisCubismRenderer.Mesh.bounds;
		base.transform.position = new Vector3(bounds.center.x, bounds.center.y + PositionYFactor, base.transform.position.z);
		Follow(this.GetCancellationTokenOnDestroy()).Forget();
	}

	public void SetEnable(bool enable)
	{
		GetComponent<MeshRenderer>().enabled = enable;
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
			base.transform.position = new Vector3(bounds.center.x + PositionXFactor, num3 + PositionYFactor, base.transform.position.z);
			base.transform.localScale = new Vector3(ScaleX, num - num2, 1f);
			await UniTask.Yield(token);
		}
	}
}
