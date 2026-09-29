using System;
using System.Collections.Generic;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework
{
	[ExecuteInEditMode]
	public class CubismUpdateController : MonoBehaviour
	{
		private Action _onLateUpdate;

		public void Refresh()
		{
			CubismModel cubismModel = this.FindCubismModel();
			if (cubismModel == null)
			{
				return;
			}
			_onLateUpdate = null;
			List<ICubismUpdatable> list = new List<ICubismUpdatable>(cubismModel.GetComponents<ICubismUpdatable>());
			CubismUpdateExecutionOrder.SortByExecutionOrder(list);
			foreach (ICubismUpdatable item in list)
			{
				_onLateUpdate = (Action)Delegate.Combine(_onLateUpdate, new Action(item.OnLateUpdate));
			}
		}

		private void Start()
		{
			Refresh();
		}

		private void LateUpdate()
		{
			if (_onLateUpdate != null)
			{
				_onLateUpdate();
			}
		}
	}
}
