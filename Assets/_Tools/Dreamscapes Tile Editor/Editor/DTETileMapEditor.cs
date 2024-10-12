using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dreamscapes.TileEditor
{
	///-/////////////////////////////////////////////////////////////////////////
	///
	public enum ToolMode
	{
		Default = 0,
		
		PlaceAsset = 1,
	}
	
	///-/////////////////////////////////////////////////////////////////////////
	///
	///	The editor script for the DTileMap class
	/// 
	[CustomEditor(typeof(DTETileMap))]
	public class DTETileMapEditor : Editor
	{
		[SerializeField] private VisualTreeAsset tree;

		private DTETileMap tileMap = null;
		private Transform assetsContainer;
		
		private ToolMode toolMode = ToolMode.Default;
		
		// Cached Visual Elements
		private VisualElement biomeAssetsContainer;
		
		// Tool Settings
		private bool gridSnapToggled = false;
		private float gridSnapSize = 1;
		
		// Place Asset ToolMode
		private GameObject selectedAsset;
		private GameObject selectedAssetPreview;

		///-////////////////////////////////////////////////
		///
		private void OnEnable()
		{
			SceneView.duringSceneGui += OnSceneBiomeAsset;
		}
		
		///-////////////////////////////////////////////////
		///
		private void OnDisable()
		{
			SceneView.duringSceneGui -= OnSceneBiomeAsset;
			
			SetToolMode(ToolMode.Default);
		}
		
#region CreateUI
		
		///-/////////////////////////////////////////////////////////////////////////
		/// 
		public override VisualElement CreateInspectorGUI()
		{
			VisualElement root = new VisualElement();
			
			tileMap = target as DTETileMap;
			if (tileMap != null)
			{
				// Get Asset Container
				assetsContainer = tileMap.transform.Find("AssetContainer");
				if (assetsContainer == null)
				{
					assetsContainer = new GameObject("AssetContainer").transform;
					assetsContainer.parent = tileMap.transform;
				}
			}
			
			toolMode = ToolMode.Default;

			// Error check, make sure that the tile map exists
			if (tileMap == null)
			{
				root.Add(new Label()
				{
					text = "Error: Tile map not found."
				});
				return root;
			}

			tree.CloneTree(root);

			//---- TILEMAP PROPERTIES ----//
			
			// Initialize the Biome Type Field
			EnumField biomeTypeField = root.Q<EnumField>(name="BiomeTypeField");
			biomeTypeField.RegisterValueChangedCallback(OnBiomeTypeChanged);
			biomeTypeField.value = tileMap.biomeType;

			// Initialize the Biome Seed Slider
			SliderInt biomeSeedSlider = root.Q<SliderInt>(name = "BiomeSeedSlider");
			biomeSeedSlider.RegisterValueChangedCallback(OnBiomeSeedChanged);
			biomeSeedSlider.value = tileMap.biomeSeed;
			
			// Initialize the Grid Size Field
			Vector2IntField gridSizeField = root.Q<Vector2IntField>(name="GridSizeField");
			gridSizeField.RegisterValueChangedCallback(OnGridSizeChanged);
			gridSizeField.value = tileMap.gridSize;
			
			//---- TOOL SETTINGS ----//
			
			// Initialize Grid Snap Toggle
			Toggle gridSnapToggle = root.Q<Toggle>(name = "GridSnapToggle");
			gridSnapToggle.RegisterValueChangedCallback(OnGridSnapToggled);
			gridSnapToggled = gridSnapToggle.value;
			
			// Initialize Grid Snap Size Field
			FloatField gridSnapSizeField = root.Q<FloatField>(name = "GridSnapSizeField");
			gridSnapSizeField.RegisterValueChangedCallback(OnGridSnapSizeChanged);
			gridSnapSize = gridSnapSizeField.value;
			
			//---- BIOME ASSETS ----//
			biomeAssetsContainer = root.Q<VisualElement>(name = "BiomeAssetsContainer");
			RefreshBiomeAssetsContainer();
				
			return root;
		}

		///-//////////////////////////////////////////////////////////////////
		///
		public override void OnInspectorGUI()
		{
			RefreshBiomeAssetsContainer();
		}
		
#endregion //CreateUI

#region Registered Callback Methods

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnBiomeTypeChanged(ChangeEvent<Enum> newBiomeType)
		{
			tileMap.SetBiomeType((BiomeType) newBiomeType.newValue);
			RefreshOtherUI();
		}
		
		///-//////////////////////////////////////////////////////////////////
		///
		private void OnBiomeSeedChanged(ChangeEvent<int> newBiomeSeed)
		{
			tileMap.SetBiomeSeed(newBiomeSeed.newValue);
			RefreshOtherUI();
		}
		
		///-//////////////////////////////////////////////////////////////////
		///
		private void OnGridSizeChanged(ChangeEvent<Vector2Int> newGridSize)
		{
			tileMap.SetGridSize(newGridSize.newValue);
			RefreshOtherUI();
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnGridSnapToggled(ChangeEvent<bool> toggled)
		{
			gridSnapToggled = toggled.newValue;
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnGridSnapSizeChanged(ChangeEvent<float> newGridSnapSize)
		{
			gridSnapSize = newGridSnapSize.newValue;
		}
		
		///-//////////////////////////////////////////////////////////////////
		///
		private void RefreshOtherUI()
		{
			RefreshBiomeAssetsContainer();
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void RefreshBiomeAssetsContainer()
		{
			biomeAssetsContainer.Clear();
			
			SODTEBiome biome = tileMap.GetBiome();

			string folderPath = biome.prefabsFolderPath;
			string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[]{ folderPath });

			Vector2 buttonSize = new Vector2(100, 100);
			
			foreach (string prefabGuid in prefabGuids)
			{
				// Get the prefab and get images for ui button
				string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuid);
				GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
				Texture2D prefabTexture = AssetPreview.GetAssetPreview(prefab);
				Image prefabImage = new Image();
				
				// Check if asset preview is still loading
				if (AssetPreview.IsLoadingAssetPreview(prefab.GetInstanceID()))
				{
					prefabImage.image = Resources.Load<Texture>("LoadingIcon");
					Repaint();
				}
				else
				{
					prefabImage.image = prefabTexture;
				}
				
				// Create the button used for selecting the prefab
				Button newPrefabButton = new Button();
				newPrefabButton.style.width = buttonSize.x;
				newPrefabButton.style.height = buttonSize.y;
				newPrefabButton.Add(prefabImage);
                        
				biomeAssetsContainer.Add(newPrefabButton);
				newPrefabButton.clickable.clicked += () => { OnSelectBiomeAsset(prefab); };
			}
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnSelectBiomeAsset(GameObject biomeAsset)
		{
			SetToolMode(ToolMode.Default);	// Exit and then reenter just in case there is an already existing preview
			
			selectedAsset = biomeAsset;
			SetToolMode(ToolMode.PlaceAsset);
		}
		
#endregion // #region Registered Callback Methods

#region Tool Mode State Machine

		///-//////////////////////////////////////////////////////////////////
		///
		private void SetToolMode(ToolMode newToolMode)
		{
			if (toolMode == newToolMode)
			{
				return;
			}

			ToolMode prevToolMode = toolMode;
			toolMode = newToolMode;
			
			// Call Exit Methods
			switch (prevToolMode)
			{
				case ToolMode.PlaceAsset:
					OnExitPlaceAssetMode();
					break;
				case ToolMode.Default:
				default:
					break;
			}
			
			// Call Enter Methods
			switch (toolMode)
			{
				case ToolMode.PlaceAsset:
					OnEnterPlaceAssetMode();
					break;
				case ToolMode.Default:
				default:
					break;
			}
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnEnterPlaceAssetMode()
		{
			CreateAssetPreview();
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnExitPlaceAssetMode()
		{
			DestroyAssetPreview();
			
			selectedAsset = null;
		}

#endregion // Tool Mode State Machine

#region OnSceneBiomeAsset

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnSceneBiomeAsset(SceneView sceneView)
		{
			if (toolMode != ToolMode.PlaceAsset)
			{
				return;
			}
			
			Event evt = Event.current;
			int id = GUIUtility.GetControlID(FocusType.Passive);
                
			switch (evt.type)
			{
				case EventType.Layout:
				case EventType.MouseMove:
				{
					// AddDefaultControl means that if no other control is selected, this will be chosen as the fallback.
					// This allows things like the translate handle and buttons to function.
					HandleUtility.AddDefaultControl(id);

					UpdateAssetPreviewPosition(evt);
					break;
				}
				case EventType.MouseDown:
					if (evt.button == 0 && HandleUtility.nearestControl == id)
					{
						// Tells the scene view that the placing prefab event is taking place and to ignore other related events 
						GUIUtility.hotControl = id;

						// Raycast from mouse to world
						Ray ray = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
						bool hit = Physics.Raycast(ray, out RaycastHit hitInfo);
						
						// Check if hit, if so -> Place Asset
						if (hit)
						{
							PlaceAsset(evt);
						}
						else
						{
							// Otherwise, set tool mode to default
							SetToolMode(ToolMode.Default);
						}

						evt.Use();
					}
					break;
				case EventType.MouseUp:
					if (evt.button == 0 && GUIUtility.hotControl == id)
					{
						GUIUtility.hotControl = 0; // resets hot control

						evt.Use();
					}
					break;
			}
		}
		
		///-//////////////////////////////////////////////////////////////////
		///
		private void CreateAssetPreview()
		{
			selectedAssetPreview = (GameObject)PrefabUtility.InstantiatePrefab(selectedAsset);
			selectedAssetPreview.transform.SetParent(assetsContainer);
			selectedAssetPreview.SetLayerInAllChildren(2);
			
			// Set material to preview highlight shader
			MeshRenderer[] renderers = selectedAssetPreview.GetComponentsInChildren<MeshRenderer>();
			Material previewHighlightShader = tileMap.GetBiome().previewHighlightShader;
			
			foreach (MeshRenderer renderer in renderers)
			{
				renderer.sharedMaterial = previewHighlightShader;
			}
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void DestroyAssetPreview()
		{
			if (selectedAssetPreview != null)
			{
				DestroyImmediate(selectedAssetPreview);
				selectedAssetPreview = null;
			}
		}
		
		///-//////////////////////////////////////////////////////////////////
		///
		private void PlaceAsset(Event evt)
		{
			Transform placedAsset = ((GameObject)PrefabUtility.InstantiatePrefab(selectedAsset)).transform;
			placedAsset.parent = assetsContainer;
			placedAsset.position = selectedAssetPreview.transform.position;  

			//records the object so that it can be undone and sets it as dirty (so that unity saves it)
			Undo.RegisterCreatedObjectUndo(placedAsset.gameObject, "DTE: Placed Biome Asset");
			EditorUtility.SetDirty(placedAsset.gameObject);

			evt.Use();
		}
		
		///-//////////////////////////////////////////////////////////////////
		///
		private void UpdateAssetPreviewPosition(Event evt)
		{
			if (selectedAssetPreview == null)
			{
				return;
			}
			
			// Raycast from mouse to world
			Ray ray = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
			bool hit = Physics.Raycast(ray, out RaycastHit hitInfo);
				
			if (hit) // if the prefab was placed on another GameObject
			{
				if (gridSnapToggled)
				{
					// Set preview to snapped hit position on grid
					// Offset snap depending on grid size
					float xPos, zPos;
					xPos = (tileMap.gridSize.x % 2 == 1) 
						? hitInfo.point.x.Round(gridSnapSize) 
						: hitInfo.point.x.Round(gridSnapSize, tileMap.biomeManager.tileSize.x / 2); 
					zPos = (tileMap.gridSize.y % 2 == 1) 
						? hitInfo.point.z.Round(gridSnapSize)
						: hitInfo.point.z.Round(gridSnapSize, tileMap.biomeManager.tileSize.y / 2);
					selectedAssetPreview.transform.position = new Vector3(xPos, hitInfo.point.y, zPos);
				}
				else
				{
					// Set preview to hit point
					selectedAssetPreview.transform.position = hitInfo.point;
				}
			}
		}
		
#endregion // OnSceneBiomeAsset
		
	}

}