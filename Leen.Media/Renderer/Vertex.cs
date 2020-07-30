using System.Runtime.InteropServices;
using SlimDX;

namespace Leen.Media.Renderer
{
    /// <summary>
    /// Vertex is the most common primitive of Direct3D API,it has at least 3 points, 
    /// X, Y and Z which describe a location in a 3D space and optionally diffuse color and a texture coordinate (discussed later). 
    /// When working with 2D graphics, the Z value is unnecessary and always will be zero. 
    /// Direct3D lets you construct vertex constructs of your flavor and you also need to tell the API what each field of the Vertex structure represents.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    class Vertex
    {
        public Vector3 pos;        // vertex untransformed position
        public uint color;         // diffuse color
        public Vector2 texPos;     // texture relative coordinates
    }
}
