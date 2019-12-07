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
        manager = World.Active.GetOrCreateManager<EntityManager>();

        AddColliders();
        AddParticles(amount);
    }

    void AddColliders()
    {

    }

    void AddParticles(int t_Amount)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
