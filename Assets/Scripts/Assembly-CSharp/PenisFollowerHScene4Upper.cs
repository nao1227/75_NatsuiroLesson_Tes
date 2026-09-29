using System.Threading;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Rendering;
using Paidia.satsuki1;
using UnityEngine;

public class PenisFollowerHScene4Upper : MonoBehaviour
{
	public CubismRenderer PenisCubismRenderer;

	public CubismRenderer UnderSideRenderer;

	private MeshRenderer _mesh;

	public float XScale;

	private bool _enabled;

	public OsawariContext Context;

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
			Bounds bounds2 = UnderSideRenderer.Mesh.bounds;
			float num = bounds.center.y + bounds.size.y / 2f;
			float num2 = bounds2.center.y + bounds2.size.y / 2f;
			float num3 = num;
			float num4 = num2;
			float y = (num3 + num4) / 2f;
			base.transform.position = new Vector3(bounds.center.x, y, -1f);
			base.transform.localScale = new Vector3(XScale, num3 - num4, 1f);
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
