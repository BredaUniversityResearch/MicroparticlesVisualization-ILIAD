using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class ParticleDestroySystem : SystemBase
{
	[BurstCompile]
	protected override void OnCreate()
	{
		RequireForUpdate<ParticleSpawnData>();
		RequireForUpdate<ParticleVisualizationSettingsData>();
	}

	[BurstCompile]
	protected override void OnDestroy()
	{ }

	[BurstCompile]
	protected override void OnUpdate()
	{
		Entity settingsEntity = SystemAPI.GetSingletonEntity<ParticleVisualizationSettingsData>();
		if (EntityManager.IsComponentEnabled<DestroyParticlesTag>(settingsEntity))
		{
			EntityManager.SetComponentEnabled<DestroyParticlesTag>(settingsEntity, false);
			EntityManager.DestroyEntity(GetEntityQuery(ComponentType.ReadOnly<ParticleProperties>()));
		}
	}
}

