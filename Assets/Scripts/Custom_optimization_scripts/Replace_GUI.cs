using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Replace_GUI : EditorWindow
{
    // Lists to store GameObjects and their replacements for direct selection
    private List<GameObject> objectsToReplace = new List<GameObject>();
    private List<GameObject> replacementObjectsForDirect = new List<GameObject>();
    
    // Lists to store regex patterns and their replacement GameObjects
    private List<string> objectsToReplaceRegex = new List<string>();
    private List<GameObject> replacementObjectsForRegex = new List<GameObject>();
    
    // Scroll position in the GUI
    private Vector2 scrollPosition;

    // Menu item to open the custom editor window
    [MenuItem("Window/Replace GameObjects")]
    public static void ShowWindow()
    {
        // Opens the window titled "Replace GameObjects"
        GetWindow<Replace_GUI>("Replace GameObjects");
    }

    // Main function to draw the GUI
    void OnGUI()
    {
        // Title label at the top of the window
        GUILayout.Label("Replace GameObjects", EditorStyles.boldLabel);

        // Begins a scroll view section to contain the GUI elements
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Temporary variables to hold the index of items to be removed
        int? itemToRemoveFromDirect = null;
        int? itemToRemoveFromRegex = null;

        // Direct GameObject Replacement Section
        GUILayout.Label("Direct GameObject Replacement", EditorStyles.boldLabel);
        for (int i = 0; i < objectsToReplace.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            // Fields to select GameObjects and their replacements
            objectsToReplace[i] = (GameObject)EditorGUILayout.ObjectField("Object to Replace", objectsToReplace[i], typeof(GameObject), true);
            replacementObjectsForDirect[i] = (GameObject)EditorGUILayout.ObjectField("Replacement Object", replacementObjectsForDirect[i], typeof(GameObject), false);

            // Button to remove an entry from the list
            if (GUILayout.Button("X"))
            {
                itemToRemoveFromDirect = i; // Mark this index for removal
            }
            EditorGUILayout.EndHorizontal();
        }

        // Regex Replacement Section
        GUILayout.Label("Regex-Based Replacement", EditorStyles.boldLabel);
        for (int i = 0; i < objectsToReplaceRegex.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            // Fields to enter regex patterns and select their replacements
            objectsToReplaceRegex[i] = EditorGUILayout.TextField("Regex Pattern", objectsToReplaceRegex[i]);
            replacementObjectsForRegex[i] = (GameObject)EditorGUILayout.ObjectField("Replacement Object", replacementObjectsForRegex[i], typeof(GameObject), false);

            // Button to remove an entry from the list
            if (GUILayout.Button("X"))
            {
                itemToRemoveFromRegex = i; // Mark this index for removal
            }
            EditorGUILayout.EndHorizontal();
        }

        // Ends the scroll view section
        EditorGUILayout.EndScrollView();

        // If an item was marked for removal, remove it from the list
        if (itemToRemoveFromDirect.HasValue)
        {
            objectsToReplace.RemoveAt(itemToRemoveFromDirect.Value);
            replacementObjectsForDirect.RemoveAt(itemToRemoveFromDirect.Value);
        }
        if (itemToRemoveFromRegex.HasValue)
        {
            objectsToReplaceRegex.RemoveAt(itemToRemoveFromRegex.Value);
            replacementObjectsForRegex.RemoveAt(itemToRemoveFromRegex.Value);
        }

        // Buttons for adding new objects/regex, executing replacements, and resetting the lists
        if (GUILayout.Button("Add New Object Replacement"))
        {
            objectsToReplace.Add(null);
            replacementObjectsForDirect.Add(null);
        }

        if (GUILayout.Button("Add New Regex Replacement"))
        {
            objectsToReplaceRegex.Add("");
            replacementObjectsForRegex.Add(null);
        }

        if (GUILayout.Button("Execute"))
        {
            ExecuteReplacement();
        }

        if (GUILayout.Button("Reset"))
        {
            // Clears all lists, resetting the state of the window
            objectsToReplace.Clear();
            replacementObjectsForDirect.Clear();
            objectsToReplaceRegex.Clear();
            replacementObjectsForRegex.Clear();
        }
    }

    // Function to replace GameObjects based on regex patterns
    void ReplaceObjects(string pattern, GameObject replacement)
    {
        List<GameObject> objectsToDestroy = new List<GameObject>();

        // Identifies all GameObjects that match the regex pattern
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (Regex.IsMatch(obj.name, pattern))
            {
                objectsToDestroy.Add(obj);
            }
        }

        // Replaces each identified GameObject
        foreach (GameObject obj in objectsToDestroy)
        {
            ReplaceSingleObject(obj, replacement);
        }
    }

    // main function to execute replacements, both direct and regex-based
    void ExecuteReplacement()
    {
        // Direct GameObject Replacement
        for (int i = objectsToReplace.Count - 1; i >= 0; i--)
        {
            if (objectsToReplace[i] != null && replacementObjectsForDirect[i] != null)
            {
                ReplaceSingleObject(objectsToReplace[i], replacementObjectsForDirect[i]);
                // After replacement, remove the entry from the list
                objectsToReplace.RemoveAt(i);
                replacementObjectsForDirect.RemoveAt(i);
            }
        }

        // Regex-Based Replacement
        for (int i = 0; i < objectsToReplaceRegex.Count; i++)
        {
            if (!string.IsNullOrEmpty(objectsToReplaceRegex[i]) && replacementObjectsForRegex[i] != null)
            {
                ReplaceObjects(objectsToReplaceRegex[i], replacementObjectsForRegex[i]);
            }
            else if (string.IsNullOrEmpty(objectsToReplaceRegex[i]))
            {
                // Log a message if the regex pattern is empty
                Debug.Log("Regex pattern cannot be empty");
            }
        }
    }

    // Replaces a single GameObject with another, preserving transform values
    void ReplaceSingleObject(GameObject obj, GameObject replacement)
    {
        // Save the object's transform values
        Vector3 position = obj.transform.position;
        Quaternion rotation = obj.transform.rotation;
        Vector3 scale = obj.transform.localScale;
        Transform parent = obj.transform.parent;
        string objName = obj.name; // Cache the name before destroying the object

        // Destroys the original object
        DestroyImmediate(obj);

        // Creates a new object with the saved transform values
        GameObject newObj = Instantiate(replacement, position, rotation, parent);
        newObj.transform.localScale = scale;
        newObj.name = objName; // Use the cached name for the replacement
    }
}