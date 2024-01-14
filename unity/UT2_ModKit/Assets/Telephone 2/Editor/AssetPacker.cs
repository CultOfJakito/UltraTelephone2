﻿// Builds an asset bundle from the selected objects in the project view.
// Once compiled go to "Menu" -> "Assets" and select one of the choices
// to build the Asset Bundle
using UnityEngine;
using UnityEditor;

public class ExportAssetBundles
{

    [MenuItem("Assets/Build AssetBundle From Selection - Track dependencies")]
    static void ExportResurce()
    {
        // Bring up save panel
        string basename = Selection.activeObject ? Selection.activeObject.name : "New Resource";
        string path = EditorUtility.SaveFilePanel("Save Resources", "", basename, "");

        if (path.Length != 0)
        {
            // Build the resource file from the active selection.
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

            // for Android
            BuildPipeline.BuildAssetBundle(Selection.activeObject,
                                           selection, path + ".resource",
                                           BuildAssetBundleOptions.CollectDependencies |
                                           BuildAssetBundleOptions.CompleteAssets,
                                           BuildTarget.StandaloneWindows64);

            Selection.objects = selection;
        }
    }

    // [MenuItem("Assets/Build AssetBundle From Selection - Track dependencies")]
    // static void ExportResource() {
    //   // Bring up save panel
    //   string path = EditorUtility.SaveFilePanel("Save Resources", "", "New Resource", "unity3d");
    //   if (path.Length != 0) {
    //     // Build the resource file from the active selection.
    //     Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
    //     BuildPipeline.BuildAssetBundle(Selection.activeObject,
    //                                    selection,
    //                                    path,
    //                                    BuildAssetBundleOptions.CollectDependencies |
    //                                    BuildAssetBundleOptions.CompleteAssets);
    //     Selection.objects = selection;
    //   }
    // }

    // [MenuItem("Assets/Build AssetBundle From Selection - No dependency tracking")]
    // static void ExportResourceNoTrack() {
    //   // Bring up save panel
    //   string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
    //   if (path.Length != 0) {
    //     // Build the resource file from the active selection.
    //     BuildPipeline.BuildAssetBundle(Selection.activeObject, Selection.objects, path);
    //   }
    // }

}