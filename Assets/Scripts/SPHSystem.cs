using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;

[UpdateBefore(typeof(TransformSystemGroup))]
public class SPHSystem : JobComponentSystem
{
    private EntityQuery SPHCharacterGroup;
    private EntityQuery SPHColliderGroup;

    private JobHandle collidersToNativeArrayJobHandle;
    private NativeArray<SPHCollider> colliders;

    private Transform cameraTransform;

    private List<SPHParticle> uniqueTypes = new List<SPHParticle>(10);
    private List<PreviousParticle> previousParticles = new List<PreviousParticle>();

    private static readonly int[] cellOffsetTable =
    {
        1, 1, 1, 1, 1, 0, 1, 1, -1, 1, 0, 1, 1, 0, 0, 1, 0, -1, 1, -1, 1, 1, -1, 0, 1, -1, -1,
        0, 1, 1, 0, 1, 0, 0, 1, -1, 0, 0, 1, 0, 0, 0, 0, 0, -1, 0, -1, 1, 0, -1, 0, 0, -1, -1,
        -1, 1, 1, -1, 1, 0, -1, 1, -1, -1, 0, 1, -1, 0, 0, -1, 0, -1, -1, -1, 1, -1, -1, 0, -1, -1, -1
    };


    protected override void OnCreate()
    {
        SPHCharacterGroup = GetEntityQuery(
            ComponentType.ReadOnly(typeof(SPHParticle)), 
            typeof(Translation), 
            typeof(SPHVelocity));

        SPHColliderGroup = GetEntityQuery(ComponentType.ReadOnly(typeof(SPHColllider)));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (cameraTransform == null)
            cameraTransform = GameObject.Find("Main Camera").transform;

        EntityManager.GetAllUniqueSharedComponentData(uniqueTypes);

        for(int typeIndex = 1; typeIndex < uniqueTypes.Count; typeIndex++)
        {

        }
    }


}
