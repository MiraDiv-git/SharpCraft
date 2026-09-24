using SharpCraft.Engine.Physics;
using Silk.NET.OpenGL;

namespace SharpCraft.Engine.Rendering.World;

public static class ChunkMeshBuilder
{
    private struct Face
    {
        public sbyte Dx, Dy, Dz;
        public float Shade;
        public Vector3 V0, V1, V2, V3;
    }

    private static readonly Face[] Faces =
    {
        // Top (+Y)
        new() { Dx = 0, Dy = 1, Dz = 0, Shade = 1.00f,
            V0 = new Vector3(0,1,0), V1 = new Vector3(1,1,0), V2 = new Vector3(1,1,1), V3 = new Vector3(0,1,1) },
        // Bottom (-Y)
        new() { Dx = 0, Dy = -1, Dz = 0, Shade = 0.50f,
            V0 = new Vector3(0,0,0), V1 = new Vector3(1,0,0), V2 = new Vector3(1,0,1), V3 = new Vector3(0,0,1) },
        // North (-Z)
        new() { Dx = 0, Dy = 0, Dz = -1, Shade = 0.80f,
            V0 = new Vector3(0,0,0), V1 = new Vector3(1,0,0), V2 = new Vector3(1,1,0), V3 = new Vector3(0,1,0) },
        // South (+Z)
        new() { Dx = 0, Dy = 0, Dz = 1, Shade = 0.80f,
            V0 = new Vector3(0,0,1), V1 = new Vector3(1,0,1), V2 = new Vector3(1,1,1), V3 = new Vector3(0,1,1) },
        // West (-X)
        new() { Dx = -1, Dy = 0, Dz = 0, Shade = 0.65f,
            V0 = new Vector3(0,0,0), V1 = new Vector3(0,0,1), V2 = new Vector3(0,1,1), V3 = new Vector3(0,1,0) },
        // East (+X)
        new() { Dx = 1, Dy = 0, Dz = 0, Shade = 0.65f,
            V0 = new Vector3(1,0,0), V1 = new Vector3(1,0,1), V2 = new Vector3(1,1,1), V3 = new Vector3(1,1,0) },
    };

    public static ChunkMesh Build(GL gl, Chunk chunk, Func<byte, (Vector3 color, bool solid)> palette)
    {
        var colors = new (Vector3 color, bool solid)[256];
        var colorInit = new bool[256];

        (Vector3 color, bool solid) BlockInfo(byte id)
        {
            if (!colorInit[id])
            {
                colors[id] = palette(id);
                colorInit[id] = true;
            }
            return colors[id];
        }

        bool IsSolid(int x, int y, int z)
        {
            if (!chunk.IsInBounds(x, y, z)) return false;
            byte nid = chunk.Blocks[(x * Chunk.Depth + z) * Chunk.Height + y];
            return nid != 0 && BlockInfo(nid).solid;
        }

        var verts = new List<float>();
        var indices = new List<uint>();
        uint vIdx = 0;

        for (int x = 0; x < Chunk.Width; x++)
        for (int y = 0; y < Chunk.Height; y++)
        for (int z = 0; z < Chunk.Depth; z++)
        {
            byte id = chunk.Blocks[(x * Chunk.Depth + z) * Chunk.Height + y];
            if (id == 0) continue;

            var info = BlockInfo(id);
            if (!info.solid) continue;

            float bx = x, by = y, bz = z;

            foreach (var face in Faces)
            {
                if (IsSolid(x + face.Dx, y + face.Dy, z + face.Dz)) continue;

                var color = info.color * face.Shade;

                void AddVert(float px, float py, float pz)
                {
                    verts.Add(px); verts.Add(py); verts.Add(pz);
                    verts.Add(color.X); verts.Add(color.Y); verts.Add(color.Z);
                }

                AddVert(bx + face.V0.X, by + face.V0.Y, bz + face.V0.Z);
                AddVert(bx + face.V1.X, by + face.V1.Y, bz + face.V1.Z);
                AddVert(bx + face.V2.X, by + face.V2.Y, bz + face.V2.Z);
                AddVert(bx + face.V3.X, by + face.V3.Y, bz + face.V3.Z);

                indices.Add(vIdx); indices.Add(vIdx + 1); indices.Add(vIdx + 2);
                indices.Add(vIdx); indices.Add(vIdx + 2); indices.Add(vIdx + 3);
                vIdx += 4;
            }
        }

        var mesh = new Mesh(gl, verts.ToArray(), indices.ToArray(), hasColor: true);
        return new ChunkMesh(mesh);
    }
}