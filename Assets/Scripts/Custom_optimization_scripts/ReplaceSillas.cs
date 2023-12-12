using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Include the SceneManagement namespace

public class ReplaceSillas : EditorWindow
{
    [MenuItem("Custom/Replace Sillas")]
    public static void ReplaceSillasWithOptimizedModel()
    {
        // Determine the current scene
        string currentSceneName = SceneManager.GetActiveScene().name.ToUpper();

        // Load the optimized models for the GLOBE scene
        GameObject optimizedSillaSmaller = null;
        GameObject optimizedSillaTaller = null;
        if (currentSceneName == "GLOBE")
        {
            optimizedSillaSmaller = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scripts/Custom-LOD_Meshes_Combine/sillas_mesh_unica/globe/smaller/silla_mesh_unica_smaller.skp");
            optimizedSillaTaller = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scripts/Custom-LOD_Meshes_Combine/sillas_mesh_unica/globe/taller/silla_mesh_unica_taller.skp");

            // Ensure that the optimized models for the GLOBE scene were found
            if (optimizedSillaSmaller == null || optimizedSillaTaller == null)
            {
                Debug.LogError("One or more optimized Silla models not found for the GLOBE scene.");
                return;
            }

        }

        // Load the optimized model for the OLIVERA scene
        GameObject optimizedSillaOlivera = null;
        if (currentSceneName == "OLIVERA")
        {
            optimizedSillaOlivera = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scripts/Custom-LOD_Meshes_Combine/sillas_mesh_unica/olivera/Silla.skp");

            // Ensure that the optimized model for the OLIVERA scene was found
            if (optimizedSillaOlivera == null)
            {
                Debug.LogError("Optimized Silla model not found for the OLIVERA scene.");
                return;
            }
        }

        //Create a list to contain all chair objects
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // Regex patterns for each scene
        string patternGlobeSmaller = @"^Chair#1\s*[\d\w#]*$";
        string patternGlobeTaller = @"^Chair#2\s*[\d\w#]*$";
        string patternOlivera = @"^Silla2\s*[\d\w#]*$";

        // Replace the objects based on the scene and naming pattern
        foreach (GameObject obj in allObjects)
        {
            // Check if the object has already been destroyed and skip it if so
            if (obj == null) continue;

            GameObject optimizedSilla = null;
            string objName = obj.name; // Cache the name before destroying the object

            if (currentSceneName == "GLOBE" && Regex.IsMatch(objName, patternGlobeSmaller))
            {
                optimizedSilla = optimizedSillaSmaller;
            }
            else if (currentSceneName == "GLOBE" && Regex.IsMatch(objName, patternGlobeTaller))
            {
                optimizedSilla = optimizedSillaTaller;
            }
            else if (currentSceneName == "OLIVERA" && Regex.IsMatch(objName, patternOlivera))
            {
                optimizedSilla = optimizedSillaOlivera;
            }

            // If a matching optimized silla was found, replace the current object
            if (optimizedSilla != null)
            {
                // Save the object's transform values
                Vector3 position = obj.transform.position;
                Quaternion rotation = obj.transform.rotation;
                Vector3 scale = obj.transform.localScale;
                string name = objName; // Use the cached name
                Transform parent = obj.transform.parent;

                // Destroy the original silla
                DestroyImmediate(obj);

                // Instantiate the new silla with the saved transform values
                GameObject newSilla = Instantiate(optimizedSilla, position, rotation, parent);
                newSilla.transform.localScale = scale;
                newSilla.name = name;
            }
        }
    }
}