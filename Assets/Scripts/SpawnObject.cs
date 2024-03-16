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
    private GameObject[] spawnPrefab;
    [SerializeField]
    private int startWait;
    //private Vector3 spawnValues;
    [SerializeField]
    private float spawnLeastWait;
    [SerializeField]
    private float spawnMostWait;
    [SerializeField]
    private bool stop;
    private int randEnemy;
    private int numberOfEnemies;
    [SerializeField]
    private float spawnWeight;

    [SerializeField]
    private float heightSpawnAdjust;
    // Start is called before the first frame update
    void Start()
    {
        numberOfEnemies = spawnPrefab.Length;
        Random.InitState((int)Time.captureDeltaTime);

        StartCoroutine(waitSpawner());
    }

    IEnumerator waitSpawner()
    {
        yield return new WaitForSeconds(startWait);

        while (!stop)
        {
            randEnemy = Random.Range(0, numberOfEnemies-1);

            SpawnObstacles();

            yield return new WaitForSeconds(spawnWeight);
        }
    }

    // Update is called once per frame
    void Update()
    {
        spawnWeight = Random.Range(spawnLeastWait, spawnMostWait);
    }

    public void SpawnObstacles()
    {
        Vector3 pos = new Vector3(refPos.transform.position.x-center.x, center.y, center.z) + new Vector3(Random.Range(-size.x / 2, size.x / 2), heightSpawnAdjust, Random.Range(-size.z / 2, size.z / 2));
        Instantiate(spawnPrefab[randEnemy], pos, Quaternion.Euler(0,Random.Range(0,359),0));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(new Vector3(refPos.transform.position.x-center.x, center.y, center.z), size);
    }
}
