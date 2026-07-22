using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerTag: IComponentData {}

public struct CameraTarget : IComponentData
{
    public UnityObjectRef<Transform> CameraTransform;
}

public class PlayerAuthoring : MonoBehaviour
{
    private class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerTag>(entity);
        }
    }
}

public partial class PlayerInputSystem : SystemBase
{
    private SurvivorInput _input;

    protected override void OnCreate()
    {
        _input = new SurvivorInput();
        _input.Enable();
    }

    protected override void OnUpdate()
    {
        var _curInput = (float2) _input.Player.Move.ReadValue<Vector2>();

        foreach (var dir in SystemAPI.Query<RefRW<CharacterMoveDirection>>().WithAll<PlayerTag>())
        {
            dir.ValueRW.Value = _curInput;
        }
    }
}
