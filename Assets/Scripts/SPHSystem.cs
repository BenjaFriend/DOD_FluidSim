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

    private struct PreviousParticle
    {
#pragma warning disable 0649
        public NativeMultiHashMap<int, int> hashMap;
        public NativeArray<Translation> particlesPosition;
        public NativeArray<SPHVelocity> particlesVelocity;
        public NativeArray<float3> particlesForces;
        public NativeArray<float> particlesPressure;
        public NativeArray<float> particlesDensity;
        public NativeArray<int> particleIndices;

        public NativeArray<int> cellOffsetTable;
#pragma warning restore 0649
    }

    [BurstCompile]
    private struct HashPositions : IJobParallelFor
    {
#pragma warning disable 0649
        [ReadOnly] public float cellRadius;

        public NativeArray<Translation> positions;
        public NativeMultiHashMap<int, int>.ParallelWriter hashMap;
#pragma warning restore 0649

        public void Execute(int index)
        {
            float3 position = positions[index].Value;

            int hash = GridHash.Hash(position, cellRadius);
            hashMap.Add(hash, index);

            positions[index] = new Translation { Value = position };
        }
    }



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
            //Get the current chucnk setting
            SPHParticle settings = uniqueTypes[typeIndex];
            SPHCharacterGroup.SetFilter(settings);

            //cache the data 
            collidersToNativeArrayJobHandle particlePositionJobHandle;
            ComponentDataArray<Translation> particlePosition = SPHCharacterGroup.ToComponentDataArray<Translation>(Allocator.TempJob, out particlePositionJobHandle);
            collidersToNativeArrayJobHandle particleVelocityJobHandle;
            ComponentDataArray<SPHVelocity> particleVelocity = SPHCharacterGroup.ToComponentDataArray<SPHVelocity>(Allocator.TempJob, out particleVelocityJobHandle);

            int cacheIndex = typeIndex - 1;
            int particleCount = particlePosition.Length;

            NativeMultiHashMap<int, int> hashMap = new NativeMultiHashMap<int, int>(particleCount, Allocator.TempJob);

            NativeArray<float3> particlesForces = new NativeArray<float3>(particleCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<float> particlesPressure = new NativeArray<float>(particleCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<float> particlesDensity = new NativeArray<float>(particleCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<int> particleIndices = new NativeArray<int>(particleCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            NativeArray<int> cellsOffsetTableNative = new NativeArray<int>(cellOffsetTable, Allocator.TempJob);

            //Add new or dispose previous particle chunks
            PreviousParticles nextParticles = new PreviousParticles
            {

            }
        }
    }


}
