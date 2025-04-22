using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Props;
using ScriptableObjects;
using UnityEngine;

public class PropAssignmentManager : MonoBehaviourPun
{
    [Header("Config")]
    [SerializeField] private int propsToAssign = 3;
    [SerializeField] private PropsDatabase propsDatabase;
    [SerializeField] private List<DropZone> dropZones;
    [SerializeField] private List<PickupObject> pickupObjects;
    
    private void Start()
    {
        dropZones = new List<DropZone>(GameManager.Instance.DropZones);
        pickupObjects = new List<PickupObject>(GameManager.Instance.PickupObjects);
        
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(DelayedAssignProps());
        }
    }
    
    private IEnumerator DelayedAssignProps()
    {
        yield return new WaitForSeconds(0.2f);
        Debug.Log("[PropAssignmentManager] Assigning props...");
        AssignRandomPropsAsMaster();
    }
    
    private void AssignRandomPropsAsMaster()
    {
        
        var allProps = new List<PropData>(propsDatabase.GetAll());
        int available = Mathf.Min(allProps.Count, dropZones.Count, pickupObjects.Count);
        int count = Mathf.Clamp(propsToAssign, 1, available);

        List<PropData> selected = new();
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, allProps.Count);
            selected.Add(allProps[index]);
            allProps.RemoveAt(index);
        }
        
        List<int> dropzonesIndex = new();
        for (int i = 0; i < dropZones.Count; i++) dropzonesIndex.Add(i);
        Shuffle(dropzonesIndex);
        int[] chosenDropZones = dropzonesIndex.GetRange(0, count).ToArray();

        

        int[] dropIDs = new int[count];
        for (int i = 0; i < count; i++)
        {
            dropIDs[i] = selected[i].ID;
        }
        
        List<PropData> shuffled = new(selected);
        Shuffle(shuffled);
        
        int[] pickupIDs = new int[count];
        for (int i = 0; i < count; i++)
        {
            pickupIDs[i] = shuffled[i].ID;
        }
        // RPC_ApplyPropAssignment(dropIDs,pickupIDs);
        photonView.RPC(nameof(RPC_ApplyPropAssignment), RpcTarget.AllBuffered, chosenDropZones, dropIDs, pickupIDs);
    }
    
    [PunRPC]
    private void RPC_ApplyPropAssignment(int[] chosenDropzones, int[] dropIDs, int[] pickupIDs)
    {
        HashSet<int> chosenSet = new(chosenDropzones);

        for (int i = 0; i < dropZones.Count; i++)
        {
            if (!chosenSet.Contains(i))
            {
                dropZones[i].gameObject.SetActive(false);
            }
        }
        
        for (int i = 0; i < dropIDs.Length; i++)
        {
            var data = propsDatabase.GetByID(dropIDs[i]);
            if (data != null)
                dropZones[chosenDropzones[i]].SetPropData(data);
        }
        
        for (int i = 0; i < pickupIDs.Length; i++)
        {
            var data = propsDatabase.GetByID(pickupIDs[i]);
            if (data != null)
                pickupObjects[i].SetPropData(data);
        }
        
        Debug.Log("[PropAssignmentManager] Props assigned successfully.");
    }
    
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
