using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

public class CombineMeshesRegex : MonoBehaviour
{
    [MenuItem("Custom/Combine Children Meshes by Regex")]
    private static void CombineChildrenMeshesByRegex()
    {
        // Define the regex pattern for GameObject names to search for in the hierarchy.
        string regexPattern = @"^(barandilla metal \d+(#\d+)?(\s+\d+)?)|(reja \d+(\s+\d+)?)$";
        Regex regex = new Regex(regexPattern);
        bool assetCreated = false;

        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
        {
            if (regex.IsMatch(obj.name))
            {
                // If a GameObject's name matches the pattern, combine its child meshes
                CombineMeshesForGameObject(obj);
                assetCreated = true;
            }
        }

        // Refresh the AssetDatabase to ensure that the newly created assets are updated in the Unity Editor's Project view
        if (assetCreated)
        {
            AssetDatabase.Refresh();
        }

    }

    private static void CombineMeshesForGameObject(GameObject selectedObject)
    {
        // Initialize a new mesh that will be used to combine all child meshes of the selected object.
        Mesh combinedMesh = new Mesh();
        
        // Calculate the transformation matrix to convert from world space to the selected object's local space.
        Matrix4x4 parentMatrix = selectedObject.transform.worldToLocalMatrix;

        // Prepare a list to hold all the mesh data to be combined
        List<CombineInstance> combine = new List<CombineInstance>();

        // Retrieve all renderer and mesh filters components from the children of the selected object.
        Renderer[] renderers = selectedObject.GetComponentsInChildren<Renderer>();
        MeshFilter[] meshFilters = selectedObject.GetComponentsInChildren<MeshFilter>();

        // Use the first material from the children to apply to the combined mesh
        Material sharedMaterial = null;
        if (renderers.Length > 0 && renderers[0].sharedMaterials.Length > 0)
        {
            sharedMaterial = renderers[0].sharedMaterials[0];
        }

        // Iterate over each mesh filter, creating a CombineInstance for its mesh. Add each mesh filter's data to the list of meshes to be combined.
        foreach (var meshFilter in meshFilters)
        {
            CombineInstance ci = new CombineInstance();
            ci.mesh = meshFilter.sharedMesh;
            ci.transform = parentMatrix * meshFilter.transform.localToWorldMatrix;
            combine.Add(ci);
        }

        // Combine all meshes in the list into the single mesh initialized earlier
        combinedMesh.CombineMeshes(combine.ToArray(), true, true);

        // Create a new GameObject that will hold the combined mesh.
        GameObject combinedObject = new GameObject(selectedObject.name + "_Combined");

        // Set the new GameObject's parent to the original selected object's parent, preserving the hierarchical structure.
        combinedObject.transform.SetParent(selectedObject.transform.parent, false);

        // Set the combined object's position, rotation, and scale to match that of the original selected object.
        combinedObject.transform.SetPositionAndRotation(selectedObject.transform.position, selectedObject.transform.rotation);
        combinedObject.transform.localScale = selectedObject.transform.localScale;

        // Add a MeshFilter component to the new GameObject and assign the combined mesh to it
        MeshFilter meshFilterNew = combinedObject.AddComponent<MeshFilter>();
        meshFilterNew.mesh = combinedMesh;

        // Add a MeshRenderer component to the new GameObject
        MeshRenderer meshRenderer = combinedObject.AddComponent<MeshRenderer>();

        // Assign the found material to the new GameObject's renderer
        if (sharedMaterial != null)
        {
            meshRenderer.sharedMaterial = sharedMaterial;
        }
        else
        {
            // Fallback check to ensure that the new combined mesh has a material
            Debug.LogError("No material found on the first child! Make sure the children have materials assigned.");
        }

        // Place the CombinedMesh inside the folder "CombinedMeshes"
        EnsureFolderExists("Assets/CombinedMeshes");
        string assetPath = "Assets/CombinedMeshes/" + selectedObject.name + "_Combined.asset";
        AssetDatabase.CreateAsset(combinedMesh, assetPath);
        AssetDatabase.SaveAssets();

        // Adjust the order of the new combined object to preserve the hierarchy in the scene
        combinedObject.transform.SetSiblingIndex(selectedObject.transform.GetSiblingIndex());

        // Delete the original parent object
        DestroyImmediate(selectedObject);
    }

    // This method checks if the specified path represents a valid folder, otherwise it creates the folder
    private static void EnsureFolderExists(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parentFolder = Path.GetDirectoryName(path);
            string newFolderName = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parentFolder, newFolderName);
        }
    }
}
