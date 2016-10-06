using UnityEngine;
using UnityEditor;
using System.Collections;

public class BuildAssetsBundle : MonoBehaviour {


	[MenuItem("Assets/Build/Build Assets Bundle")]

	static void ExportResources () {
		BuildPipeline.BuildAssetBundles ("AssetsBundle", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
	}
}
