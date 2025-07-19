using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Props;
using ScriptableObjects;
using UnityEngine;

public class PropAssignmentManager : MonoBehaviourPun
{
    [Header("Config")]
    [SerializeField] private PropsDatabase propsDatabase;
    
    private List<DropZone> dropZones;
    private List<PickupObject> pickupObjects;
    private int propsToAssign = 0;
    
    private void Start()
    {
        dropZones = new List<DropZone>(GameManager.Instance.DropZones);
        pickupObjects = new List<PickupObject>(GameManager.Instance.PickupObjects);
        
        if (PhotonNetwork.IsMasterClient)
        {
            propsToAssign = GameManager.Instance.PropsToWin;
            StartCoroutine(DelayedAssignProps());
        }
    }
    
    private IEnumerator DelayedAssignProps()
    {
        yield return new WaitForSeconds(0.2f);
        Debug.Log("[PropAssignmentManager] Assigning props...");
        AssignDataToGameObjects();
    }
    
    private void AssignDataToGameObjects()
    {
        var availableProps = new List<PropData>(propsDatabase.GetAll());
        int maxAssignable = Mathf.Min(availableProps.Count, dropZones.Count, pickupObjects.Count);
        int count = Mathf.Clamp(propsToAssign, 1, maxAssignable);

        List<PropData> selected = GetUniqueRandomProps(availableProps, count);
        int[] dropPropIDs = GetIDsFromProps(selected);
        
        int[] chosenDropZonesIndices = GetRandomIndices(dropZones.Count, count);

        List<PropData> shuffledForPickup = new(selected);
        Shuffle(shuffledForPickup);
        int[] pickupPropIDs = GetIDsFromProps(shuffledForPickup);
        
        photonView.RPC(nameof(RPC_ApplyPropAssignment), RpcTarget.AllBuffered, chosenDropZonesIndices, dropPropIDs, pickupPropIDs);
    }
    
    [PunRPC]
    private void RPC_ApplyPropAssignment(int[] chosenDropZonesIndices, int[] dropIDs, int[] pickupIDs)
    {
        var selectedDropIndices = new HashSet<int>(chosenDropZonesIndices);
        for (int i = 0; i < dropZones.Count; i++)
        {
            if (!selectedDropIndices.Contains(i))
                dropZones[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < dropIDs.Length; i++)
        {
            var data = propsDatabase.GetByID(dropIDs[i]);
            if (data != null)
                dropZones[chosenDropZonesIndices[i]].SetPropData(data);
        }

        for (int i = 0; i < pickupIDs.Length; i++)
        {
            var data = propsDatabase.GetByID(pickupIDs[i]);
            if (data != null)
                pickupObjects[i].SetPropData(data);
        }
    }
    
    // ──────────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────────
    
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
    
    private List<PropData> GetUniqueRandomProps(List<PropData> available, int count)
    {
        List<PropData> result = new();
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, available.Count);
            result.Add(available[index]);
            available.RemoveAt(index);
        }
        return result;
    }
    
    private int[] GetIDsFromProps(List<PropData> props)
    {
        int[] ids = new int[props.Count];
        for (int i = 0; i < props.Count; i++)
            ids[i] = props[i].ID;
        return ids;
    }
    
    private int[] GetRandomIndices(int max, int count)
    {
        List<int> indices = new();
        for (int i = 0; i < max; i++) indices.Add(i);
        Shuffle(indices);
        return indices.GetRange(0, count).ToArray();
    }
}
