using Photon.Pun;
using UnityEngine;

namespace Props
{
    public abstract class PropBehaviourBase : MonoBehaviourPun
    {
        [SerializeField] protected PropData propData;
        [SerializeField] protected Renderer rend;

        public PropData PropData => propData;
        public int PropID => propData.ID;

        protected virtual void Start()
        {
            if (rend == null)
                rend = GetComponent<Renderer>();
        }

        public virtual void SetPropData(PropData data)
        {
            propData = data;
            ApplyColor();
        }

        protected virtual void ApplyColor()
        {
            if (rend != null && propData != null)
                rend.material.color = GetAssignedColor();
        }

        protected abstract Color GetAssignedColor();
    }
}