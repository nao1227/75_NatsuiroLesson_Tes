using UnityEngine;

namespace Paidia.satsuki1
{
	public class VaginaMosaicHScene3 : MonoBehaviour
	{
		private MeshRenderer _meshRenderer;

		public OsawariPants _pants;

		public OsawariLeg Leg;

		public HScene3OsawariHelper Helper;

		private void Start()
		{
			_meshRenderer = GetComponent<MeshRenderer>();
		}

		private void Update()
		{
			if (Helper.ClothStatus == ClothStatus.Naked)
			{
				_meshRenderer.enabled = Leg.IsMosaicNeeded();
			}
			else
			{
				_meshRenderer.enabled = _pants.Value > 0.5f || _pants.IsAbleToInsert();
			}
		}
	}
}
