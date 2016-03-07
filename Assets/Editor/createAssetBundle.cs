using UnityEditor;
using UnityEngine;


public class CreateAssetBundles
{
	[MenuItem ("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles ()
	{
		#if UNITY_EDITOR
         UnityEditor.ScriptingImplementation backend = (UnityEditor.ScriptingImplementation)UnityEditor.PlayerSettings.GetPropertyInt("ScriptingBackend", UnityEditor.BuildTargetGroup.iOS);
         if (backend != UnityEditor.ScriptingImplementation.IL2CPP) {
			 Debug.LogError ("Warning: If the scripting backend is not IL2CPP there may be problems");
         }
		#endif
		
		BuildPipeline.BuildAssetBundles ("Assets/AssetBundles", BuildAssetBundleOptions.ForceRebuildAssetBundle, EditorUserBuildSettings.activeBuildTarget);
	}
}
