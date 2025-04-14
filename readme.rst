Avatar Customization System
============================

Setup
-----

clone repository, open unity and import.
Game and CustomEditor should work out of the box, no extra step.

I did took the liberty to rename component from the original repository, so please check section below.


Design Process
---------------

1. Artist Empowerment Through Tooling
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

- **File-Based Workflow**:
  - Uses filenames as data source (``[Category]_PartName`` convention). Example: ``Hair_Punk``, ``Top_LeatherJacket``
  - Enables artist-driven content updates without code changes or tool development
  - If creating tool, extend what artist already use, unless we're also making tool for UGC

- **Validation Infrastructure**:
  - Automatic checks for naming conventions and bone compatibility
  - Prevents broken assets from entering production pipeline


2. Modularity as First Principle
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

- **Granular Asset Separation**:
  - Meshes, Animation, materials, and textures stored as independent assets
  - Enables combinatorial customization (10 tops × 5 materials = 50 variants)

- **Lego-Like Reusability**:
  .. code-block:: text

    Same mesh → Multiple contexts:
    - Cloud ↔ Bush (Super Mario paradigm)


3. Performance Throughs
^^^^^^^^^^^^^^^^^^^^^^^

  - Shared base materials with property toggling
  - Texture Atlasing, save memory, allow batching
  - Common Rig/skinned System, mesh could be combined
  - Mesh Reuse, Enables Gpu instancing, compute skinning
  - Assets streaming: Prioritize order of loading: Player first, then his party, then by distance.
  - Transition avatar: Highly optimized mesh (low footprint, ex: blurry ghost), useful sudden surge of adding or updating lots` of player

4. Animation Scalability
^^^^^^^^^^^^^^^^^^^^^^^^

  - All characters share core bone structure, Enables:
    - Motion retargeting, animation across body types
    - Crowd animation optimizations, batch processing
    - Custom asset interoperability

  - ECS would be a great candidate




Key Components
--------------

AvatarAssembler (in Editor Tool)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


.. image:: https://github.com/Incubatio/avatar-system-tech-test/blob/main/ScreenShots/AvatarAssembler_EditorTool.jpg

``Code/Tools/AvatarAssembler.cs``:

  - CustomEditor to preview animated avatar and switch parts

  - Work in combination with ``ScriptableObject AvatarSet``

  - Top and Bottom Part Re-use the rig of the base (copying bones via ``SkinnedMeshHelper.ReBone()``)

  - Top and Bottom Part also re-use Material. ``//TODO: very superficial implementation there, could go more granular with shaders, textures and various properties ...``
    Could also share instance of material between different avatar


Avatar Config Generator
~~~~~~~~~~~~~~~~~~~~~~~

.. image:: https://github.com/Incubatio/avatar-system-tech-test/blob/main/ScreenShots/EntryPoint_and_GenerateConfig.jpg

``Code/Tools/AvatarConfigGenerator.cs``:

  - Browse project database to build array of Avatar Parts (Bases, Hair, Top, Bottom, Accessory, Materials)

  - New part types are easy to add: Create ``[Category]_PartName`` and Regenerate ``AvatarSystemConfig`` via menu ``Avatar \ Generate Config`` in Editor


Main MonoBehavior
~~~~~~~~~~~~~~~~~

``MainMono.cs`` (Single Entry Point of "the game"):

  - Set the number spawning characters

  - Use a UIControler to swap Parts "in Game" via UI, it use the scene as database. ``//TODO: keep the scene as source of truth, but use AvatarSet as middleware``

  - Has a Mirror to challenge performance, but also look into self and reflect.


Project Structure
-----------------

::

    Assets/
    ├── Art/
    │   ├── Animations
    │   ├── Materials
    │   ├── Prefabs         # base prefabs, Modular avatar parts
    │   └── RawImports      # imported fbx, skinned avatar parts
    │
    ├── Code/
    │   ├── Components      # Data-objects, ex: Data only monobehavior
    │   ├── Configs         # ReadOnly data, ex: ScriptableObject
    │   ├── Controllers     # Second level flow state objects
    │   ├── Helpers         # Stateless utilities
    │   ├── Tools           # executables, ex: Menu Scripts, CustomEditor
    │   ├── UI              # UIToolkit documents
    │   └── MainMono.cs     # First level flow state object
    │
    ├── Design/             # Contain scripts and tools for game designer, level designer, content creators
    │   └── AvatarSets      # Open or create AvatarSets and play with avatar in Editor
    │
    └── PlayGame.scene      # Entry point for the game


Rendering Performance:
----------------------

Here are the rendering optimisation I considered.

+---------------------+----------------------+-----------------------------------+
| Technique           | Viability            | Reason                            |
+=====================+======================+===================================+
| Static Batching     | [x] Not applicable   | All meshes are skinned/animated   |
+---------------------+----------------------+-----------------------------------+
| Mesh Combining      | [x] Over Unity's 65k | Current avatar: 200k+ vertices    |
|                     | limit                |                                   |
+---------------------+----------------------+-----------------------------------+
| GPU Instancing      | [!] Custom required  | Built-in doesn't support skinning |
+---------------------+----------------------+-----------------------------------+
| Compute Shader      | [!] Support limited  | Need poweful GPU. Complex Impl.   |
|                     |   on mobile          |                                   |
+---------------------+----------------------+-----------------------------------+

Considering my limited time on the project, I just enabled Unity 6 GPU instancing which does reduce drawcalls, 
although doesn't seem to affect frametime. Maybe it Maybe it would have a higher impact on Mobile.



Potential Improvements
----------------------

1. **Texture Atlasing**

2. **DOTS**, improved CPU usage for vast amount of object and animations

3. **Mesh Combine**, creating a new SkinnedMeshRenderer to combine the meshes and use the same bones

4. **LOD System**

5. **Asset Streaming**
