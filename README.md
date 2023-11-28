# Leonardo's Theatre Scripts
The scripts in this repository unify meshes and replace GameObjects with others, preserving their Transform and position.

### Prerequisites for running the scripts
- The Theatre model `Olivera2.2` must be unpacked in Unity.

## Script 1: Combine Selected Meshes

### Description
This script combines all child meshes of a selected GameObject into a single mesh, replaces the parent objects selected in the editor with a new combined object, and ensures the transform is preserved. The new mesh is saved in the "Assets/CombinedMeshes" directory.

### Prerequisite
- Selected GameObjects must all their children with only one type of material.
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
- The script assumes that a model named `Silla.skp` is inside the `Assets/Models/` directory.

### Usage
1. Copy and paste the script into a new C# file in your Unity project. Save it and let Unity compile it.
2. Click on the menu item `Custom/Replace Sillas` in the Unity Editor.
3. The script will search the entire scene for GameObjects matching the regex pattern and combine their children's meshes accordingly.