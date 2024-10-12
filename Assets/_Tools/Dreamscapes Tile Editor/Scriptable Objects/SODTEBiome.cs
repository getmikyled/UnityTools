using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamscapes.TileEditor
{
	///-/////////////////////////////////////////////////////////////////////////
	///
	[CreateAssetMenu(fileName="Biome", menuName="Dreamscapes/Biome")]
	public class SODTEBiome : ScriptableObject
	{
		public String prefabsFolderPath;
		public Material previewHighlightShader;
		public GameObject[] tiles;
	}
}
