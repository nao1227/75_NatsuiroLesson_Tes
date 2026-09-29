using System.Threading;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Rendering;
using UnityEngine;

public class PenisFollowerFellatio : MonoBehaviour
{
	public CubismRenderer PenisCubismRenderer;

	public CubismRenderer VaginaCubismRenderer;

	public CubismRenderer ManBodyCubismRenderer;

	private bool _enabled;

	public float Factor = 1f;

	public float YOffset;

	protected bool _isSucking;

	private float penisTop;

	private float penisBottom;

	private float vaginaTop;

	private float manBodyTop;

	[SerializeField]
	private float ScaleMin = 0.5f;

	private MeshRenderer _mesh;

	private void Start()
	{
		Bounds bounds = PenisCubismRenderer.Mesh.bounds;
		base.transform.position = new Vector3(bounds.center.x, bounds.center.y, -1f);
		_mesh = GetComponent<MeshRenderer>();
		Follow(this.GetCancellationTokenOnDestroy()).Forget();
	}

	public void SetSucking(bool val)
	{
		_isSucking = val;
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
			_mesh.enabled = _enabled;
			Bounds bounds = PenisCubismRenderer.Mesh.bounds;
			Bounds bounds2 = VaginaCubismRenderer.Mesh.bounds;
			Bounds bounds3 = ManBodyCubismRenderer.Mesh.bounds;
			penisTop = bounds.center.y + bounds.size.y / 2f;
			vaginaTop = bounds2.center.y + bounds2.size.y / 2f;
			manBodyTop = bounds3.center.y + bounds3.size.y / 2f;
			penisBottom = bounds.center.y - bounds.size.y / 2f;
			float num = Mathf.Min(vaginaTop, penisTop);
			if (_isSucking)
			{
				num = penisTop;
			}
			float num2 = manBodyTop;
			float num3 = (num + num2) / 2f;
			base.transform.position = new Vector3(bounds.center.x, num3 + YOffset, -1f);
			base.transform.localScale = new Vector3(0.05f, (num - num2) * Factor, 1f);
			await UniTask.Yield(token);
		}
	}

	public void SetEnabled(bool val)
	{
		_enabled = val;
	}
}
