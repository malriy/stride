using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipGenerator : MonoBehaviour
{
    public ObjectPooling chipPool;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnChip(Vector3 startPos)
    {
        GameObject chip = chipPool.GetPooledObject();
        chip.transform.position = startPos;
        chip.SetActive(true);
    }
}
