using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class MeshCombine
{
    [MenuItem("EditorTools/Mesh/CombineMesh")]
    public static void CombineMesh()
    {
        var obj = Selection.activeGameObject;
        if (obj == null)
        {
            return;
        }

        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        Vector3 centerPos = GetCenter(meshFilters);
        List<Material> materials = new List<Material>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        foreach (var child in meshFilters)
        {
            if (child == null || child.sharedMesh == null)
            {
                continue;
            }

            Matrix4x4 matrix4X4 = child.transform.localToWorldMatrix;
            matrix4X4.m03 -= centerPos.x;
            matrix4X4.m13 -= centerPos.y;
            matrix4X4.m23 -= centerPos.z;

            combineInstances.Add(new CombineInstance()
            {
                mesh = child.sharedMesh,
                transform = matrix4X4
            });

            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            foreach (var childmat in renderer.sharedMaterials)
            {
                if (!materials.Contains(childmat))
                {
                    materials.Add(childmat);
                }
            }
            //materials.AddRange(renderer.sharedMaterials);
        }

        GameObject combine = new GameObject("Combine Mesh");
        combine.transform.position = centerPos;
        MeshFilter meshFilter = combine.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = new Mesh { name = $"Combine Mesh {Guid.NewGuid()}", indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        meshFilter.sharedMesh.CombineMeshes(combineInstances.ToArray(), true);
        MeshRenderer meshRenderer = combine.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterials = materials.ToArray();

        AssetDatabase.CreateAsset(meshFilter.sharedMesh, $"Assets/Temp/CombineMesh/{meshFilter.sharedMesh.name}.asset");

        Debug.Log("Combine Completed");
    }

    private static Vector3 GetCenter(Component[] components)
    {
        if (components != null && components.Length > 0)
        {
            Vector3 min = components[0].transform.position;
            Vector3 max = min;
            foreach (var comp in components)
            {
                min = Vector3.Min(min, comp.transform.position);
                max = Vector3.Max(max, comp.transform.position);
            }

            return min + ((max - min) / 2);
        }

        return Vector3.zero;
    }
}
