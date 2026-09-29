using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Live2D.Cubism.Rendering.Masking
{
	[CreateAssetMenu(menuName = "Live2D Cubism/Mask Texture")]
	public sealed class CubismMaskTexture : ScriptableObject, ICubismMaskCommandSource
	{
		private struct SourcesItem
		{
			public ICubismMaskTextureCommandSource Source;

			public CubismMaskTile[] Tiles;
		}

		[SerializeField]
		[HideInInspector]
		private int _size = 1024;

		[SerializeField]
		[HideInInspector]
		private int _subdivisions = 3;

		private RenderTexture _renderTexture;

		public static CubismMaskTexture GlobalMaskTexture => Resources.Load<CubismMaskTexture>("Live2D/Cubism/GlobalMaskTexture");

		public int Size
		{
			get
			{
				return _size;
			}
			set
			{
				if (value != _size && value.IsPowerOfTwo())
				{
					_size = value;
					RefreshRenderTexture();
				}
			}
		}

		public int Channels => 4;

		public int Subdivisions
		{
			get
			{
				return _subdivisions;
			}
			set
			{
				if (value != _subdivisions)
				{
					_subdivisions = value;
					RefreshRenderTexture();
				}
			}
		}

		private CubismMaskTilePool TilePool { get; set; }

		private RenderTexture RenderTexture
		{
			get
			{
				if (_renderTexture == null)
				{
					RefreshRenderTexture();
				}
				return _renderTexture;
			}
			set
			{
				_renderTexture = value;
			}
		}

		private List<SourcesItem> Sources { get; set; }

		private bool IsRevived => TilePool != null;

		private bool ContainsSources
		{
			get
			{
				if (Sources != null)
				{
					return Sources.Count > 0;
				}
				return false;
			}
		}

		public static implicit operator Texture(CubismMaskTexture value)
		{
			return value.RenderTexture;
		}

		public void AddSource(ICubismMaskTextureCommandSource source)
		{
			TryRevive();
			if (Sources == null)
			{
				Sources = new List<SourcesItem>();
			}
			else if (Sources.FindIndex((SourcesItem i) => i.Source == source) != -1)
			{
				return;
			}
			SourcesItem item = new SourcesItem
			{
				Source = source,
				Tiles = TilePool.AcquireTiles(source.GetNecessaryTileCount())
			};
			Sources.Add(item);
			source.SetTiles(item.Tiles);
		}

		public void RemoveSource(ICubismMaskTextureCommandSource source)
		{
			if (ContainsSources)
			{
				int num = Sources.FindIndex((SourcesItem i) => i.Source == source);
				if (num != -1)
				{
					TilePool.ReturnTiles(Sources[num].Tiles);
					Sources.RemoveAt(num);
				}
			}
		}

		private void TryRevive()
		{
			if (!IsRevived)
			{
				RefreshRenderTexture();
			}
		}

		private void ReinitializeSources()
		{
			if (ContainsSources)
			{
				for (int i = 0; i < Sources.Count; i++)
				{
					SourcesItem value = Sources[i];
					value.Tiles = TilePool.AcquireTiles(value.Source.GetNecessaryTileCount());
					value.Source.SetTiles(value.Tiles);
					Sources[i] = value;
				}
			}
		}

		private void RefreshRenderTexture()
		{
			RenderTexture = new RenderTexture(Size, Size, 0, RenderTextureFormat.ARGB32);
			TilePool = new CubismMaskTilePool(Subdivisions, Channels);
			ReinitializeSources();
		}

		private void OnEnable()
		{
			CubismMaskCommandBuffer.AddSource(this);
		}

		private void OnDestroy()
		{
			CubismMaskCommandBuffer.RemoveSource(this);
		}

		void ICubismMaskCommandSource.AddToCommandBuffer(CommandBuffer buffer)
		{
			if (ContainsSources)
			{
				buffer.SetRenderTarget(RenderTexture);
				buffer.ClearRenderTarget(clearDepth: false, clearColor: true, Color.clear);
				for (int i = 0; i < Sources.Count; i++)
				{
					Sources[i].Source.AddToCommandBuffer(buffer);
				}
			}
		}
	}
}
