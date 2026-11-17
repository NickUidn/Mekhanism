using System.Diagnostics.Contracts;
using UnityEditor;
using UnityEngine;

public class ContractManager : MonoBehaviour
{
    [SerializeField] GameObject contractPrefab;
    [SerializeField] GameObject contractSpot1;
    [SerializeField] GameObject contractSpot2;
    [SerializeField] GameObject contractSpot3;
    private GameObject[] spawnedContracts = new GameObject[3];
    public ContractClass contract;

    private void Awake()
    {
        //if (contractPrefab != null)
        //{
        //    contract=Instantiate(contractPrefab,gameObject.transform).GetComponent<ContractClass>();
        //    contract.AddContractSegment(2, 10, 50, 25);
        //}
    }
  RectTransform scaleCheckRef;
    private void Start()
    {
        scaleCheckRef= GameObject.Find("Cycle").GetComponent<RectTransform>();
    }
  
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnContract();
        }
    }
    private void SpawnContract()
    {
        // Find the first free spot
        int slotIndex = -1;
        for (int i = 0; i < spawnedContracts.Length; i++)
        {
            if (spawnedContracts[i] == null)
            {
                slotIndex = i;
                break;
            }
        }

        // All spots taken — ignore the command
        if (slotIndex == -1)
            return;

        RectTransform spotRT = GetSpotRect(slotIndex + 1);
        if (spotRT == null) return;

        GameObject instance = Instantiate(contractPrefab, transform);
        instance.transform.localScale = scaleCheckRef.localScale;

        RectTransform instanceRT = instance.GetComponent<RectTransform>();

        // Center-left point of the spot in its local space
        Vector3 leftEdgeCenterLocal = new Vector3(
            spotRT.rect.xMin,
            spotRT.rect.center.y,
            0f
        );
        float halfWidth = instanceRT.rect.width * 0.5f * instanceRT.lossyScale.x;
        // Convert to world, then assign (works across differing parent scales)
        instanceRT.position = spotRT.TransformPoint(leftEdgeCenterLocal + new Vector3(halfWidth, 0f, 0f));

        spawnedContracts[slotIndex] = instance;
    }
        
    public void ClearSpot(int spotNumber)
    {
        int index = spotNumber - 1;
        if (index < 0 || index >= spawnedContracts.Length)
        {
            Debug.LogWarning($"ClearSpot: invalid spot number {spotNumber} (expected 1-3)");
            return;
        }

        if (spawnedContracts[index] != null)
        {
            Destroy(spawnedContracts[index]);
            spawnedContracts[index] = null;
        }
    }

    private RectTransform GetSpotRect(int spotNumber)
    {
        GameObject spot = spotNumber switch
        {
            1 => contractSpot1,
            2 => contractSpot2,
            3 => contractSpot3,
            _ => null
        };
        return spot != null ? spot.GetComponent<RectTransform>() : null;
    }
}
