using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Pipeline;
using UnityEngine;

namespace Ultracrypt.Editor.WaffleBuildPipeline
{
	public class BuildAddressables : MonoBehaviour
	{
		//EDIT THESE!!

		//AssetPathLocation needs to lead to a getter that returns the path where you store all your bundles in the mod.
		private const string AssetPathLocation = "{CultOfJakito.UltraTelephone2.Assets.AddressableManager.AssetPath}";

		//This is just the name of your mod.
		private const string MonoscriptBundleNaming = "ultratelephone2";

		//Don't touch these unless you know what you're doing.
		private const string ResultPath = "Built Bundles";
		private const string WbpTemplateName = "WBP Assets";
		private const string CatalogPostfix = "wbp";
		private const string EmptyGroupName = "Empty Dont Delete";
		private const string EmptyAssetPath = "Assets/WaffleBuildPipeline/Assets/Empty.png";
		private static string[] s_commonGroupNames = { "Assets", "Game Prefabs", "Music", "Other" };

		private static AddressableAssetSettings Settings => AddressableAssetSettingsDefaultObject.Settings;

		[MenuItem("Addressable Build Pipeline/Debug Fast Build")]
		public static void FastBuildButton()
		{
			Build(false);
		}

		[MenuItem("Addressable Build Pipeline/Release Build")]
		public static void BuildButton()
		{
			Build(true);
		}

		private static void Build(bool buildCommon)
		{
			ValidateAddressables();
			SetCorrectValuesForSettings();
			EnsureCustomTemplateExists();
			CreateEmptyGroup();

			if (!Directory.Exists(ResultPath))
			{
				Directory.CreateDirectory(ResultPath);
			}

			if (buildCommon)
			{
				BuildContent();
			}
			else
			{
				BuildContentFast();
			}

			ReplaceBuiltInWithEmpty();
		}

		private static void BuildContent()
		{
			AddressableAssetSettings.BuildPlayerContent();
		}

		// removes common groups (which take ages) but their needed assets are put in your bundles, increasing size
		private static void BuildContentFast()
		{
			List<AddressableAssetGroup> commonGroups = new List<AddressableAssetGroup>(Settings.groups.Where(group => s_commonGroupNames.Contains(group.name)));
			Settings.groups.RemoveAll(commonGroups.Contains);
            ValidateAddressables();
			AddressableAssetSettings.BuildPlayerContent();
			Settings.groups.AddRange(commonGroups);
			ValidateAddressables(); // this method refreshes asset db, means that you dont get missing references when readding groups
		}

		private static void SetCorrectValuesForSettings()
		{
			Settings.profileSettings.CreateValue("ModBuildPath", ResultPath);
			Settings.profileSettings.CreateValue("ModLoadPath", AssetPathLocation);
			Settings.profileSettings.SetValue(Settings.activeProfileId, "ModBuildPath", ResultPath);
			Settings.profileSettings.SetValue(Settings.activeProfileId, "ModLoadPath", AssetPathLocation);

			Settings.IgnoreUnsupportedFilesInBuild = true;
			Settings.OverridePlayerVersion = CatalogPostfix;
			Settings.BuildRemoteCatalog = true;
			Settings.RemoteCatalogBuildPath.SetVariableByName(Settings, "ModBuildPath");
			Settings.RemoteCatalogLoadPath.SetVariableByName(Settings, "ModLoadPath");
			Settings.MonoScriptBundleNaming = MonoScriptBundleNaming.Custom;
			Settings.MonoScriptBundleCustomNaming = MonoscriptBundleNaming;
			Settings.ShaderBundleNaming = ShaderBundleNaming.Custom;
			Settings.ShaderBundleCustomNaming = "shader";
		}

		private static void EnsureCustomTemplateExists()
		{
			foreach (ScriptableObject so in Settings.GroupTemplateObjects)
			{
				if ((bool)so && so.name == WbpTemplateName)
				{
					return;
				}
			}

			if (!Settings.CreateAndAddGroupTemplate(WbpTemplateName, "Assets for Waffle's Build Pipeline.", typeof(BundledAssetGroupSchema)))
			{
				Debug.LogError($"Failed to create the '{WbpTemplateName}' template, whar?");
				return;
			}

			AddressableAssetGroupTemplate wbpAssetsTemplate = Settings.GroupTemplateObjects[Settings.GroupTemplateObjects.Count - 1] as AddressableAssetGroupTemplate;

			if ((bool)wbpAssetsTemplate && wbpAssetsTemplate.Name != WbpTemplateName)
			{
				Debug.LogError($"Somehow got wrong template, this shouldn't be possible? [got {wbpAssetsTemplate.name}]");
				return;
			}

			BundledAssetGroupSchema groupSchema = wbpAssetsTemplate.GetSchemaByType(typeof(BundledAssetGroupSchema)) as BundledAssetGroupSchema;

			if (!(bool)groupSchema)
			{
				Debug.LogError("Getting the schema from the template is null?");
				return;
			}

			SetCorrectValuesForSchema(groupSchema);
		}

		// TundraEditor: Core/Editor/TundraInit.cs
		// thanks pitr i stole this completely ;3
		private static void ValidateAddressables(bool forceRewrite = false)
		{
			// TODO check the content
			var templatePostfix = ".template";
			var metaPostfix = ".meta";

			var assetPath = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
			var assetTemplatePath = assetPath + templatePostfix;

			var metaPath = assetPath + metaPostfix;
			var metaTemplatePath = assetPath + metaPostfix + templatePostfix;

			var valid = File.Exists(assetPath);

			if (!valid || forceRewrite)
			{
				Debug.Log($"Rewriting Addressables: {assetPath}");
				File.Copy(assetTemplatePath, assetPath, true);
				File.Copy(metaTemplatePath, metaPath, true);
				// Mark the asset database as dirty to force a refresh
				AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
				AddressableAssetSettingsDefaultObject.Settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(assetPath);
			}
		}

		private static void SetCorrectValuesForSchema(BundledAssetGroupSchema groupSchema)
		{
			groupSchema.IncludeInBuild = true;
			groupSchema.IncludeAddressInCatalog = true;
			groupSchema.BuildPath.SetVariableByName(Settings, "ModBuildPath");
			groupSchema.LoadPath.SetVariableByName(Settings, "ModLoadPath");
			groupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
			groupSchema.UseAssetBundleCrcForCachedBundles = false;
			groupSchema.UseAssetBundleCrc = false;
		}

		private static void CreateEmptyGroup()
		{
			if (Settings.groups.Any(x => x.name == EmptyGroupName))
			{
				return;
			}

			AddressableAssetGroup group = Settings.CreateGroup(EmptyGroupName, false, false, false, null, typeof(BundledAssetGroupSchema));
			SetCorrectValuesForSchema(group.GetSchema<BundledAssetGroupSchema>());
			List<AddressableAssetEntry> entries = new List<AddressableAssetEntry>
			{
				Settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(EmptyAssetPath), group, false, false)
			};

			group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, entries, false, true);
			Settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, entries, true, false);
		}

		private static void ReplaceBuiltInWithEmpty()
		{
			string emptyPath = Path.Combine(ResultPath, $"{EmptyGroupName.Replace(" ", "").ToLower()}_assets_all.bundle");
			string shaderPath = Path.Combine(ResultPath, $"{Settings.ShaderBundleCustomNaming}_unitybuiltinshaders.bundle");
			File.Delete(shaderPath);
			File.Move(emptyPath, shaderPath);
		}
	}
}
