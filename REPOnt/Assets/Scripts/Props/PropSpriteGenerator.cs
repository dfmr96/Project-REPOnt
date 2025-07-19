using System.IO;
using UnityEditor;
using UnityEngine;

namespace Props
{
    public class PropSpriteGenerator : MonoBehaviour
    {
        public Camera renderCamera;
        public Transform spawnPoint;
        public Vector2Int resolution = new Vector2Int(512, 512);
        public string outputFolder = "Assets/PropIcons";
        public GameObject[] propsToRender;

        #if UNITY_EDITOR
        [ContextMenu("Generate Sprites")]
        public void GenerateSprites()
        {
            RenderTexture rt = new RenderTexture(resolution.x, resolution.y, 24);
            renderCamera.targetTexture = rt;
            renderCamera.clearFlags = CameraClearFlags.SolidColor;
            renderCamera.backgroundColor = new Color(0, 0, 0, 0); // Transparente

            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            foreach (GameObject prefab in propsToRender)
            {
                GameObject instance = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
                instance.transform.localScale = Vector3.one;

                Renderer renderer = instance.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    Vector3 offset = renderer.bounds.center - spawnPoint.position;
                    instance.transform.position -= offset;

                    float maxExtent = Mathf.Max(renderer.bounds.extents.x, renderer.bounds.extents.y, renderer.bounds.extents.z);
                    renderCamera.orthographicSize = maxExtent * 1.5f;
                }

                renderCamera.Render();

                RenderTexture.active = rt;
                Texture2D tex = new Texture2D(resolution.x, resolution.y, TextureFormat.ARGB32, false);
                tex.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
                tex.Apply();

                byte[] png = tex.EncodeToPNG();
                string path = $"{outputFolder}/{prefab.name}.png";
                File.WriteAllBytes(path, png);

                DestroyImmediate(instance);
            }

            renderCamera.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(rt);
            AssetDatabase.Refresh();
        }
        #endif
    }
}
