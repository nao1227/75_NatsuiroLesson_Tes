using Live2D.Cubism.Rendering;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class VaginaMosaic : MonoBehaviour
	{
		public CubismRenderer VaginaCubismRenderer;

		public CubismRenderer VaginaLowerCubismRenderer;

		public CubismRenderer ManBodyCubismRenderer;

		public float YFactor;

		public float YOffset;

		private MeshRenderer _meshRenderer;

		private float _originalHeight;

		private Vector3 _originalScale;

		public OsawariPants _pants;

		public OsawariPiston _piston;

		private ContextManager _contextManager;

		private void Start()
		{
			_originalHeight = GetComponent<MeshRenderer>().bounds.size.y;
			_originalScale = base.transform.localScale;
			_meshRenderer = GetComponent<MeshRenderer>();
			_contextManager = Object.FindObjectOfType<ContextManager>();
		}

		private void Update()
		{
			if (_contextManager.Context == OsawariContext.Fellatio)
			{
				_meshRenderer.enabled = false;
				return;
			}
			_meshRenderer.enabled = _pants.Value > 0.01f || _piston.IsAbleToInsert();
			Vector3 position = base.transform.position;
			Bounds bounds = VaginaCubismRenderer.Mesh.bounds;
			Bounds bounds2 = VaginaLowerCubismRenderer.Mesh.bounds;
			Bounds bounds3 = ManBodyCubismRenderer.Mesh.bounds;
			float num = bounds.center.y + bounds.size.y / 2f;
			float a = bounds2.center.y - bounds2.size.y / 2f;
			float b = bounds3.center.y + bounds3.size.y / 2f;
			float num2 = Mathf.Max(a, b);
			float num3 = num - num2;
			base.transform.position = new Vector3(position.x, (num2 + num) / 2f + YOffset, position.z);
			base.transform.localScale = new Vector3(_originalScale.x, num3 * YFactor, _originalScale.z);
		}
	}
}
