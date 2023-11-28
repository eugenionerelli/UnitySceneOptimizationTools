using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class ReplaceSillas : EditorWindow
{
    [MenuItem("Custom/Replace Sillas")]
    public static void ReplaceSillasWithOptimizedModel()
    {
        //Load the Optimized Asset
        GameObject optimizedSilla = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Silla.skp");

        if (optimizedSilla == null)
        {
            Debug.LogError("Optimized Silla model not found.");
            return;
        }
        
        //Create a list to contain all chair objects
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<GameObject> sillasToReplace = new List<GameObject>();

        // Look for all the sillas to be replaced using a regex
        foreach (GameObject obj in allObjects)
        {
            if (Regex.IsMatch(obj.name, @"^Silla2\s*[\d\w#]*$"))
            {
                sillasToReplace.Add(obj);
            }
        }

        // Replace with the unified model and destroy the identified sillas
        foreach (GameObject obj in sillasToReplace)
        {
            //Save the object's position, rotation and scale to preserve them
            Vector3 position = obj.transform.position;
            Quaternion rotation = obj.transform.rotation;
            Vector3 scale = obj.transform.localScale;
            string name = obj.name;
            Transform parent = obj.transform.parent; // Store the parent object

            DestroyImmediate(obj);
            
            // Place the new silla as a child of the original parent object with the original position, rotation and scale
            GameObject newSilla = Instantiate(optimizedSilla, position, rotation, parent); 
            newSilla.transform.localScale = scale;
            newSilla.name = name;
        }
    }
}
