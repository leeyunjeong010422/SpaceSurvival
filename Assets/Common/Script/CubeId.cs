using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeId : MonoBehaviour
{
    [SerializeField] GameObject[] cubes;

    private Dictionary<GameObject, int> idDictionary;

    private void Start()
    {
        idDictionary = new Dictionary<GameObject, int>(cubes.Length << 1);
        for (int i = 0; i < cubes.Length; i++)
        {
            idDictionary.Add(cubes[i], i);
        }
    }

    public GameObject GetCube(int id)
    {
        if (id < 0 || id >= cubes.Length)
        {
            Debug.LogError("잘못된 id");
            return null;
        }
        return cubes[id];
    }

    public int GetId(GameObject cube)
    {
        if (idDictionary.TryGetValue(cube, out int value))
        {
            return value;
        }
        else
        {
            Debug.LogError("목록에 등록되지 않은 큐브");
            return -1;
        }
    }
}
