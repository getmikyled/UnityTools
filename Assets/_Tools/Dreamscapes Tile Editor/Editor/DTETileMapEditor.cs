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
		
		TileBrush = 1,
		BiomeAsset = 2,
	}

	///-/////////////////////////////////////////////////////////////////////////
	///
	public enum Tab
	{
		None = 0,
		
		TileBrush = 1,
		BiomeAssets = 2,
	}
	
	///-/////////////////////////////////////////////////////////////////////////
	///
	
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
		private Tab tab = Tab.None;
		
		// Cached Visual Elements
		private VisualElement tileBrushContainer;
		private VisualElement biomeAssetsContainer;
		
		// Tool Settings
		private bool gridSnapToggled = false;
		private float gridSnapSize = 1;
		
		// Tile Brush Tool Mode
		private GameObject selectedTileBrush;
		private GameObject selectedTileBrushPreview;
		private GameObject hiddenTile;
		
		// Place Asset ToolMode
		private GameObject selectedAsset;
		private GameObject selectedAssetPreview;

		///-////////////////////////////////////////////////
		///
		private void OnEnable()
		{
			SceneView.duringSceneGui += OnSceneTileBrush;
			SceneView.duringSceneGui += OnSceneBiomeAsset;
		}
		
		///-////////////////////////////////////////////////
		///
		private void OnDisable()
		{
			SceneView.duringSceneGui -= OnSceneTileBrush;
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
			
			// Initialize Grid Snap Size Field
			Slider gridSnapSizeSlider = root.Q<Slider>(name = "GridSnapSlider");
			gridSnapSizeSlider.RegisterValueChangedCallback(OnGridSnapSizeChanged);
			gridSnapSize = gridSnapSizeSlider.value;
			
			//---- TAB BUTTONS ----//
			Button tileBrushTabButton = root.Q<Button>(name = "TileBrushTab");
			tileBrushTabButton.clicked += () => { SetTab(Tab.TileBrush); };
			Button biomeAssetsTabButton = root.Q<Button>(name = "BiomeAssetsTab");
			biomeAssetsTabButton.clicked += () => { SetTab(Tab.BiomeAssets); };
			
			//---- TILE BRUSH ----//
			tileBrushContainer = root.Q<VisualElement>(name = "BiomeAssetsContainer");
			
			//---- BIOME ASSETS ----//
			biomeAssetsContainer = root.Q<VisualElement>(name = "BiomeAssetsContainer");
			
			SetTab(Tab.TileBrush);
				
			return root;
		}

		///-/////////////////////////////////////////////////////////////////////////
		///
		public override bool UseDefaultMargins()
		{
			return false;
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

		private void RefreshTileBrushContainer()
		{
			tileBrushContainer.Clear();
            			
            SODTEBiome biome = tileMap.GetBiome();
            
            Vector2 buttonSize = new Vector2(100, 100);

            foreach (GameObject prefab in biome.tiles)
            {
	            // Get the prefab's texture
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
                        
	            tileBrushContainer.Add(newPrefabButton);
	            newPrefabButton.clickable.clicked += () => { OnSelectTileBrush(prefab); };
            }
		}
		
		///-//////////////////////////////////////////////////////////////////
		///
		private void OnSelectTileBrush(GameObject tileBrush)
		{
			SetToolMode(ToolMode.Default);	// Exit and then reenter just in case there is an already existing preview
			
			selectedTileBrush = tileBrush;
			SetToolMode(ToolMode.TileBrush);
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
			SetToolMode(ToolMode.BiomeAsset);
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
				case ToolMode.TileBrush:
					OnExitTileBrushMode();
					break;
				case ToolMode.BiomeAsset:
					OnExitBiomeAssetMode();
					break;
				case ToolMode.Default:
				default:
					break;
			}
			
			// Call Enter Methods
			switch (toolMode)
			{
				case ToolMode.TileBrush:
					OnEnterTileBrushMode();
					break;
				case ToolMode.BiomeAsset:
					OnEnterBiomeAssetMode();
					break;
				case ToolMode.Default:
				default:
					break;
			}
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnEnterTileBrushMode()
		{
			CreateTileBrushPreview();
		}
		
		///-//////////////////////////////////////////////////////////////////
		///
		private void OnExitTileBrushMode()
		{
			SetHiddenTile(null);
			DestroyTileBrushPreview();
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnEnterBiomeAssetMode()
		{
			CreateBiomeAssetPreview();
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnExitBiomeAssetMode()
		{
			DestroyBiomeAssetPreview();
			
			selectedAsset = null;
		}

#endregion // Tool Mode State Machine

#region Tab State Machine

		///-//////////////////////////////////////////////////////////////////
		///
		private void SetTab(Tab newTab)
		{
			if (tab == newTab)
			{
				return;
			}

			Tab prevTab = newTab;
			tab = newTab;
					
			// Call Exit Methods
			switch (prevTab)
			{
				case Tab.TileBrush:
					OnExitTileBrushTab();
					break;
				case Tab.BiomeAssets:
					OnExitBiomeAssetsTab();
					break;
				default:
					break;
			}
					
			// Call Enter Methods
			switch (tab)
			{
				case Tab.TileBrush:
					OnEnterTileBrushTab();
					break;
				case Tab.BiomeAssets:
					OnEnterBiomeAssetsTab();
					break;
				default:
					break;
			}
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnEnterTileBrushTab()
		{
			tileBrushContainer.style.display = DisplayStyle.Flex;
			RefreshTileBrushContainer();
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnExitTileBrushTab()
		{
			tileBrushContainer.style.display = DisplayStyle.None;
		}
		
		///-//////////////////////////////////////////////////////////////////
		///
		private void OnEnterBiomeAssetsTab()
		{
			biomeAssetsContainer.style.display = DisplayStyle.Flex;
			RefreshBiomeAssetsContainer();
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnExitBiomeAssetsTab()
		{
			biomeAssetsContainer.style.display = DisplayStyle.None;
		}

#endregion // Tab State Machine

#region OnSceneTileBrush

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnSceneTileBrush(SceneView sceneView)
		{
			if (toolMode != ToolMode.TileBrush)
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

					UpdateTileBrushPreviewPosition(evt);
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
							ReplaceTile(evt, hitInfo.transform.gameObject);
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
		private void CreateTileBrushPreview()
		{
			selectedTileBrushPreview = (GameObject)PrefabUtility.InstantiatePrefab(selectedTileBrush);
			selectedTileBrushPreview.transform.SetParent(tileMap.transform);
			selectedTileBrushPreview.SetLayerInAllChildren(2);
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void DestroyTileBrushPreview()
		{
			if (selectedTileBrush != null)
			{
				DestroyImmediate(selectedTileBrushPreview);
				selectedTileBrush = null;
			}
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void ReplaceTile(Event evt, GameObject tileHit)
		{
			// Instantiate tile
			Transform tile = ((GameObject)PrefabUtility.InstantiatePrefab(selectedTileBrush)).transform;
			tile.parent = tileMap.tilesContainer;
			tile.position = selectedTileBrushPreview.transform.position;
			
			// Destroy tile hit
			DestroyImmediate(tileHit);

			//records the object so that it can be undone and sets it as dirty (so that unity saves it)
			Undo.RegisterCreatedObjectUndo(tile.gameObject, "DTE: Replaced tile");
			EditorUtility.SetDirty(tile.gameObject);

			evt.Use();
		}
		
		///-//////////////////////////////////////////////////////////////////
		///
		private void UpdateTileBrushPreviewPosition(Event evt)
		{
			if (selectedTileBrushPreview == null)
			{
				return;
			}
			
			// Raycast from mouse to world
			Ray ray = HandleUtility.GUIPointToWorldRay(evt.mousePosition);
			bool hit = Physics.Raycast(ray, out RaycastHit hitInfo);
				
			if (hit) // if the prefab was placed on another GameObject
			{
				selectedTileBrushPreview.SetActive(true);
				SetHiddenTile(hitInfo.transform.gameObject);
				
				selectedTileBrushPreview.transform.position = hitInfo.transform.position;
			}
			else
			{
				selectedTileBrushPreview.SetActive(false);
				SetHiddenTile(null);
			}
		}

		///-//////////////////////////////////////////////////////////////////
		///
		private void SetHiddenTile(GameObject tile)
		{
			if (hiddenTile == tile || tile == selectedTileBrushPreview)
			{
				return;
			}

			GameObject prevTile = hiddenTile;
			hiddenTile = tile;
			
			// Reactivate hidden tile
			if (prevTile != null)
			{
				prevTile.SetActive(true);
			}

			// Deactivate new hidden tile
			if (hiddenTile != null)
			{
				hiddenTile.SetActive(false);
				
				// Set preview layer to default
				selectedTileBrushPreview.SetLayerInAllChildren(0);
			}
			else
			{
				// If tile is null, set preview layer to ignore raycast
				selectedTileBrushPreview.SetLayerInAllChildren(2);
			}
		}
		
#endregion // OnSceneTileBrush

#region OnSceneBiomeAsset

		///-//////////////////////////////////////////////////////////////////
		///
		private void OnSceneBiomeAsset(SceneView sceneView)
		{
			if (toolMode != ToolMode.BiomeAsset)
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

					UpdateBiomeAssetPreviewPosition(evt);
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
							PlaceBiomeAsset(evt);
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
		private void CreateBiomeAssetPreview()
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
		private void DestroyBiomeAssetPreview()
		{
			if (selectedAssetPreview != null)
			{
				DestroyImmediate(selectedAssetPreview);
				selectedAssetPreview = null;
			}
		}
		
		///-//////////////////////////////////////////////////////////////////
		///
		private void PlaceBiomeAsset(Event evt)
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
		private void UpdateBiomeAssetPreviewPosition(Event evt)
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