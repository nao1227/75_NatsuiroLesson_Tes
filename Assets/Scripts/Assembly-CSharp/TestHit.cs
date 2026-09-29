using Live2D.Cubism.Framework.Raycasting;
using UnityEngine;

public class TestHit : MonoBehaviour
{
	private CubismRaycaster _raycaster;

	private void Start()
	{
		_raycaster = GetComponent<CubismRaycaster>();
	}

	private void Update()
	{
		CubismRaycastHit[] array = new CubismRaycastHit[4];
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		_raycaster.Raycast(ray, array);
		CubismRaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			CubismRaycastHit cubismRaycastHit = array2[i];
			if (cubismRaycastHit.Drawable == null)
			{
				break;
			}
			Debug.Log("TEST TOUCHABLE RESULTS :" + cubismRaycastHit.Drawable?.name);
		}
	}
}
