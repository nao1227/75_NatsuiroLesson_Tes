using System;
using System.Collections.Generic;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using UnityEngine;
using UnityEngine.Rendering;

namespace Live2D.Cubism.Rendering.Masking
{
	[ExecuteInEditMode]
	[CubismDontMoveOnReimport]
	public sealed class CubismMaskController : MonoBehaviour, ICubismMaskTextureCommandSource, ICubismMaskCommandSource, ICubismUpdatable
	{
		private struct MasksMaskedsPair
		{
			public CubismRenderer[] Masks;

			public List<CubismRenderer> Maskeds;
		}

		private class MasksMaskedsPairs
		{
			public List<MasksMaskedsPair> Entries = new List<MasksMaskedsPair>();

			public void Add(CubismDrawable masked, CubismDrawable[] masks)
			{
				for (int i = 0; i < Entries.Count; i++)
				{
					bool flag = Entries[i].Masks.Length == masks.Length;
					if (!flag)
					{
						continue;
					}
					for (int j = 0; j < Entries[i].Masks.Length; j++)
					{
						if (Entries[i].Masks[j] != masks[j].GetComponent<CubismRenderer>())
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						Entries[i].Maskeds.Add(masked.GetComponent<CubismRenderer>());
						return;
					}
				}
				CubismRenderer[] array = new CubismRenderer[masks.Length];
				for (int k = 0; k < masks.Length; k++)
				{
					array[k] = masks[k].GetComponent<CubismRenderer>();
				}
				Entries.Add(new MasksMaskedsPair
				{
					Masks = array,
					Maskeds = new List<CubismRenderer> { masked.GetComponent<CubismRenderer>() }
				});
			}
		}

		[SerializeField]
		[HideInInspector]
		private CubismMaskTexture _maskTexture;

		public CubismMaskTexture MaskTexture
		{
			get
			{
				if (_maskTexture == null)
				{
					_maskTexture = CubismMaskTexture.GlobalMaskTexture;
				}
				return _maskTexture;
			}
			set
			{
				if (!(value == _maskTexture))
				{
					_maskTexture = value;
					OnDestroy();
					Start();
				}
			}
		}

		private CubismMaskMaskedJunction[] Junctions { get; set; }

		private bool IsRevived => Junctions != null;

		[HideInInspector]
		public bool HasUpdateController { get; set; }

		public int ExecutionOrder => CubismUpdateExecutionOrder.CubismMaskController;

		public bool NeedsUpdateOnEditing => true;

		private void TryRevive()
		{
			if (!IsRevived)
			{
				ForceRevive();
			}
		}

		private void ForceRevive()
		{
			CubismDrawable[] drawables = this.FindCubismModel().Drawables;
			MasksMaskedsPairs masksMaskedsPairs = new MasksMaskedsPairs();
			for (int i = 0; i < drawables.Length; i++)
			{
				if (drawables[i].IsMasked)
				{
					CubismDrawable[] array = Array.FindAll(drawables[i].Masks, (CubismDrawable mask) => mask != null);
					if (array.Length != 0)
					{
						masksMaskedsPairs.Add(drawables[i], array);
					}
				}
			}
			Junctions = new CubismMaskMaskedJunction[masksMaskedsPairs.Entries.Count];
			for (int num = 0; num < Junctions.Length; num++)
			{
				CubismMaskRenderer[] array2 = new CubismMaskRenderer[masksMaskedsPairs.Entries[num].Masks.Length];
				for (int num2 = 0; num2 < array2.Length; num2++)
				{
					array2[num2] = new CubismMaskRenderer().SetMainRenderer(masksMaskedsPairs.Entries[num].Masks[num2]);
				}
				Junctions[num] = new CubismMaskMaskedJunction().SetMasks(array2).SetMaskeds(masksMaskedsPairs.Entries[num].Maskeds.ToArray()).SetMaskTexture(MaskTexture);
			}
		}

		public void OnLateUpdate()
		{
			if (base.enabled && IsRevived)
			{
				for (int i = 0; i < Junctions.Length; i++)
				{
					Junctions[i].Update();
				}
			}
		}

		private void Start()
		{
			if (!(MaskTexture == null))
			{
				MaskTexture.AddSource(this);
				HasUpdateController = GetComponent<CubismUpdateController>() != null;
			}
		}

		private void LateUpdate()
		{
			if (!HasUpdateController)
			{
				OnLateUpdate();
			}
		}

		private void OnDestroy()
		{
			if (!(MaskTexture == null))
			{
				MaskTexture.RemoveSource(this);
			}
		}

		int ICubismMaskTextureCommandSource.GetNecessaryTileCount()
		{
			TryRevive();
			return Junctions.Length;
		}

		void ICubismMaskTextureCommandSource.SetTiles(CubismMaskTile[] value)
		{
			for (int i = 0; i < Junctions.Length; i++)
			{
				Junctions[i].SetMaskTile(value[i]);
			}
		}

		void ICubismMaskCommandSource.AddToCommandBuffer(CommandBuffer buffer)
		{
			for (int i = 0; i < Junctions.Length; i++)
			{
				Junctions[i].AddToCommandBuffer(buffer);
			}
		}
	}
}
