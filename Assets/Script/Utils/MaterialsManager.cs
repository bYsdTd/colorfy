using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialManager  
{
	static private MaterialManager instance = null;

	static public MaterialManager Instance()
	{
		if(instance == null)
		{
			instance = new MaterialManager();
		}

		return instance;
	}
		
	Dictionary<string, Material> cached_materials = new Dictionary<string, Material>();

	public Material GetMaterial(string material_name)
	{
		if(cached_materials.ContainsKey(material_name))
		{
			return cached_materials[material_name];
		}
		else
		{
			Material mat = Resources.Load<Material>(AssetsPathConfig.assets_path_config[material_name]);

			cached_materials.Add(material_name, mat);

			return mat;
		}
	}
}
