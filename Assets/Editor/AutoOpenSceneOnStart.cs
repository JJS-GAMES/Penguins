using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public static class AutoOpenSceneOnStart
{
    private const string FlagFilePath = "Assets/unityLoaded.txt~";
    private const string ScenePathToOpen = "Assets/PenguinEgg/Scenes/Menu.unity";
    static AutoOpenSceneOnStart()
    {
        EditorApplication.update += RunOnceOnLoad;
    }

    private static void RunOnceOnLoad() 
    {
        EditorApplication.update -= RunOnceOnLoad;

        if (!File.Exists(FlagFilePath))
        {
            File.WriteAllText(FlagFilePath, "true");
            EditorSceneManager.OpenScene(ScenePathToOpen, OpenSceneMode.Single);
        }
    }
}
