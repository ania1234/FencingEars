using UnityEngine;
using Zenject;

public class OverallProjectInstaller : MonoInstaller<OverallProjectInstaller>
{
	public TextureManager textureManagerPrefab;

	public override void InstallBindings ()
	{
		Container.Bind<TextureManager> ().FromComponentInNewPrefab (textureManagerPrefab).AsCached ();
	}
}