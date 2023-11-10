using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
//using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
//using static UnityEngine.InputManagerEntry;
//using UnityEngine.UIElements;
//using UnityEngine.Rendering.VirtualTexturing;
//using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class ParticleSpawnSystem : SystemBase
{
	[BurstCompile]
	protected override void OnCreate()
	{
		//a_state.RequireForUpdate(a_state.GetEntityQuery(ComponentType.ReadOnly<ParticleSpawnData>(), ComponentType.ReadOnly<ParticleVisualizationSettingsData>()));
		RequireForUpdate<ParticleSpawnData>();
		RequireForUpdate<ParticleVisualizationSettingsData>();
	}

	[BurstCompile]
	protected override void OnDestroy()
	{ }

	[BurstCompile]
	protected override void OnUpdate()
	{
		Enabled = false;

		Entity settingsEntity = SystemAPI.GetSingletonEntity<ParticleVisualizationSettingsData>();
		RefRW<ParticleSpawnData> spawnData = SystemAPI.GetComponentRW<ParticleSpawnData>(settingsEntity);
		RefRO<ParticleVisualizationSettingsData> visualizationData = SystemAPI.GetComponentRO<ParticleVisualizationSettingsData>(settingsEntity);

		var ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
		//var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

		var job = new SpawnParticleJob
		{
			m_entriesPerParticle = spawnData.ValueRO.m_entriesPerParticle,
			m_prefab = visualizationData.ValueRO.m_particlePrefab,
			m_spawnData = spawnData.ValueRO.Value,
			m_ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter()
		};
		//job.Run(spawnData.ValueRO.Value.Value.m_depths.Length / spawnData.ValueRO.m_entriesPerParticle);
		JobHandle jobHandle = job.Schedule(spawnData.ValueRO.Value.Value.m_depths.Length / spawnData.ValueRO.m_entriesPerParticle, 64);
		//jobHandle.Complete();

		EntityManager.SetComponentEnabled<ParticleSpawnData>(settingsEntity, false);
		ecbSystem.AddJobHandleForProducer(jobHandle);
	}

	//public void OnUpdate(ref SystemState a_state)
	//{
	//	a_state.Enabled = false;

	//	Entity settingsEntity = SystemAPI.GetSingletonEntity<ParticleSpawnData>();
	//	RefRO<ParticleSpawnData> spawnData = SystemAPI.GetComponentRO<ParticleSpawnData>(settingsEntity);
	//	RefRO<ParticleVisualizationSettingsData> visualizationData = SystemAPI.GetComponentRO<ParticleVisualizationSettingsData>(settingsEntity);
	//	var ecb = new EntityCommandBuffer(Allocator.Temp);

	//	int i = 0;
	//	while (i < spawnData.ValueRO.m_lats.Length)
	//	{
	//		Entity newParticle = ecb.Instantiate(visualizationData.ValueRO.m_particlePrefab);
	//		LocalTransform newTransform = new LocalTransform
	//		{
	//			Position = float3.zero,
	//			Rotation = quaternion.identity,
	//			Scale = 0.0001f
	//		};
	//		ecb.SetComponent(newParticle, newTransform);

	//		BlobBuilder builder = new BlobBuilder(Allocator.Temp);
	//		ref ParticlePropertiesBlob ppBlob = ref builder.ConstructRoot<ParticlePropertiesBlob>();
	//		var depthArrayBuilder = builder.Allocate(ref ppBlob.m_depths, spawnData.ValueRO.m_entriesPerParticle);
	//		var latArrayBuilder = builder.Allocate(ref ppBlob.m_lats, spawnData.ValueRO.m_entriesPerParticle);
	//		var lonArrayBuilder = builder.Allocate(ref ppBlob.m_lons, spawnData.ValueRO.m_entriesPerParticle);

	//		for (var j = 0; j < spawnData.ValueRO.m_entriesPerParticle; j++)
	//		{
	//			depthArrayBuilder[j] = spawnData.ValueRO.m_depths[i + j];
	//			latArrayBuilder[j] = spawnData.ValueRO.m_lats[i + j];
	//			lonArrayBuilder[j] = spawnData.ValueRO.m_lons[i + j];
	//		}

	//		var blobAsset = builder.CreateBlobAssetReference<ParticlePropertiesBlob>(Allocator.Persistent);
	//		ecb.AddComponent(newParticle, new ParticleProperties
	//		{
	//			Value = blobAsset
	//		});
	//		builder.Dispose();
	//		//Important note: the created blob assets are not currently disposed

	//		i += spawnData.ValueRO.m_entriesPerParticle;
	//	}
	//	ecb.RemoveComponent<ParticleSpawnData>(settingsEntity);
	//	ecb.Playback(a_state.EntityManager);
	//}
}

[BurstCompile]
public partial struct SpawnParticleJob : IJobParallelFor
{
	[ReadOnly] public Entity m_prefab;
	[ReadOnly] public BlobAssetReference<ParticlePropertiesBlob> m_spawnData;
	[ReadOnly] public int m_entriesPerParticle;
	public EntityCommandBuffer.ParallelWriter m_ecb;

	[BurstCompile]
	public void Execute(int a_index)
	{
		Entity newParticle = m_ecb.Instantiate(a_index, m_prefab);
		LocalTransform newTransform = new LocalTransform
		{
			Position = float3.zero,
			Rotation = quaternion.identity,
			//Scale = 0.001f
			Scale = 1f
		};
		m_ecb.SetComponent(a_index, newParticle, newTransform);
		m_ecb.AddComponent<ParticleColourComponent>(a_index, newParticle, new ParticleColourComponent { Value = new float4(1f, 1f, 1f, 1f)});

		BlobBuilder builder = new BlobBuilder(Allocator.Temp);
		ref ParticlePropertiesBlob ppBlob = ref builder.ConstructRoot<ParticlePropertiesBlob>();
		var depthArrayBuilder = builder.Allocate(ref ppBlob.m_depths, m_entriesPerParticle);
		var latArrayBuilder = builder.Allocate(ref ppBlob.m_lats, m_entriesPerParticle);
		var lonArrayBuilder = builder.Allocate(ref ppBlob.m_lons, m_entriesPerParticle);

		for (var j = 0; j < m_entriesPerParticle; j++)
		{
			depthArrayBuilder[j] = m_spawnData.Value.m_depths[a_index* m_entriesPerParticle + j];
			latArrayBuilder[j] = m_spawnData.Value.m_lats[a_index * m_entriesPerParticle + j];
			lonArrayBuilder[j] = m_spawnData.Value.m_lons[a_index * m_entriesPerParticle + j];
		}

		var blobAsset = builder.CreateBlobAssetReference<ParticlePropertiesBlob>(Allocator.Persistent);
		m_ecb.AddComponent(a_index, newParticle, new ParticleProperties
		{
			Value = blobAsset
		});
		builder.Dispose();
		//Debug.Log($"Thread {a_index} complete");
		//Important note: the created blob assets are not currently disposed, apply cleanup tags
	}
}

