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

	public void Start()
	{
		int entriesPerParticle = -1;
		float[] depth, lat, lon;
		if (TryParseCSVVert(m_depthCSVFile, ref entriesPerParticle, out depth) && TryParseCSVVert(m_latitudeCSVFile, ref entriesPerParticle, out lat) && TryParseCSVVert(m_longitudeCSVFile, ref entriesPerParticle, out lon))
		{
			var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			EntityQuery query = entityManager.CreateEntityQuery(typeof(ParticleVisualizationSettingsData));
			Entity root = query.GetSingletonEntity();

			ParticleSpawnData spawnData = new ParticleSpawnData
			{
				m_depths = new NativeArray<float>(depth, Allocator.Persistent),
				m_lats = new NativeArray<float>(lat, Allocator.Persistent),
				m_lons = new NativeArray<float>(lon, Allocator.Persistent),
				m_entriesPerParticle = entriesPerParticle
			};
			entityManager.AddComponentData(root, spawnData);
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
		result = new float[lines.Length * numberParticles];

		int i = 0;
		while (true)
		{
			if (numberParticles != entries.Length)
			{
				Debug.LogError($"Inconsistent amount of particle data on line {i} in file \"{a_csvFile.name}\", parsing failed. The line has {entries.Length} entries per instead of {numberParticles}.");
				result = null;
				return false;
			}

			for (int j = 0; j < entries.Length; j++)
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
