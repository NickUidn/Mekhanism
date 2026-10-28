using System.Diagnostics.Contracts;
using UnityEditor;
using UnityEngine;

public class ContractManager : MonoBehaviour
{
    [SerializeField] GameObject contractPrefab;
    public ContractClass contract;
    private void Awake()
    {
        //if (contractPrefab != null)
        //{
        //    contract=Instantiate(contractPrefab,gameObject.transform).GetComponent<ContractClass>();
        //    contract.AddContractSegment(2, 10, 50, 25);
        //}
    }

}
