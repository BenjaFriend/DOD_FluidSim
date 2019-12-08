using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[System.Serializable]
public struct Position : IComponentData
{
    public float3 value;
}

public class PositionComponent : ComponentDataProxy<Position> { }

