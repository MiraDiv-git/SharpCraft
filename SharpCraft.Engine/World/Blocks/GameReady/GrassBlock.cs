using SharpCraft.Engine.Assets;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.World.Blocks.GameReady;

public class GrassBlock : Block
{
    public GrassBlock(GL gl, Shader shader)
        : base(gl, shader, 
            AssetManager.LoadTexture("Textures/Blocks/grass.png", flipVertically: false), 
            BlockMeshes.Cube) { }
}