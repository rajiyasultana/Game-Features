using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling Instance;
    public GameObject projectilePrefeb;
    public int poolSize = 20;
    private Queue<GameObject> projectiles = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        for(int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefeb);
            obj.SetActive(false);
            projectiles.Enqueue(obj);
        }

    }

    public GameObject GetProjectile()
    {
        if (projectiles.Count > 0)
        {
            GameObject obj = projectiles.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return null;
    }

    public void ReturnProjectile(GameObject obj)
    {
        obj.SetActive(false);
        projectiles.Enqueue(obj);
    }

}
