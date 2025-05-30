using UnityEngine;

namespace Props
{
    [CreateAssetMenu(fileName = "ObjectData", menuName = "Data/ObjectData")]
    public class PropData : ScriptableObject
    {
        [field:SerializeField] private string propName;
        [field:SerializeField] private int id;
        [field:SerializeField] private Color baseColor;
        [field:SerializeField] private Color dropZoneColor;
        [field:SerializeField] private GameObject prefab;
        public string PropName => propName;
        public int ID => id;
        public Color BaseColor => baseColor;
        public Color DropZoneColor => dropZoneColor;
        public GameObject Prefab => prefab;
    }
}
