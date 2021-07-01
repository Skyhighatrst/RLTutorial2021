using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    public Mesh mesh;
    public Material material;

    private EntityManager entityManager;
    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var playerArchetype = entityManager.CreateArchetype(
            typeof(PlayerTag),
            typeof(AwaitingTurn),
            typeof(Energy),
            typeof(Speed),
            typeof(Translation),
            typeof(NonUniformScale),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld)
        );

        ConfigurePlayerEntity(playerArchetype, 0, 0, 1);
        ConfigurePlayerEntity(playerArchetype, 1, 1, 2);
    }

    private void ConfigurePlayerEntity(EntityArchetype playerArchetype, int x, int y, float speed)
    {
        var playerEntity = entityManager.CreateEntity(playerArchetype);

        entityManager.AddComponentData(playerEntity, new Translation {Value = new float3(x, 0f, y)});
        entityManager.AddComponentData(playerEntity, new NonUniformScale {Value = new float3(1f, 0.1f, 1f)});
        entityManager.AddComponentData(playerEntity, new Rotation {Value = quaternion.EulerXYZ(new float3(0f, 0f, 0f))});
        entityManager.AddComponentData(playerEntity, new Energy {Value = 0});
        entityManager.AddComponentData(playerEntity, new Speed {Value = speed});

        entityManager.AddSharedComponentData(playerEntity, new RenderMesh
        {
            mesh = mesh,
            material = material,
        });
    }
}
