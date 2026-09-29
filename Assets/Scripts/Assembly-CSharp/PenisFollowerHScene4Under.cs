using System.Threading;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Rendering;
using Paidia.satsuki1;
using UnityEngine;

public class PenisFollowerHScene4Under : MonoBehaviour
{
	public CubismRenderer PenisCubismRenderer;

	public CubismRenderer UpperRenderer;

	public float XScale;

	private MeshRenderer _mesh;

	public OsawariContext Context;

	private bool _enabled;

	private void Start()
	{
		Bounds bounds = PenisCubismRenderer.Mesh.bounds;
		base.transform.position = new Vector3(bounds.center.x, bounds.center.y, -1f);
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
			_mesh.enabled = _enabled && Context == OsawariContext.Paizuri;
			Bounds bounds = PenisCubismRenderer.Mesh.bounds;
			Bounds bounds2 = UpperRenderer.Mesh.bounds;
			_ = bounds.center;
			_ = bounds.size.y / 2f;
			float num = bounds.center.y - bounds.size.y / 2f;
			float num2 = bounds2.center.y - bounds2.size.y / 2f;
			float num3 = num;
			float y = (num2 + num3) / 2f;
			base.transform.position = new Vector3(bounds.center.x, y, -1f);
			base.transform.localScale = new Vector3(XScale, num2 - num3, 1f);
			await UniTask.Yield(token);
		}
	}

	public void SetEnabled(bool enabled)
	{
		_enabled = enabled;
	}

	public void SetContext(OsawariContext context)
	{
		Context = context;
	}
}
