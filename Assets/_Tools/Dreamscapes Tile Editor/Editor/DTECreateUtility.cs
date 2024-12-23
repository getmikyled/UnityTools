using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Dreamscape.TileEditor
{
	///-/////////////////////////////////////////////////////////////////////////
	///
	public static class DTECreateUtility
	{
		private const string TILE_MAP_PREFAB_PATH = "Assets/Tools/Dreamscapes Tile Editor/Prefabs/TileMap.prefab";
		
		///-/////////////////////////////////////////////////////////////////////////
		///
		[MenuItem("GameObject/Dreamscapes/TileMap")]
		public static void CreateTileMap(MenuCommand menuCommand)
		{
			GameObject tileMap = CreatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(TILE_MAP_PREFAB_PATH));
			tileMap.GetComponent<DTETileMap>().SetBiomeType(BiomeType.Dreamscape);
		}

		///-/////////////////////////////////////////////////////////////////////////
		///
		public static GameObject CreatePrefab(GameObject prefab)
		{
			GameObject newObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
			PlaceObjectInSceneView(newObject);
			return newObject;
		}

		///-/////////////////////////////////////////////////////////////////////////
		///
		public static void PlaceObjectInSceneView(GameObject gameObject)
		{
			// Find location
			SceneView sceneView = SceneView.lastActiveSceneView;
			gameObject.transform.position = (sceneView) ? sceneView.pivot : Vector3.zero;
			
			// Place object in scene with unique name
			StageUtility.PlaceGameObjectInCurrentStage(gameObject);
			GameObjectUtility.EnsureUniqueNameForSibling(gameObject);
			
			// Record undo and select new object
			Undo.RegisterCreatedObjectUndo(gameObject, "DTE: Created object " + gameObject.name);
			Selection.activeGameObject = gameObject;
			
			// Mark scene as dirty
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}
	}

}