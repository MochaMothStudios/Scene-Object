using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FedoraDev.SceneObject.Implementations
{
	public class InstantLoader : ISceneLoader
	{
		[SerializeField, BoxGroup("Loading Screen")] bool _showLoadingScreen;
		[SerializeField, ShowIf(nameof(_showLoadingScreen)), BoxGroup("Loading Screen")] ISceneObject _loadingScene;

		public IEnumerator LoadAsync(ISceneObject sceneObject)
		{
			AsyncOperation loadingOperation;

			if (_showLoadingScreen)
			{
				loadingOperation = SceneManager.LoadSceneAsync(_loadingScene.ScenePath, LoadSceneMode.Additive);
				while (!loadingOperation.isDone)
					yield return null;
			}

			LoadingBarBehaviour loadingBar = Object.FindObjectOfType<LoadingBarBehaviour>();

			loadingOperation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
			while (!loadingOperation.isDone)
			{
				if (loadingBar != null)
					loadingBar.SetSliderValue(loadingOperation.progress / 2f);
				yield return null;
			}

			loadingOperation = SceneManager.LoadSceneAsync(sceneObject.ScenePath, LoadSceneMode.Additive);
			while (!loadingOperation.isDone)
			{
				if (loadingBar != null)
					loadingBar.SetSliderValue((loadingOperation.progress / 2f) + 0.5f);
				yield return null;
			}

			if (_showLoadingScreen)
			{
				loadingOperation = SceneManager.UnloadSceneAsync(_loadingScene.ScenePath);
				while (!loadingOperation.isDone)
					yield return null;
			}

			_ = SceneManager.SetActiveScene(SceneManager.GetSceneByPath(sceneObject.ScenePath));
		}
	}
}
