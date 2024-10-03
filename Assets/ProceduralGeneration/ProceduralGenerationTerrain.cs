using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GetMikyled.ProceduralGenerationTerrain
{
    ///-//////////////////////////////////////////////////////////////////
    ///
    [RequireComponent(typeof(MeshFilter))]
    public class ProceduralGenerationTerrain : MonoBehaviour
    {
        private Mesh mesh;
        private MeshFilter meshFilter;

        private Vector3[] vertices;
        private int[] triangles;

        [Min(0)] [SerializeField] private int xSize = 5;
        [Min(0)] [SerializeField] private int zSize = 10;

        ///-//////////////////////////////////////////////////////////////////
        ///
        private void OnValidate()
        {
            SetMesh();
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        private void SetMesh()
        {
            meshFilter = GetComponent<MeshFilter>();

            meshFilter.sharedMesh.Clear();

            CreateShape();
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        private void CreateShape()
        {
            // Create Vertices
            vertices = new Vector3[((xSize + 1) * (zSize + 1))];
            for (int i = 0, z = 0; z <= zSize; z++)
            {
                for (int x = 0; x<= xSize; x++)
                {
                    vertices[i] = new Vector3(x, 0, z);
                    i++;
                }
            }

            // Create triangles for every row (z)
            triangles = new int[xSize * zSize * 6];
            int vert = 0;
            int tris = 0;
            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + xSize + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + xSize + 1;
                    triangles[tris + 5] = vert + xSize + 2;

                    vert++;
                    tris += 6;
                }
                vert++;
            }

            UpdateMesh();
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        private void UpdateMesh()
        {
            meshFilter.sharedMesh.vertices = vertices;
            meshFilter.sharedMesh.triangles = triangles;
            meshFilter.sharedMesh.RecalculateNormals();
        }

        ///-//////////////////////////////////////////////////////////////////
        ///
        private void OnDrawGizmos()
        {
            if (vertices == null) return;

            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], .1f);
            }
        }

    }

}