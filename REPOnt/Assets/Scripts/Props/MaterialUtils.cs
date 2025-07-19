using System.Collections.Generic;
using UnityEngine;

namespace Props
{
    public static class MaterialUtils
    {
        private static Dictionary<GameObject, Material[]> originalMaterials = new();

        public static void SetGrayAlpha(GameObject obj, float newAlpha = 1f)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null) return;

            if (!originalMaterials.ContainsKey(obj))
            {
                Material[] originals = renderer.materials;
                originalMaterials[obj] = originals;
            }

            Material[] currentMaterials = renderer.materials;
            Material[] modifiedMaterials = new Material[currentMaterials.Length];

            for (int i = 0; i < currentMaterials.Length; i++)
            {
                Material mat = new Material(currentMaterials[i]);

                mat.SetFloat("_Mode", 3);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                mat.color = new Color(0.5f, 0.5f, 0.5f, newAlpha);
                modifiedMaterials[i] = mat;
            }

            renderer.materials = modifiedMaterials;
        }

        public static void RestoreOriginalMaterials(GameObject obj)
        {
            if (!originalMaterials.ContainsKey(obj)) return;

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null) return;

            renderer.materials = originalMaterials[obj];
            originalMaterials.Remove(obj);
        }
    }
}
