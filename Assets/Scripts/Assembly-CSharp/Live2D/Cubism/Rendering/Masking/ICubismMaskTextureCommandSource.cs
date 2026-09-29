namespace Live2D.Cubism.Rendering.Masking
{
	public interface ICubismMaskTextureCommandSource : ICubismMaskCommandSource
	{
		int GetNecessaryTileCount();

		void SetTiles(CubismMaskTile[] value);
	}
}
