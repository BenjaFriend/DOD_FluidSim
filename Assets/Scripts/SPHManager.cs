using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using TMPro;

public class SPHManager : MonoBehaviour
{
    // Import
    [Header("Import")]
    [SerializeField] private GameObject SPHParticlePrefab = null;
    [SerializeField] private GameObject SpawnableParticlePrefab = null;
    [SerializeField] private GameObject SPHColliderPrefab = null;

    private EntityManager manager;

    // Properties
    [Header("Properties")]
    [SerializeField] private int InitalSpawnAmount = 5000;

    [Header("UI")]
    public TextMeshProUGUI EntityCountLabel = null;
    public TMP_InputField SpawningInputField = null;


    private GameObject[] colliders;

    private int m_EntityCount = 0;

    private void Awake()
    {
        // Find all colliders
        colliders = GameObject.FindGameObjectsWithTag("SPHCollider");

        // Ensure that we have set up the manager properly
        Debug.Assert(SPHParticlePrefab != null);
        Debug.Assert(SPHColliderPrefab != null);
        Debug.Assert(SpawnableParticlePrefab != null);
    }

    private void Start()
    {
        manager = World.Active.EntityManager;
        
        // Setup
        AddColliders();
        AddParticles(InitalSpawnAmount);
    }

    public void AddParticlesFromUI()
    {
        if(SpawningInputField != null)
        {
            int amount = 0;

            bool iRes = int.TryParse(SpawningInputField.text, out amount);
            if(iRes)
            {
                AddParticles(amount);
            }
            else
            {
                Debug.LogError("Invalid spawn format. Must be an int");
            }
        }
    }

    private void AddParticles(int _amount)
    {
        NativeArray<Entity> entities = new NativeArray<Entity>(_amount, Allocator.Temp);
        manager.Instantiate(SPHParticlePrefab, entities);

        for (int i = 0; i < _amount; i++)
        {
            manager.SetComponentData(
                entities[i], 
                new Translation 
                {
                    Value = new float3(
                        i % 16 + UnityEngine.Random.Range(-0.1f, 0.1f),
                        2 + (i / 16 / 16) * 1.1f, 
                        (i / 16) % 16) + UnityEngine.Random.Range(-0.1f, 0.1f)
                }
            );
        }

        entities.Dispose();
        IncreateEntityCount(_amount);
    }

    private void AddColliders()
    {
        // Turn them into entities
        NativeArray<Entity> entities = new NativeArray<Entity>(colliders.Length, Allocator.Temp);
        manager.Instantiate(SPHColliderPrefab, entities);

        // Set data
        for (int i = 0; i < colliders.Length; i++)
        {
            manager.SetComponentData(entities[i], new SPHCollider
            {
                position = colliders[i].transform.position,
                right = colliders[i].transform.right,
                up = colliders[i].transform.up,
                scale = new float2(colliders[i].transform.localScale.x / 2f, colliders[i].transform.localScale.y / 2f)
            });
        }

        // Done
        entities.Dispose();
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                SpawnSphereEntity(Camera.main.transform.position, hit.point);
            }        
        }
    }

    private void SpawnSphereEntity(float3 CamPos, float3 TargetPos)
    {
        var instance = manager.Instantiate(SpawnableParticlePrefab);
        manager.SetComponentData(
            instance,
            new Translation
            {
                Value = TargetPos
            }
        );

        IncreateEntityCount(1);
    }

    private void IncreateEntityCount(int amount)
    {
        m_EntityCount += amount;
        if (EntityCountLabel != null)
        {
            EntityCountLabel.text = m_EntityCount.ToString();
        }
    }
}
