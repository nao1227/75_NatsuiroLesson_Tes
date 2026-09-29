using System.Threading;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using Paidia.satsuki1;
using UnityEngine;

public class PenisFollowerHScene4Normal : MonoBehaviour
{
	public CubismRenderer PenisCubismRenderer;

	public CubismRenderer VaginaCubismRenderer;

	public float XScale;

	private MeshRenderer _mesh;

	private float _pistonValue;

	private bool _enabled;

	private OsawariContext _context;

	public CubismModel Model;

	public int PistonParamIndex;

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
			_mesh.enabled = _enabled && _context == OsawariContext.Osawari;
			Bounds bounds = PenisCubismRenderer.Mesh.bounds;
			Bounds bounds2 = VaginaCubismRenderer.Mesh.bounds;
			float num = bounds.center.y + bounds.size.y / 2f;
			float num2 = bounds.center.y - bounds.size.y / 2f;
			float num3 = bounds2.center.y + bounds2.size.y / 2f;
			float num4 = num2;
			float num5 = ((!(Model.Parameters[PistonParamIndex].Value < -0.5f)) ? num3 : num);
			float y = (num5 + num4) / 2f;
			base.transform.position = new Vector3(bounds.center.x, y, -1f);
			base.transform.localScale = new Vector3(XScale, num5 - num4, 1f);
			await UniTask.Yield(token);
		}
	}

	public void SetEnabled(bool enabled)
	{
		_enabled = enabled;
	}

	public void SetContext(OsawariContext context)
	{
		_context = context;
	}

	public void SetPistonValue(float value)
	{
		_pistonValue = value;
	}
}
