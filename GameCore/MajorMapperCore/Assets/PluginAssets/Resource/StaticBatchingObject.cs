using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Copy meshes from children into the parent's Mesh.
// CombineInstance stores the list of meshes.  These are combined
// and assigned to the attached Mesh.

public class StaticBatchingObject
{
    public static void StaticBatchingScene(Transform root)
    {

        var rootTransform = root;

        //// all mesh
        var meshFilters = rootTransform.GetComponentsInChildren<MeshFilter>();

        var listMaterialForMesh = new Dictionary<Material, List<GameObject>>();

        //// split each mesh by material
        foreach (var meshFilter in meshFilters)
        {

            var objRenderer = meshFilter.GetComponent<Renderer>();
            if (objRenderer == null)
            {
                continue;
            }

            if (objRenderer.sharedMaterial == null)
            {
                continue;
            }

            if (!listMaterialForMesh.ContainsKey(objRenderer.sharedMaterial))
            {
                var listCombine = new List<GameObject>();
                listMaterialForMesh.Add(objRenderer.sharedMaterial, listCombine);
            }

            listMaterialForMesh[objRenderer.sharedMaterial].Add(meshFilter.gameObject);
        }

        //// static batching
        foreach (var combine in listMaterialForMesh)
        {
            //Debug.Log("combine " + combine.Value.Count);
            StaticBatchingUtility.Combine(combine.Value.ToArray(), rootTransform.gameObject);
        }
    }

}