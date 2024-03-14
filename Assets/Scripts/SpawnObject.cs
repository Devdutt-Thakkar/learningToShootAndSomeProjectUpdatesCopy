using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    [SerializeField]
    private Vector3 center, size;
    [SerializeField]
    private GameObject refPos;
    [SerializeField]
    private GameObject spawnPrefab;

    [SerializeField]
    private float heightSpawnAdjust;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            SpawnObstacles();
        }
    }

    public void SpawnObstacles()
    {
        Vector3 pos = new Vector3(refPos.transform.position.x-center.x, center.y, center.z) + new Vector3(Random.Range(-size.x / 2, size.x / 2), heightSpawnAdjust, Random.Range(-size.z / 2, size.z / 2));
        Instantiate(spawnPrefab, pos, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(new Vector3(refPos.transform.position.x-center.x, center.y, center.z), size);
    }
}
