using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Combine_GUI : EditorWindow
{
    // Lists to store GameObjects for direct selection and regex patterns for combining
    private List<GameObject> objectsToCombine = new List<GameObject>();
    private Vector2 scrollPosition;
    private List<string> regexPatterns = new List<string>();
    private string combinedMeshesFolder = "Assets/CombinedMeshes";

    // The menu item to open this custom editor window
    [MenuItem("Window/Combine Meshes")]
    public static void ShowWindow()
    {
        // Opens the window, titled "Combine Meshes"
        GetWindow<Combine_GUI>("Combine Meshes");
    }

    // Main function to draw the GUI for the editor window
    void OnGUI()
    {
        // Window's title
        GUILayout.Label("Combine GameObjects and GameObjects by Regex", EditorStyles.boldLabel);
        
        // Begins a scroll view section to contain the GUI elements
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Section for direct GameObject combination in the GUI
        GUILayout.Label("Direct GameObjects", EditorStyles.boldLabel);
        for (int i = 0; i < objectsToCombine.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // Section for direct GameObject combination in the GUI
            objectsToCombine[i] = (GameObject)EditorGUILayout.ObjectField("Object to Combine", objectsToCombine[i], typeof(GameObject), true);

            // Button "X" to remove an entry from the list
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                objectsToCombine.RemoveAt(i);
                GUILayout.EndHorizontal();
                Repaint();
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        // Section for regex-based GameObject combination in the GUI
        GUILayout.Label("Regex Patterns", EditorStyles.boldLabel);
        for (int i = 0; i < regexPatterns.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            regexPatterns[i] = EditorGUILayout.TextField("Regex Pattern", regexPatterns[i]);

            // Button "X" to remove an entry from the list
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                regexPatterns.RemoveAt(i);
                GUILayout.EndHorizontal();
                Repaint();
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        // Ends the scroll view section
        EditorGUILayout.EndScrollView();

        // Buttons for adding new objects/regex, executing combination, and reset
        if (GUILayout.Button("Add New Object"))
        {
            objectsToCombine.Add(null);
        }

        if (GUILayout.Button("Add New Regex"))
        {
            regexPatterns.Add("");
        }
        
        // Combine meshes for direct selection and regex patterns
        if (GUILayout.Button("Combine"))
        {
            foreach (var selectedObject in objectsToCombine)
            {
                if (selectedObject != null)
                {
                    CombineMeshesForGameObject(selectedObject);
                }
            }

            foreach (string pattern in regexPatterns)
            {
                // Check and notify if the regex pattern is null or empty
                if (!string.IsNullOrEmpty(pattern))
                {
                    CombineChildrenMeshesByRegex(pattern);
                }
                else
                {
                    Debug.Log("Regex cannot be empty");
                }
            }

            // Remove null entries from the list (after combining them they're removed)
            objectsToCombine.RemoveAll(item => item == null);
            Repaint();
        }

        if (GUILayout.Button("Reset"))
        {
            // Clears all lists resetting the state of the script
            objectsToCombine.Clear();
            regexPatterns.Clear();
        }
    }

    // Combines meshes of a single GameObject
    private void CombineMeshesForGameObject(GameObject selectedObject)
    {
        // Ensure the object is not part of a prefab before destroying
        if (PrefabUtility.IsPartOfPrefabInstance(selectedObject))
        {
            Debug.LogError("GameObject is part of a prefab, cannot destroy: " + selectedObject.name);
            return;
        }

        if (selectedObject == null)
        {
            Debug.LogError("No object selected.");
            return;
        }

        // Initialize a new mesh that will be used to combine all child meshes of the selected object.
        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Set to 32-bit index format

        // Calculate the transformation matrix to convert from world space to the selected object's local space.
        Matrix4x4 parentMatrix = selectedObject.transform.worldToLocalMatrix;

        // Prepare a list to hold all the mesh data to be combined.
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

        // Iterate over each mesh filter, creating a CombineInstance for its mesh. Add each of the mesh filter's to the list of meshes to be combined.
        foreach (var meshFilter in meshFilters)
        {
            if (meshFilter.sharedMesh == null) continue;

            CombineInstance ci = new CombineInstance
            {
                mesh = meshFilter.sharedMesh,
                transform = parentMatrix * meshFilter.transform.localToWorldMatrix
            };
            combine.Add(ci);
        }

        // Combine all meshes in the list into the single mesh initialized earlier
        combinedMesh.CombineMeshes(combine.ToArray(), true, true);

        // Create a new GameObject that will hold the combined mesh.
        GameObject combinedObject = new GameObject(selectedObject.name + "_Combined");
        // Set the new GameObject's parent to the original selected object's parent, preserving the hierarchical structure
        combinedObject.transform.SetParent(selectedObject.transform.parent, false);
        // Set the combined object's position, rotation, and scale to match that of the original selected object.
        combinedObject.transform.SetPositionAndRotation(selectedObject.transform.position, selectedObject.transform.rotation);
        combinedObject.transform.localScale = selectedObject.transform.localScale;

        // Add a MeshFilter component to the new GameObject and assign the combined mesh to it.
        MeshFilter meshFilterNew = combinedObject.AddComponent<MeshFilter>();
        meshFilterNew.mesh = combinedMesh;

        // Add a MeshRenderer component to the new GameObject
        MeshRenderer meshRenderer = combinedObject.AddComponent<MeshRenderer>();

        // Assign the found material to the new GameObject's renderer
        if (sharedMaterial != null)
        {
            meshRenderer.sharedMaterial = sharedMaterial;
        }

        // Place the CombinedMesh inside the folder "CombinedMeshes"
        EnsureFolderExists(combinedMeshesFolder);
        AssetDatabase.CreateAsset(combinedMesh, combinedMeshesFolder + "/" + combinedObject.name + ".asset");
        AssetDatabase.SaveAssets();

        // Adjust the order of the new combined object to preserve the hierarchy in the scene
        combinedObject.transform.SetSiblingIndex(selectedObject.transform.GetSiblingIndex());

        // Handle undo and prefab operations
        Undo.RecordObject(selectedObject, "Combine Meshes");
        Undo.DestroyObjectImmediate(selectedObject);
        Undo.RegisterCreatedObjectUndo(combinedObject, "Combine Meshes");
    }

    // Combines children meshes of GameObjects matching a regex pattern
    private void CombineChildrenMeshesByRegex(string regexPattern)
    {
        // Skip processing if the regex pattern is null or empty
        if (string.IsNullOrEmpty(regexPattern))
        {
            Debug.Log("Regex pattern cannot be empty");
            return;
        }

        Regex regex = new Regex(regexPattern);
        List<GameObject> matchedObjects = new List<GameObject>();

        // Find all GameObjects matching the regex pattern
        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
        {
            if (regex.IsMatch(obj.name))
            {
                matchedObjects.Add(obj);
            }
        }

        // Combine meshes of matched GameObjects
        foreach (GameObject matchedObj in matchedObjects)
        {
            CombineMeshesForGameObject(matchedObj);
        }
    }


    // This method checks if the specified path represents a valid folder, otherwise it creates the folder.
    private static void EnsureFolderExists(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parentFolder = System.IO.Path.GetDirectoryName(path);
            string newFolderName = System.IO.Path.GetFileName(path);
            AssetDatabase.CreateFolder(parentFolder, newFolderName);
        }
    }
}
