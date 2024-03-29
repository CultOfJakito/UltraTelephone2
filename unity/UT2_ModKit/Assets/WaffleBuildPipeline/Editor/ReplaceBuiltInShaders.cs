using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ultracrypt.Editor.WaffleBuildPipeline
{
    public class ReplaceBuiltInShaders
    {
	    [MenuItem("WaffleBuildPipeline/Replace BuiltIn Shaders")]
	    public void ReplaceInSceneAndPrefabs()
	    {
			ReplaceDefaultAssets(true);
			ReplaceDefaultAssets(false);
	    }
	    
        private static void ReplaceDefaultAssets(bool doPrefabsInsteadOfScene)
		{
			Debug.Log("Looking for default assets to replace...");

			var unityBuiltInPath = "Resources/unity_builtin_extra";
			var tundraResourcesPath = "Assets/Tundra/Core/BuiltInResources";

			var knownBaseTypes = new List<Type> { typeof(Renderer) };

			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();

			// Collect all relevant components in scene
			var relevantComponents = new List<Component>();

			if (doPrefabsInsteadOfScene)
			{
				// Load all non-scene assets
				Resources.LoadAll("Crypt_Assets");
				Resources.LoadAll("ULTRAKILL Prefabs");
				var rootObjects = Resources.FindObjectsOfTypeAll<GameObject>().Where(go => !EditorUtility.IsPersistent(go.transform.root.gameObject) && go.hideFlags != HideFlags.HideAndDontSave && go.hideFlags != HideFlags.NotEditable).ToArray();

				foreach (var rootObject in rootObjects)
				{
					var components = rootObject.GetComponentsInChildren<Component>(true);
					foreach (var component in components)
					{
						if (component == null) continue;

						var componentType = component.GetType();

						if (knownBaseTypes.Any(baseType => baseType.IsAssignableFrom(componentType)))
						{
							relevantComponents.Add(component);
						}
					}
				}
			}
			else
			{
				var rootObjects = Resources.FindObjectsOfTypeAll<GameObject>().Where(go => EditorUtility.IsPersistent(go.transform.root.gameObject) && go.hideFlags != HideFlags.HideAndDontSave && go.hideFlags != HideFlags.NotEditable).ToArray();

				foreach (var rootObject in rootObjects)
				{
					var components = rootObject.GetComponentsInChildren<Component>(true);
					foreach (var component in components)
					{
						if (component == null) continue;

						var componentType = component.GetType();

						if (knownBaseTypes.Any(baseType => baseType.IsAssignableFrom(componentType)))
						{
							relevantComponents.Add(component);
						}
					}
				}
			}

			Debug.Log($"Found {relevantComponents.Count} relevant components");

			var replaced = 0;
			foreach (var component in relevantComponents)
			{
				if (component is Renderer renderer)
				{
					var materials = renderer.sharedMaterials;
					for (var i = 0; i < materials.Length; i++)
					{
						var material = materials[i];
						if (material == null) continue;
						// Check if material is a built-in resource
						var ownPath = AssetDatabase.GetAssetPath(material);
						if (!ownPath.StartsWith(unityBuiltInPath)) continue;

						Debug.Log($"Found built-in material: {ownPath}/{material.name}");

						// Create duplicate or reuse duplicate
						var duplicatePath = $"{tundraResourcesPath}/{material.name}.asset";
						var duplicate = AssetDatabase.LoadAssetAtPath<Material>(duplicatePath);
						if (duplicate == null)
						{
							Debug.Log($"Creating duplicate: {duplicatePath}");
							duplicate = new Material(material);
							AssetDatabase.CreateAsset(duplicate, duplicatePath);
						}
						else
						{
							Debug.Log($"Reusing duplicate: {duplicatePath}");
						}

						// Replace material
						materials[i] = duplicate;
						renderer.sharedMaterials = materials;
						replaced++;

						EditorUtility.SetDirty(renderer);

					}
				}
			}

			sw.Stop();
			Debug.Log($"Replaced {replaced} default assets in {sw.Elapsed.TotalSeconds:0.00} seconds");

			// Ensure changes submitted before build
			AssetDatabase.SaveAssets();
			Resources.UnloadUnusedAssets();
		}
    }
}