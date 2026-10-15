#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Hackathon.EditorTools
{
    [InitializeOnLoad]
    public static class ScreenCaptureHelper
    {
        const string TriggerFile = "capture_trigger.txt";

        static ScreenCaptureHelper()
        {
            EditorApplication.delayCall += CheckTrigger;
        }

        static void CheckTrigger()
        {
            if (File.Exists(TriggerFile))
            {
                try
                {
                    File.Delete(TriggerFile);
                }
                catch { }

                CaptureAllScreens();
            }
        }

        [MenuItem("Tools/Capture Reference Screenshots")]
        public static void CaptureAllScreens()
        {
            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "ReferenceScreenshots");
            Directory.CreateDirectory(outputDir);

            string[] scenes = new string[]
            {
                "Assets/_Game/_Scenes/_Menu.unity",
                "Assets/_Game/_Scenes/_Tutorials.unity",
                "Assets/_Game/_Scenes/_Game.unity",
                "Assets/_Game/_Scenes/_Credits.unity"
            };

            string currentScene = EditorSceneManager.GetActiveScene().path;

            foreach (string scenePath in scenes)
            {
                if (!File.Exists(scenePath)) continue;

                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);

                Camera cam = Camera.main;
                if (cam == null) cam = Object.FindAnyObjectByType<Camera>();

                if (cam != null)
                {
                    Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
                    var originalModes = new RenderMode[canvases.Length];
                    var originalCameras = new Camera[canvases.Length];
                    for (int i = 0; i < canvases.Length; i++)
                    {
                        originalModes[i] = canvases[i].renderMode;
                        originalCameras[i] = canvases[i].worldCamera;
                        if (canvases[i].renderMode == RenderMode.ScreenSpaceOverlay)
                        {
                            canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
                            canvases[i].worldCamera = cam;
                        }
                    }

                    int width = 1920;
                    int height = 1080;
                    RenderTexture rt = new RenderTexture(width, height, 24);
                    cam.targetTexture = rt;
                    cam.Render();

                    RenderTexture.active = rt;
                    Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
                    tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                    tex.Apply();

                    cam.targetTexture = null;
                    RenderTexture.active = null;
                    Object.DestroyImmediate(rt);

                    for (int i = 0; i < canvases.Length; i++)
                    {
                        if (canvases[i] != null)
                        {
                            canvases[i].renderMode = originalModes[i];
                            canvases[i].worldCamera = originalCameras[i];
                        }
                    }

                    string filePath = Path.Combine(outputDir, $"{sceneName}.png");
                    File.WriteAllBytes(filePath, tex.EncodeToPNG());
                    Object.DestroyImmediate(tex);
                    Debug.Log($"[ScreenCaptureHelper] Captured: {filePath}");
                }
            }

            if (!string.IsNullOrEmpty(currentScene) && File.Exists(currentScene))
            {
                EditorSceneManager.OpenScene(currentScene, OpenSceneMode.Single);
            }

            Debug.Log("[ScreenCaptureHelper] All reference screenshots captured successfully!");
        }
    }
}
#endif
