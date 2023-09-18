using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct ParticleSpawnSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState a_state)
	{
		//a_state.RequireForUpdate(a_state.GetEntityQuery(ComponentType.ReadOnly<ParticleSpawnData>(), ComponentType.ReadOnly<ParticleVisualizationSettingsData>()));
		a_state.RequireForUpdate<ParticleSpawnData>();
		a_state.RequireForUpdate<ParticleVisualizationSettingsData>();
	}

	[BurstCompile]
	public void OnDestroy(ref SystemState a_state)
	{ }

	[BurstCompile]
	public void OnUpdate(ref SystemState a_state)
	{
		a_state.Enabled = false;

		Entity settingsEntity = SystemAPI.GetSingletonEntity<ParticleSpawnData>();
		RefRO<ParticleSpawnData> spawnData = SystemAPI.GetComponentRO<ParticleSpawnData>(settingsEntity);
		RefRO<ParticleVisualizationSettingsData> visualizationData = SystemAPI.GetComponentRO<ParticleVisualizationSettingsData>(settingsEntity);
		var ecb = new EntityCommandBuffer(Allocator.Temp);

		int i = 0;
		while (i < spawnData.ValueRO.m_lats.Length)
		{
			Entity newParticle = ecb.Instantiate(visualizationData.ValueRO.m_particlePrefab);
			LocalTransform newTransform = new LocalTransform
			{
				Position = new float3 { x = spawnData.ValueRO.m_lons[i] - 10.476f, y = spawnData.ValueRO.m_lats[i] - 63.583f, z = spawnData.ValueRO.m_depths[i] },
				Rotation = quaternion.identity,
				Scale = 0.0001f
			};
			ecb.SetComponent(newParticle, newTransform);
			i += spawnData.ValueRO.m_entriesPerParticle;
		}
		ecb.RemoveComponent<ParticleSpawnData>(settingsEntity);
		ecb.Playback(a_state.EntityManager);
	}
}

