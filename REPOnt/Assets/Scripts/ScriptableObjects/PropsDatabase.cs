using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Props;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "PropsDatabase", menuName = "Data/PropsDatabase")]
    public class PropsDatabase : ScriptableObject
    {
        [Header("Editor Config")]
        [SerializeField] private List<PropData> rawList;

        [SerializeField]
        private SerializedDictionary<int, PropData> propsById;

        public PropData GetByID(int id)
        {
            return propsById.GetValueOrDefault(id);
        }

        public IReadOnlyCollection<PropData> GetAll()
        {
            return propsById.Values;
        }
        
#if UNITY_EDITOR
        [ContextMenu("Rebuild Dictionary from List")]
        public void RebuildDictionaryFromList()
        {
            propsById.Clear();
            foreach (var data in rawList)
            {
                if (data == null) continue;
                if (propsById.TryAdd(data.ID, data)) continue;
                Debug.LogWarning($"[PropsDatabase] Duplicate ID {data.ID} found. Skipping.");
            }

            Debug.Log("[PropsDatabase] Dictionary rebuilt from list successfully.");
        }
#endif
    }
}
