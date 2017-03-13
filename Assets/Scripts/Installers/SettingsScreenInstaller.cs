using UnityEngine;
using Zenject;
using UnityEngine.UI;

public class SettingsScreenInstaller : MonoInstaller<SettingsScreenInstaller>
{
	public Text chooseImageText;

	public Image mainTextureImage;

	public RectTransform imageCutRectTransform;

	public ScrollRect mainScrollRect;

	public GameObject UIBlocker;

	public Camera mainCamera;

	public override void InstallBindings ()
	{
		Container.BindInstance<Text> (chooseImageText).WithId ("ChooseImageText").AsCached ();
		Container.BindInstance<Image> (mainTextureImage).WithId ("MainTextureImage").AsCached ();
		Container.BindInstance<RectTransform> (imageCutRectTransform).WithId ("ImageCutRectTransform").AsCached ();
		Container.BindInstance<GameObject> (UIBlocker).WithId ("UIBlocker").AsCached ();
		Container.BindInstance<Camera> (mainCamera).WithId ("MainCamera").AsCached ();
		Container.BindInstance<ScrollRect> (mainScrollRect).WithId ("MainScrollRect").AsCached ();
	}
}