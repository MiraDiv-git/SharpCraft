![game_logo](Templates/game_logo.png)

Welcome to the SharpCraft - open-source voxel 3D engine built on C#.

# Documentation
You can access official documentation by **this link**:
https://sharpcraft-docs.netlify.app/

# Solution layout

- **`SharpCraft.Engine`** — the platform: rendering (OpenGL via Silk.NET),
  UI, input, audio (MiniAudio), assets, localization, scenes and the voxel
  world primitives (chunks, mesh builder, world renderer).
- **`SharpCraft`** — the demo game app. Shows the minimal scene API usage.
- **`SharpCraft.Editor`** — the editor (world view, free camera, HUD). Start
  of a full editor to come.

Resources (fonts, localization, textures) live in `SharpCraft.Engine/Resources`
and are packed into `Resources.scres` automatically on every build/publish by
`eng/Resources.targets`. Shaders are embedded into the engine assembly.

# Building the game

    ./build.sh <os> [config]

Example:

    ./build.sh linux-x64 Release

Supported OS targets: `win-x64`, `linux-x64`, `osx-x64`.
The published binary lands in `SharpCraft/bin/<config>/<os>/`.

# Building the editor

    dotnet build SharpCraft.slnx            # build everything
    dotnet run   --project SharpCraft.Editor

Or publish it like the game:

    dotnet publish SharpCraft.Editor -c Release -r linux-x64 --self-contained -o out

# Editor controls

- **WASD** — move the camera
- **Space / Ctrl** — up / down
- **Shift** — speed up
- **Hold RMB** — look around (Esc releases the mouse)
- **F1** — toggle the HUD