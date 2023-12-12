# Leonardo's Theatre Scripts
The scripts in this repository unify meshes and replace GameObjects with others, preserving their Transform and position.

# Non-GUI Scripts

## Script 1: Combine Selected Meshes

### Description
This script combines all child meshes of a selected GameObject into a single mesh, replaces the parent objects selected in the editor with a new combined object, and ensures the transform is preserved. The new mesh is saved in the "Assets/CombinedMeshes" directory.

### Prerequisite
- Selected GameObjects and all their children must only have one type of material.
### Usage
1. Copy and paste the script into a new C# file in your Unity project. Save it script and let Unity compile it.
2. Select the parent object in the Unity Editor's hierarchy view.
3. Navigate to the menu bar and click `Custom/Combine Selected Meshes`.
4. The script will create a new object with the combined mesh and replace the parent object.

## Script 2: Combine Meshes using regexes

### Description
This script searches for GameObjects in the scene hierarchy with names matching a specified regex pattern and combines their children's meshes into a single mesh, replaces the parent object with a new combined object, and ensures the transform is preserved. The new mesh is saved in the `Assets/CombinedMeshes` directory.
- In this implementation, it looks for all objects called _"barandilla"_ and _"reja"_.

### Prerequisite
- This specific script only looks for objects starting with the name _"barandilla"_ and _"reja"_.
- Objects that this script looks for must have only one type of material.
- Any combined mesh already present in the CombinedMeshes folder shouldn't.

### Usage
1. Copy and paste the script into a new C# file in your Unity project. Save it and let Unity compile it.
2. Click on the menu item `Custom/Combine Children Meshes by Regex` in the Unity Editor.
3. The script will search the entire scene for GameObjects matching the regex pattern and combine their children's meshes accordingly.

## Script 3: Replace GameObjects with regexes

### Description
The `ReplaceSillas.cs` script searches the entire Unity scene for GameObjects with names that match the regex pattern for _'sillas'_. It replaces each matching GameObject with the `Silla.skp` model, preserving the original GameObject's transform (position, rotation, and scale) and parent-child hierarchy.

### Prerequisites
- The script assumes that a model named `Silla.skp` is inside the `Assets/Scripts/Custom_Optimization_Scripts/sillas_mesh_unica` directory.

### Usage
1. Copy and paste the script into a new C# file in your Unity project. Save it and let Unity compile it.
2. Click on the menu item `Custom/Replace Sillas` in the Unity Editor.
3. The script will search the entire scene for GameObjects matching the regex pattern and combine their children's meshes accordingly.

## Script 4: LODify.cs
### Description
The `LODify.cs` script is a utility for automatically attaching `LODGeneratorHelper` components to GameObjects and generating (LOD) groups. It targets GameObjects within the specific layer called `LODify`. This script is useful for automating the LOD generation process in large scenes.

### Prerequisites
- `LODGeneratorHelper` component and `LODGenerator` logic must be available in the project.
- Target GameObjects should be properly set up and named or layered accordingly in the Unity project.

### Usage
1. Copy and paste the script into a new C# file within the `Assets/Scripts/Custom_Optimization_Scripts` directory of your Unity project. Save the script and let Unity compile it.
2. Run the script by navigating to `Custom/Attach LODGeneratorHelper and Run GenerateLODs` in the Unity Editor.

### Notes
- The script will automatically find and process the target GameObjects based on their names or layer.
- If the target GameObjects already contain LOD groups or specific LOD parent objects, they will be skipped to avoid duplication.
- The script logs informative messages to the Unity console for actions taken or skipped.

# GUI Scripts

## Script 1: Replace GameObjects with Selected Models

### Description
The `Replace_GUI.cs` script provides a GUI in the Unity Editor to replace GameObjects either based on specific regex patterns or direct selection with selected prefab models. Users can add multiple replacements, either by selecting GameObjects directly or by specifying regex-pattern-model pairs, allowing for varied and complex replacement criteria. The changes are made immediately in the scene, and the original GameObject's transform values are preserved.

### Prerequisites
- Models intended for replacement must be available in the Unity project's `Assets/Scripts/Custom_Optimization_Scripts/sillas_mesh_unica/` folder. In there, there are two subfolders:
- `/globe`
   - `/globe/smaller` (for the smaller chair model)
   - `/globe/taller` (for the taller chair model)
- `/olivera` (for all the chair models)

### Usage
1. Copy and paste the script into a new C# file within the `Assets/Scripts/Custom_Optimization_Scripts` directory in your Unity project. Save the script and let Unity compile it.
2. Open the custom GUI window by navigating to `Window/Replace GameObjects` in the Unity Editor.
3. Use the GUI to:
   - Add new direct object-model pairs for replacement.
   - Add new regex-pattern-model pairs for replacement.
   - Select models for each replacement method from the project assets.
   - Remove replacements with the "X" button.
   - Execute the replacements by clicking the "Execute" button.
   - Reset the GUI fields to their default state by clicking the "Reset" button.

### Notes
- The "Execute" button will start the replacement process, iterating through all GameObjects in the current scene and replacing those that match either the direct selection or the regex patterns.
- The "Reset" button will clear all input fields and selections, allowing the user to start fresh.

## Script 2: Combine_GUI.cs

### Description
The `Combine_GUI.cs` script provides a GUI in the Unity Editor for combining meshes of GameObjects. Users can combine GameObjects either directly or based on regex patterns.

### Prerequisites
- GameObjects to be combined and all their children must only have one type of material.

### Usage
1. Copy and paste the script into a new C# file within the `Assets/Scripts/Custom_Optimization/Scripts/` directory of your Unity project. Save the script and let Unity compile it.
2. Open the custom GUI window by navigating to `Window/Combine Meshes` in the Unity Editor.
3. Use the GUI to:
   - Add GameObjects or regex patterns for mesh combination.
   - Remove entries with the "X" button.
   - Execute the combination process by clicking the "Combine" button.
   - Reset the GUI fields to their default state by clicking the "Reset" button.

### Notes
- The "Combine" button will start the process of mesh combination, affecting either the specified GameObjects or those that match the regex patterns.
- The "Reset" button clears all selections and inputs, allowing for a fresh start.

### Useful regular expressions
**For "Globe" Theater**:
- Tall chair models
   - `^Chair#2\s*[\d\w#]*$`
- Small chair models
   - `^Chair#1\s*[\d\w#]*$`

**For Olivera Theatre (all chair models)**
- `^Silla2\s*[\d\w#]*$`
