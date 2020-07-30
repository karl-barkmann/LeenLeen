using Leen.Media.Renderer;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Leen.Media.Controls.Primitives
{
    interface IRenderTarget
    {
        Transform RenderTransform { get; set; }

        D3DImage ForegroundRenderSource { get; set; }

        void Initialize(IMediaPlayer player, IVideoRenderer renderer);

        void Clear();

        void Setup(Size renderSize);
    }
}
