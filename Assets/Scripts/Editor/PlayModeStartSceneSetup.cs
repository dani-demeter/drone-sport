using UnityEditor;
using UnityEditor.SceneManagement;

namespace DroneSport.EditorTools
{
    [InitializeOnLoad]
    internal static class PlayModeStartSceneSetup
    {
        private const string MenuScenePath = "Assets/Scenes/Menu.unity";

        static PlayModeStartSceneSetup()
        {
            SceneAsset menuScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(MenuScenePath);
            if (menuScene != null && EditorSceneManager.playModeStartScene != menuScene)
            {
                EditorSceneManager.playModeStartScene = menuScene;
            }
        }
    }
}
