# Dreamscape Tile Editor Tool

## Problem
Dreamscape is a 3D roguelite that has a map generation system that builds out by tiles. The game requires there to be lots of variation of chunks to be made, but that would require a lot of time from environment artists and level designers that the team doesn't have.

## Solution
In order to automate some of the processes, I created the **Dreamscape Level Editor Tool** (DTE)

## Documentation

### What features does the tool provide?
- Biome Type
  - Change the biome type of the chunk  
- Biome Seed
  - Change the seed of the biome for more tile variation. Requires multiple tile variants.
- Grid Size
  - Change the size of the chunk.
- Tool Settings
  - Grid Snap
    - Adjust the size of the grid snap when placing objects
- Tool Modes
  - Tile Brush
    - The Tile Brush tool allows you to paint specific tile types over the already generated tiles.
  - Biome Assets
    - The Biome Assets tool allows you to select from prefabs that belong to that specific biome and swiftly place them onto the tiles.

![image](https://github.com/user-attachments/assets/b27304e9-7082-4142-9efa-37712a36a688)

### How to get started
To create a new tile map:
Right Click in the Hierarchy -> Navigate to Dreamscapes -> Tilemap

<img width="1280" alt="image" src="https://github.com/user-attachments/assets/11d8c327-42ff-4771-9377-5eed36ad958f">

### Features Deep Dive

#### Biome Type
Change for the biome type of the chunk. The current options are {Dreamscape, Lava}
![image](https://github.com/user-attachments/assets/424a03be-457d-4bef-a127-60e5625e5495)

#### Biome Seed
Change the seed of the biome for more tile variation. Requires multiple tile variants. The range is from 0 to 100.
![image](https://github.com/user-attachments/assets/ea25439d-f54c-495d-a36c-d023833d4201)

#### Grid Size
Change the size of the chunk.
![image](https://github.com/user-attachments/assets/35e57546-9206-439e-a013-cddb1b03370d)

#### Tile Brush Tool
The Tile Brush tool allows you to paint specific tile types over the already generated tiles.

Instructions
- Select the tile you would like to paint with.
- Click on the tile in the scene view that you would like to replace.
- To exit the tool, click on empty space in the scene view (Will be improved).

![image](https://github.com/user-attachments/assets/c9bdda6a-2d4d-4fe1-adce-3fdad8e3e725)

#### Biome Assets Tool
The Biome Assets tool allows you to select from prefabs that belong to that specific biome and swiftly place them onto the tiles.

Instructions
- Select the biome asset you would like to place
- Click on the spot you would like to place the tile. If you want to **snap**, hold the left CTRL key.
- If you would like to rotate the object, hold left click and release when satisfied with the rotation

Current Flaws
- Make sure that the tilemap's position is integer only, otherwise the position snapping will be off. Generally, tilemaps should only be created in prefabs.

![image](https://github.com/user-attachments/assets/5e07a79a-f98f-40b5-bf5f-18fc1a1e5430)

### How to add new Biome Assets, Tile Brush Options, and Tile Variants
In your project folders, navigate to Assets -> Prefabs -> WorldGen -> (THE RESPECTIVE BIOME)

Next, add the prefab to its respective folder.

![image](https://github.com/user-attachments/assets/5da3b6f1-dc13-4e3a-bd5f-3b798260a8de)




