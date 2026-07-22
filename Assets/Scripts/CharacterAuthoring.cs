using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Burst;

public struct CharacterMoveDirection : IComponentData
{
    public float2 Value;
}

public struct CharacterMoveSpeed : IComponentData
{
    public float Value;
}

public struct InitializeCharacterFlag :IComponentData, IEnableableComponent {}

public class CharacterAuthoring : MonoBehaviour
{
    public float MoveSpeed;

    private class Baker : Baker<CharacterAuthoring>
    {
        public override void Bake(CharacterAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<InitializeCharacterFlag>(entity);
            AddComponent<CharacterMoveDirection>(entity);
            AddComponent(entity, new CharacterMoveSpeed 
            { 
                Value = authoring.MoveSpeed
            });
        }
    }
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct CharacterInitializeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (mass, shouldInitialize) in SystemAPI.Query<RefRW<PhysicsMass>, EnabledRefRW<InitializeCharacterFlag>>())
        {
            mass.ValueRW.InverseInertia = float3.zero;
            shouldInitialize.ValueRW = false;
        }
    }
}

public partial struct CharacterMoveSystem: ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (direction, speed, velocity) in SystemAPI.Query<CharacterMoveDirection, CharacterMoveSpeed, RefRW<PhysicsVelocity>>())
        {
            var moveStep2d = direction.Value * speed.Value;
            velocity.ValueRW.Linear = new float3(moveStep2d, 0f);
        }
    }
}
