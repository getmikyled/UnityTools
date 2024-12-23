using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamscape.TileEditor
{
	///-/////////////////////////////////////////////////////////////////////////
	///
	[CreateAssetMenu(fileName="BiomeManager", menuName="Dreamscapes/BiomeManager")]
	public class SODTEBiomeManager : ScriptableObject
	{
		public Vector2 tileSize;
		public int tileCountMultiplier = 1;
		public SODTEBiome[] biomes;
	}

}