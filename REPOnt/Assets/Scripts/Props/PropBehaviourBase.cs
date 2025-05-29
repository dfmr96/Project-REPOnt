using Photon.Pun;
using UnityEngine;

namespace Props
{
    public abstract class PropBehaviourBase : MonoBehaviourPun
    {
        [SerializeField] protected PropData propData;
        [SerializeField] protected Renderer rend;
        [SerializeField] protected MeshFilter _meshFilter;

        public PropData PropData => propData;
        public int PropID => propData.ID;

        protected virtual void Start()
        {
            if (rend == null)
                rend = GetComponent<Renderer>();
            _meshFilter = GetComponent<MeshFilter>();
        }

        public virtual void SetPropData(PropData data)
        {
            propData = data;
            ApplyColor();
            ApplyMesh();
        }

        protected virtual void ApplyColor()
        {
            if (rend != null && propData != null)
                rend.material.color = GetAssignedColor();
        }
        
        protected virtual void ApplyMesh()
        {
            _meshFilter.mesh = propData.PropMesh;
        }

        protected abstract Color GetAssignedColor();
    }
}