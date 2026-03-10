using SharpCraft.Engine.Assets;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.World.Blocks.GameReady;

public class DirtBlock : Block
{
    public DirtBlock(GL gl, Shader shader)
        : base(gl, shader, 
            AssetManager.LoadTexture("Textures/Blocks/dirt.png", flipVertically: false), 
            BlockMeshes.Cube) { }
}