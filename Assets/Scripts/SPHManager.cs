using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;


public class SPHManager : MonoBehaviour
{
    // Import
    [Header("Import")]
    [SerializeField] private GameObject sphParticlePrefab = null;
    [SerializeField] private GameObject sphColliderPrefab = null;
    private EntityManager manager;

    // Properties
    [Header("Properties")]
    [SerializeField] private int amount = 5000;

    // Start is called before the first frame update
    void Start()
    {
        manager = World.Active.EntityManager;

        AddColliders();
        AddParticles(amount);
    }

    void AddColliders()
    {
        GameObject[] colliders = GameObject.FindGameObjectsWithTag("SPHCollider");

        //Convert to entities
        NativeArray<Entity> entities = new NativeArray<Entity>(colliders.Length, Allocator.Temp);
        manager.Instantiate(sphColliderPrefab, entities);

        for(int i = 0; i < colliders.Length; i++)
        {
            manager.SetComponentData(entities[i], new SPHCollider
            {
                position = colliders[i].transform.position,
                right = colliders[i].transform.right,
                up = colliders[i].transform.up,
                scale = new float2(colliders[i].transform.localScale.x / 2f, colliders[i].transform.localScale.y /2f)
            });
        }

        entities.Dispose();
    }

    void AddParticles(int t_Amount)
    {
        NativeArray<Entity> entities = new NativeArray<Entity>(t_Amount, Allocator.Temp);
        manager.Instantiate(sphParticlePrefab, entities);

        for(int i = 0; i < t_Amount; i++)
        {
            manager.SetComponentData(
                entities[i], 
                new Position
                { 
                    value = new float3(i % 16 + UnityEngine.Random.Range(-0.1f, 0.1f), 
                    2 + (i / 16 / 16) * 1.1f, 
                    (i / 16) % 16) + UnityEngine.Random.Range(-0.1f, 0.1f) 
                });
        }

        entities.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
