using Photon.Pun;
using UnityEngine;

namespace Props
{
    public abstract class PropBehaviourBase : MonoBehaviourPun
    {
        [SerializeField] protected PropData propData;
        [SerializeField] protected Renderer rend;
        //[SerializeField] protected Transform bodyTransform;

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
            InstantiateProp();
        }

        protected virtual void ApplyColor()
        {
            if (rend != null && propData != null)
                rend.material.color = GetAssignedColor();
        }

        protected virtual void InstantiateProp()
        {
            GameObject newProp = Instantiate(propData.Prefab, transform.position, transform.rotation);
            newProp.transform.SetParent(transform);
            Collider childCollider = newProp.GetComponentInChildren<Collider>();
            if (childCollider != null)
            {
                System.Type colliderType = childCollider.GetType();
                Collider copied = gameObject.AddComponent(colliderType) as Collider;
                childCollider.enabled = false;
                if (copied is BoxCollider box && childCollider is BoxCollider originalBox)
                {
                    box.center = originalBox.center;
                    box.size = originalBox.size;
                }
            }
        }

        protected abstract Color GetAssignedColor();
    }
}