using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

namespace UnityMeshSimplifier
{
    public static class LODifyScript
    {
        // Adds a menu item to the Unity Editor
        [MenuItem("Custom/Attach LODGeneratorHelper and Run GenerateLODs")]
        private static void AttachLODGeneratorHelperAndRunGenerateLODs()
        {
            // Define the layer used onto which execute this script
            int layerToLODify = LayerMask.NameToLayer("LODify");

            // Find specific parent game objects in the scene by name
            GameObject olivera = GameObject.Find("Olivera2.2 (1)");
            GameObject globe = GameObject.Find("sakespeares_globe");

            // Check if either parent game object is found in the scene; if not, log an error and exit the function
            if (olivera == null && globe == null)
            {
                Debug.LogError("Neither 'Olivera2.2 (1)' nor 'sakespeares_globe' could be found in the scene.");
                return;
            }

            // Retrieve all game objects within the specified layer
            var lodifyObjects = FindObjectsInLayer(layerToLODify);

            // Filter the objects to include only those that are children of either 'olivera' or 'globe'
            lodifyObjects = lodifyObjects.Where(go => IsChildOf(go.transform, olivera?.transform) || IsChildOf(go.transform, globe?.transform)).ToArray();

            // Iterate over each object in the filtered list
            foreach (var lodifyObject in lodifyObjects)
            {
                // Skip objects that already have LODs or an LODGroup
                var existingLodParent = lodifyObject.transform.Find(LODGenerator.LODParentGameObjectName);
                if (existingLodParent != null)
                {
                    Debug.LogWarning($"Skipping '{lodifyObject.name}' because it already has LODs under '{LODGenerator.LODParentGameObjectName}'.", lodifyObject);
                    continue;
                }

                var existingLodGroup = lodifyObject.GetComponent<LODGroup>();
                if (existingLodGroup != null)
                {
                    Debug.LogWarning($"Skipping '{lodifyObject.name}' because it already has a LOD Group.", lodifyObject);
                    continue;
                }

                // Add LODGeneratorHelper to the object if it doesn't already have one and generate LODs
                var lodGeneratorHelper = lodifyObject.GetComponent<LODGeneratorHelper>() ?? lodifyObject.AddComponent<LODGeneratorHelper>();
                var lodGroup = LODGenerator.GenerateLODs(lodGeneratorHelper);

                // Check if LODs were generated successfully and set 'isGenerated' to true
                if (lodGroup != null)
                {
                    SerializedObject serializedObject = new SerializedObject(lodGeneratorHelper);
                    SerializedProperty isGeneratedProp = serializedObject.FindProperty("isGenerated");
                    if (isGeneratedProp != null)
                    {
                        isGeneratedProp.boolValue = true;
                        serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        Debug.LogError("The 'isGenerated' property was not found on the LODGeneratorHelper component.");
                    }
                }
            }
        }

        // Method to find all game objects in a specific layer
        private static GameObject[] FindObjectsInLayer(int layer)
        {
            return GameObject.FindObjectsOfType<GameObject>().Where(go => go.layer == layer).ToArray();
        }

        // Method to check if a transform is a child (direct or indirect) of another transform
        private static bool IsChildOf(Transform child, Transform parent)
        {
            if (parent == null) return false;
            while (child != null)
            {
                if (child == parent) return true;
                child = child.parent;
            }
            return false;
        }
    }
}