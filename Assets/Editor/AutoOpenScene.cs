using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class AutoPlaySceneSwitcher
{
    static string playScenePath = "Assets/Scenes/MainMenuScene.unity"; // Change to your play scene path
    const string previousSceneKey = "PreviousScenePath_ForAutoSwitch";

    static AutoPlaySceneSwitcher()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorApplication.update += OnEditorUpdate;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            string currentScene = EditorSceneManager.GetActiveScene().path;
            if (currentScene != playScenePath)
            {
                EditorPrefs.SetString(previousSceneKey, currentScene);

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(playScenePath);
                    EditorApplication.isPlaying = true; // Enter Play mode manually
                }
                else
                {
                    EditorApplication.isPlaying = false;
                }
            }
        }
    }

    static void OnEditorUpdate()
    {
        if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (EditorPrefs.HasKey(previousSceneKey))
            {
                string prevScene = EditorPrefs.GetString(previousSceneKey);
                if (!string.IsNullOrEmpty(prevScene) && prevScene != playScenePath)
                {
                    EditorPrefs.DeleteKey(previousSceneKey);
                    EditorSceneManager.OpenScene(prevScene);
                }
            }
        }
    }
}
