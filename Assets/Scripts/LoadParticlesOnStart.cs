using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Scenes;
using Unity.Collections;
using Unity.Entities.UniversalDelegates;
using System.Linq;

public class LoadParticlesOnStart : MonoBehaviour
{
	public TextAsset m_depthCSVFile;
	public TextAsset m_latitudeCSVFile;
	public TextAsset m_longitudeCSVFile;
	public float m_timePerTimeIndex;

	bool m_initialized;

	public void Update()
	{
		if (m_initialized)
			return;
		m_initialized = true;

		int entriesPerParticle = -1;
		float[] depth, lat, lon;
		if (TryParseCSVVert(m_depthCSVFile, ref entriesPerParticle, out depth) && TryParseCSVVert(m_latitudeCSVFile, ref entriesPerParticle, out lat) && TryParseCSVVert(m_longitudeCSVFile, ref entriesPerParticle, out lon))
		{
			EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			EntityQuery query = entityManager.CreateEntityQuery(typeof(ParticleVisualizationSettingsData));
			Entity root = query.GetSingletonEntity();

			BlobBuilder builder = new BlobBuilder(Allocator.Temp);
			ref ParticlePropertiesBlob ppBlob = ref builder.ConstructRoot<ParticlePropertiesBlob>();
			BlobBuilderArray<float> depthArrayBuilder = builder.Allocate(ref ppBlob.m_depths, depth.Length);
			BlobBuilderArray<float> latArrayBuilder = builder.Allocate(ref ppBlob.m_lats, lat.Length);
			BlobBuilderArray<float> lonArrayBuilder = builder.Allocate(ref ppBlob.m_lons, lon.Length);

			for (var j = 0; j < depth.Length; j++)
			{
				depthArrayBuilder[j] = depth[j];
				latArrayBuilder[j] = lat[j];
				lonArrayBuilder[j] = lon[j];
			}

			BlobAssetReference<ParticlePropertiesBlob> blobAsset = builder.CreateBlobAssetReference<ParticlePropertiesBlob>(Allocator.Persistent);
			entityManager.AddComponentData(root, new ParticleSpawnData
			{
				Value = blobAsset,
				m_entriesPerParticle = entriesPerParticle
			});
			builder.Dispose();
			entityManager.AddComponentData(root, new ParticleTimingData
			{
				m_numberIndices = entriesPerParticle,
				m_timeIndex = 0,
				m_timePassed = 0f,
				m_timePerIndex = m_timePerTimeIndex
			});
		}
	}

	private bool TryParseCSV(TextAsset a_csvFile, ref int a_entriesPerParticle, out float[] result)
	{
		string[] lines = a_csvFile.text.Split('\n');

		string[] entries = lines[0].Split(',');
		if (a_entriesPerParticle < 0)
		{
			a_entriesPerParticle = entries.Length;
		}
		else if (a_entriesPerParticle != entries.Length)
		{
			Debug.LogError($"Inconsistent amount of particle data on line 0 in file \"{a_csvFile.name}\", parsing failed.");
			result = null;
			return false;
		}
		result = new float[lines.Length*entries.Length];

		int i = 0;
		while (true)
		{
			
			if (a_entriesPerParticle < 0)
			{
				a_entriesPerParticle = entries.Length;
			}
			else if (a_entriesPerParticle != entries.Length)
			{
				Debug.LogError($"Inconsistent amount of particle data on line {i} in file \"{a_csvFile.name}\", parsing failed.");
				result = null;
				return false;
			}

			for (int j = 0; j < entries.Length; j++)
			{
				if (float.TryParse(entries[j], out float value))
				{
					result[i* a_entriesPerParticle + j] = value;
				}
				else
				{
					Debug.LogError($"Error parsing CSV value into float at row {i}, column {j} in file \"{a_csvFile.name}\"");
					result = null;
					return false;
				}
			}

			i++;
			if (i == lines.Length || lines[i] == "")
			{
				break;
			}
			else
				entries = lines[i].Split(',');
		}
		return true;
	}

	private bool TryParseCSVVert(TextAsset a_csvFile, ref int a_entriesPerParticle, out float[] result)
	{
		string[] lines = a_csvFile.text.Split('\n');

		if (a_entriesPerParticle < 0)
		{
			a_entriesPerParticle = lines.Length;
		}
		else if (a_entriesPerParticle != lines.Length)
		{
			Debug.LogError($"Inconsistent amount of particle data in file \"{a_csvFile.name}\", parsing failed. The file has {lines.Length} entries per particle instead of {a_entriesPerParticle}.");
			result = null;
			return false;
		}
		string[] entries = lines[0].Split(',');
		int numberParticles = entries.Length;
		//int numberParticles = 92;
		result = new float[lines.Length * numberParticles];

		int i = 0;
		while (true)
		{
			//if (numberParticles != entries.Length)
			//{
			//	Debug.LogError($"Inconsistent amount of particle data on line {i} in file \"{a_csvFile.name}\", parsing failed. The line has {entries.Length} entries per instead of {numberParticles}.");
			//	result = null;
			//	return false;
			//}

			for (int j = 0; j < numberParticles; j++)
			{
				if (float.TryParse(entries[j], out float value))
				{
					result[i + a_entriesPerParticle * j] = value;
				}
				else
				{
					Debug.LogError($"Error parsing CSV value into float at row {i}, column {j} in file \"{a_csvFile.name}\"");
					result = null;
					return false;
				}
			}

			i++;
			if (i == lines.Length || lines[i] == "")
			{
				break;
			}
			else
				entries = lines[i].Split(',');
		}
		return true;
	}

}
