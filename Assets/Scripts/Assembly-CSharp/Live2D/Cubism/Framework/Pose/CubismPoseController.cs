using System;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.Pose
{
	public sealed class CubismPoseController : MonoBehaviour, ICubismUpdatable
	{
		[SerializeField]
		public int defaultPoseIndex;

		private const float BackOpacityThreshold = 0.15f;

		private CubismModel _model;

		private CubismPoseData[][] _poseData;

		[HideInInspector]
		public bool HasUpdateController { get; set; }

		public int ExecutionOrder => CubismUpdateExecutionOrder.CubismPoseController;

		public bool NeedsUpdateOnEditing => false;

		public void Refresh()
		{
			_model = this.FindCubismModel();
			if (_model == null)
			{
				return;
			}
			Component[] parts = _model.Parts;
			CubismPosePart[] componentsMany = parts.GetComponentsMany<CubismPosePart>();
			for (int i = 0; i < componentsMany.Length; i++)
			{
				int groupIndex = componentsMany[i].GroupIndex;
				int partIndex = componentsMany[i].PartIndex;
				if (_poseData == null || _poseData.Length <= groupIndex)
				{
					Array.Resize(ref _poseData, groupIndex + 1);
				}
				if (_poseData[groupIndex] == null || _poseData[groupIndex].Length <= partIndex)
				{
					Array.Resize(ref _poseData[groupIndex], partIndex + 1);
				}
				_poseData[groupIndex][partIndex].PosePart = componentsMany[i];
				_poseData[groupIndex][partIndex].Part = componentsMany[i].GetComponent<CubismPart>();
				defaultPoseIndex = ((defaultPoseIndex >= 0) ? defaultPoseIndex : 0);
				if (partIndex != defaultPoseIndex)
				{
					_poseData[groupIndex][partIndex].Part.Opacity = 0f;
				}
				_poseData[groupIndex][partIndex].Opacity = _poseData[groupIndex][partIndex].Part.Opacity;
				if (componentsMany[i].Link != null && componentsMany[i].Link.Length != 0)
				{
					_poseData[groupIndex][partIndex].LinkParts = new CubismPart[componentsMany[i].Link.Length];
					for (int j = 0; j < componentsMany[i].Link.Length; j++)
					{
						string id = componentsMany[i].Link[j];
						_poseData[groupIndex][partIndex].LinkParts[j] = _model.Parts.FindById(id);
					}
				}
			}
			HasUpdateController = GetComponent<CubismUpdateController>() != null;
		}

		private void DoFade()
		{
			for (int i = 0; i < _poseData.Length; i++)
			{
				int num = -1;
				float num2 = 1f;
				for (int j = 0; j < _poseData[i].Length; j++)
				{
					CubismPart part = _poseData[i][j].Part;
					if (part.Opacity > _poseData[i][j].Opacity)
					{
						num = j;
						num2 = part.Opacity;
						break;
					}
				}
				if (num < 0)
				{
					break;
				}
				for (int k = 0; k < _poseData[i].Length; k++)
				{
					if (k != num)
					{
						CubismPart part2 = _poseData[i][k].Part;
						float num3 = part2.Opacity;
						if ((1f - num3) * (1f - num2) > 0.15f)
						{
							num3 = 1f - 0.15f / (1f - num2);
						}
						if (part2.Opacity > num3)
						{
							part2.Opacity = num3;
						}
					}
				}
			}
		}

		private void CopyPartOpacities()
		{
			for (int i = 0; i < _poseData.Length; i++)
			{
				for (int j = 0; j < _poseData[i].Length; j++)
				{
					CubismPart[] linkParts = _poseData[i][j].LinkParts;
					if (linkParts == null)
					{
						continue;
					}
					float opacity = _poseData[i][j].Part.Opacity;
					foreach (CubismPart cubismPart in linkParts)
					{
						if (cubismPart != null)
						{
							cubismPart.Opacity = opacity;
						}
					}
				}
			}
		}

		private void SavePartOpacities()
		{
			for (int i = 0; i < _poseData.Length; i++)
			{
				for (int j = 0; j < _poseData[i].Length; j++)
				{
					_poseData[i][j].Opacity = _poseData[i][j].Part.Opacity;
				}
			}
		}

		public void OnLateUpdate()
		{
			if (base.enabled && !(_model == null) && _poseData != null)
			{
				DoFade();
				CopyPartOpacities();
				SavePartOpacities();
			}
		}

		private void OnEnable()
		{
			Refresh();
		}

		private void LateUpdate()
		{
			if (!HasUpdateController)
			{
				OnLateUpdate();
			}
		}
	}
}
