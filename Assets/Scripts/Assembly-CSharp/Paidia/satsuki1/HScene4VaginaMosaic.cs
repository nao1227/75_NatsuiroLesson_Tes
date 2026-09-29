using UnityEngine;

namespace Paidia.satsuki1
{
	public class HScene4VaginaMosaic : MonoBehaviour
	{
		private MeshRenderer _meshRenderer;

		public HScene4OsawariPiston _piston;

		private OsawariContext _context;

		private async void Start()
		{
			_meshRenderer = GetComponent<MeshRenderer>();
			_meshRenderer.enabled = false;
		}

		public void SetContext(OsawariContext context)
		{
			_context = context;
		}

		private void Update()
		{
			try
			{
				_meshRenderer.enabled = _piston.IsAbleToInsert() && _context == OsawariContext.Osawari;
			}
			catch
			{
			}
		}
	}
}
