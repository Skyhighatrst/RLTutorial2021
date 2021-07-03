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

        var actorArchetype = entityManager.CreateArchetype(
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
        
        

        ConfigureCharacter(actorArchetype, 0, 0, 1, true);
        ConfigureCharacter(actorArchetype, 2, 2, 1.5f, true);
        
        ConfigureCharacter(actorArchetype, 1, 1, 2);
        ConfigureCharacter(actorArchetype, 2, 1, 1);
        ConfigureCharacter(actorArchetype, 3, 1, .5f);
        ConfigureCharacter(actorArchetype, 4, 1, 5);
        ConfigureCharacter(actorArchetype, 5, 1, 1);
    }

    private Entity ConfigureCharacter(EntityArchetype playerArchetype, int x, int y, float speed, bool isPlayerControlled = false)
    {
        var entity = entityManager.CreateEntity(playerArchetype);

        entityManager.AddComponentData(entity, new Translation {Value = new float3(x, 0f, y)});
        entityManager.AddComponentData(entity, new NonUniformScale {Value = new float3(1f, 0.1f, 1f)});
        entityManager.AddComponentData(entity, new Rotation {Value = quaternion.EulerXYZ(new float3(0f, 0f, 0f))});
        entityManager.AddComponentData(entity, new Energy {Value = 0});
        entityManager.AddComponentData(entity, new Speed {Value = speed});

        entityManager.AddSharedComponentData(entity, new RenderMesh
        {
            mesh = mesh,
            material = material,
        });

        if (isPlayerControlled)
        {
            entityManager.AddComponent<PlayerTag>(entity);
            entityManager.AddComponentData(entity, new MaterialColor {Value = new float4(1, 0, 0, 1f)});
        }

        return entity;
    }
}
