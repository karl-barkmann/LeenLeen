using Leen.Native;
using SlimDX;
using SlimDX.Direct3D9;
using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Leen.Media.Renderer
{
    /// <summary>
    /// A video renderer which implements by DirectX 9, supports multi video formats.
    /// </summary>
    public class D3DImageRenderer : IVideoRenderer, IDisposable
    {
        #region 常量

        private const VertexFormat CUSTOM_VERTEX = VertexFormat.Position | VertexFormat.Diffuse | VertexFormat.Texture1;

        internal const int SM_CXVIRTUALSCREEN = 78;
        internal const int SM_CYVIRTUALSCREEN = 79;
        internal static Format D3DFormatYV12 = D3DX.MakeFourCC((byte)'Y', (byte)'V', (byte)'1', (byte)'2');
        internal static Format D3DFormatNV12 = D3DX.MakeFourCC((byte)'N', (byte)'V', (byte)'1', (byte)'2');
        internal static Color4 BlackColor = new Color4(0x01, 0, 0, 0);

        #endregion

        #region 私有变量

        // 如果为Vista以上，则将实例化为Ex版本
        private Direct3D m_Direct3D;
        private readonly int r_AdapterId;
        private CreateFlags m_CreateFlag;

        private Device m_Device;
        private IntPtr m_DummyRenderHwnd;
        private HwndSource m_DummyRenderWnd;

        private Surface m_InputSurface;
        private Texture m_RendreTexture;
        private Surface m_TextureSurface;
        private VertexBuffer m_VertexBuffer;

        private DisplayMode m_DisplayMode;

        private readonly object r_renderLock;

        // 视频格式信息
        private FrameFormat m_FrameFormat;

        // 帧格式为非YV12, NV12时，uv变量无效. 此时yStride即为图像宽，yHeight即为图像高度，ySize即为图像Buffer大小
        private int m_YStride;
        private int m_YHeight;
        private int m_UVStride;
        private int m_UVHeight;
        private int m_YSize;
        private int m_UVSize;
        private int m_SurfaceYStride;
        private int m_SurfaceUVStride;
        private int m_PixelWidth;
        private int m_PixelHeight;

        private readonly bool r_IsVistaOrBetter;

        private bool m_DriverOrDirectXNotReady;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造 <see cref="D3DImageRenderer"/> 类的实例。
        /// </summary>
        public D3DImageRenderer() : this(0) { }

        /// <summary>
        /// 构造 <see cref="D3DImageRenderer"/> 类的实例。
        /// </summary>
        /// <param name="adapterId">显卡适配器Id。</param>
        public D3DImageRenderer(int adapterId)
        {
            r_renderLock = new object();

            r_IsVistaOrBetter = IsVistaOrBetter;
            r_AdapterId = adapterId;

            m_DummyRenderWnd = CreateChildWindow("D3DDummyWnd");
            m_DummyRenderHwnd = m_DummyRenderWnd.Handle;
        }

        #endregion

        #region IVideoRenderer接口

        /// <summary>
        /// 当后台缓冲发生变化时引发。
        /// </summary>
        public event EventHandler IsBackBufferAvailableChanged;

        /// <summary>
        /// 当后台缓冲刷新时引发。
        /// </summary>
        public event EventHandler BackBufferRefreshed;

        /// <summary>
        /// 获取渲染交换链的后台缓冲。
        /// </summary>
        public IntPtr BackBuffer => m_TextureSurface == null ? IntPtr.Zero : m_TextureSurface.ComPointer;

        /// <summary>
        /// 确认该渲染器是否支持指定视频格式。
        /// </summary>
        /// <param name="format">需要检测的视频帧格式。</param>
        /// <returns></returns>
        /// <exception cref="MediaDeviceUnavailableException">设备不可用。</exception>
        public bool CheckFormat(FrameFormat format)
        {
            if (m_DriverOrDirectXNotReady)
            {
                throw new MediaDeviceUnavailableException();
            }

            EnsureD3DCreated();
            return CheckFormat(ConvertToD3D(format));
        }

        /// <summary>
        /// 使用指定视频参数初始化渲染缓冲表面。
        /// </summary>
        /// <param name="surfaceStride">视频像素宽度。</param>
        /// <param name="surfaceHeight">视频像素高度。</param>
        /// <param name="pixelWidth">视频像素宽度。</param>
        /// <param name="pixelHeight">视频像素高度。</param>
        /// <param name="format">视频帧采样格式。</param>
        /// <returns></returns>
        /// <exception cref="MediaDeviceUnavailableException">设备不可用。</exception>
        public bool SetupSurface(int surfaceStride, int surfaceHeight, int pixelWidth, int pixelHeight, FrameFormat format)
        {
            if (m_DriverOrDirectXNotReady)
            {
                throw new MediaDeviceUnavailableException();
            }

            if (m_InputSurface != null)
            {
                throw new InvalidOperationException("Surface is busy,please release it first.");
            }

            EnsureD3DCreated();

            if (surfaceStride <= 0 || surfaceHeight <= 0 || pixelWidth <= 0 || pixelHeight <= 0)
            {
                throw new ArgumentException(Properties.Resources.STR_ERR_InvalidRenderSize);
            }

            Format d3dFormat = ConvertToD3D(format);

            lock (r_renderLock)
            {
                if (!CheckFormat(d3dFormat))
                {
                    // 显卡不支持该格式
                    throw new NotSupportedException(Properties.Resources.STR_ERR_UnsupportedHardware);
                }
            }

            #region 初始化尺寸参数

            m_PixelWidth = pixelWidth;
            m_PixelHeight = pixelHeight;
            m_FrameFormat = format;
            switch (format)
            {
                case FrameFormat.YV12:
                    m_YStride = pixelWidth;
                    m_SurfaceYStride = surfaceStride;
                    m_YHeight = pixelHeight;
                    m_YSize = surfaceStride * surfaceHeight;
                    m_UVStride = m_YStride >> 1;
                    m_SurfaceUVStride = m_SurfaceYStride >> 1;
                    m_UVHeight = m_YHeight >> 1;
                    m_UVSize = m_YSize >> 2;
                    break;

                case FrameFormat.NV12:
                    m_YStride = pixelWidth;
                    m_SurfaceYStride = surfaceStride;
                    m_YHeight = pixelHeight;
                    m_YSize = surfaceStride * surfaceHeight;
                    m_UVStride = m_YStride;
                    m_SurfaceUVStride = m_SurfaceYStride >> 1;
                    m_UVHeight = m_YHeight >> 1;
                    m_UVSize = m_YSize >> 1;
                    break;

                case FrameFormat.YUY2:
                case FrameFormat.UYVY:
                case FrameFormat.RGB15: // rgb555
                case FrameFormat.RGB16: // rgb565
                    m_YStride = pixelWidth << 1;
                    m_YHeight = pixelHeight;
                    m_SurfaceYStride = surfaceStride;
                    m_YSize = m_YStride * m_YHeight;
                    m_UVStride = m_UVHeight = m_UVSize = 0;
                    m_SurfaceUVStride = 0;
                    break;

                case FrameFormat.RGB32:
                case FrameFormat.ARGB32:
                    m_YStride = pixelWidth << 2;
                    m_SurfaceYStride = surfaceStride;
                    m_YHeight = surfaceHeight;
                    m_YSize = m_YStride * m_YHeight;
                    m_UVStride = m_UVHeight = m_UVSize = 0;
                    m_SurfaceUVStride = 0;
                    break;

                default:
                    return false;
            }

            #endregion

            lock (r_renderLock)
            {
                CreateResource(d3dFormat, pixelWidth, pixelHeight);
            }

            return true;
        }

        /// <summary>
        /// 渲染一帧图像。
        /// </summary>
        /// <param name="buffers">帧数据缓冲，以不同的帧格式对应不同的帧数据数组。</param>
        /// <exception cref="System.InvalidOperationException">设备或缓冲表面未初始化。</exception>
        public void Render(IntPtr[] buffers)
        {
            if (m_InputSurface == null)
            {
                throw new InvalidOperationException("surface not ready,please setup surface first.");
            }

            lock (r_renderLock)
            {
                if (m_FrameFormat == FrameFormat.NV12 ||
                    m_FrameFormat == FrameFormat.YUY2 ||
                    m_FrameFormat == FrameFormat.YV12 ||
                    m_FrameFormat == FrameFormat.UYVY)
                {
                    FillBuffer(buffers[0], buffers[1], buffers[2]);
                }
                else
                {
                    FillBuffer(buffers[0]);
                }

                StretchSurface();

                CreateScene();
            }

            BackBufferRefreshed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 渲染一帧YUV帧采样的视频图像。
        /// </summary>
        /// <param name="uBuffer">U分量数据缓冲。</param>
        /// <param name="vBuffer">V分量数据缓冲。</param>
        /// <param name="yBuffer">Y分量数据缓冲。</param>
        /// <exception cref="System.InvalidOperationException">设备或缓冲表面未初始化。</exception>
        public void Render(IntPtr yBuffer, IntPtr uBuffer, IntPtr vBuffer)
        {
            if (m_InputSurface == null)
            {
                throw new InvalidOperationException("surface not ready,please setup surface first.");
            }

            lock (r_renderLock)
            {
                FillBuffer(yBuffer, uBuffer, vBuffer);

                StretchSurface();

                CreateScene();
            }

            BackBufferRefreshed?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 清除缓冲表面数据。
        /// </summary>
        public void ClearSurface()
        {
            if (m_Device == null || m_InputSurface == null)
            {
                return;
            }

            try
            {
                m_Device.ColorFill(m_TextureSurface, BlackColor);
                BackBufferRefreshed?.Invoke(this, EventArgs.Empty);
            }
            catch (NullReferenceException)
            {
                //SlimDX has some wried bug.
            }

            ReleaseResource();
        }

        /// <summary>
        /// 获取后台渲染表面。
        /// </summary>
        public Surface RenderSurface
        {
            get
            {
                return m_TextureSurface;
            }
        }

        /// <summary>
        /// 获取一个值指示渲染设备是否可用。
        /// </summary>
        public bool IsDeviceAvailable
        {
            get
            {
                return CheckDevice();
            }
        }

        #endregion

        #region 私有函数

        private static HwndSource CreateChildWindow(string windowName)
        {
            HwndSourceParameters hwsp = new HwndSourceParameters(windowName)
            {
                UsesPerPixelOpacity = true,
                WindowStyle = 0 // no style, in particular no WM_CHILD
            };
            HwndSource overlayHwndSource = new HwndSource(hwsp);
            User32.ConvertToChildWindow(overlayHwndSource.Handle);
            return overlayHwndSource;
        }

        private void EnsureD3DCreated()
        {
            if (m_Direct3D == null && !m_DriverOrDirectXNotReady)
            {
                InitD3D(r_AdapterId);
            }
        }

        private void InitD3D(int adapterId = 0)
        {
            try
            {
                m_Direct3D = r_IsVistaOrBetter ? new Direct3DEx() : new Direct3D();
            }
            catch(Direct3D9NotFoundException ex)
            {
                m_DriverOrDirectXNotReady = true;
                throw new MediaDeviceUnavailableException(ex.Message, ex);
            }
            catch (Direct3D9Exception ex)
            {
                //either display driver or directx is not installed
                m_DriverOrDirectXNotReady = true;
                throw new MediaDeviceUnavailableException(ex.Message, ex);
            }

            m_DisplayMode = m_Direct3D.GetAdapterDisplayMode(adapterId);
            Capabilities deviceCap = m_Direct3D.GetDeviceCaps(adapterId, DeviceType.Hardware);
            m_CreateFlag = CreateFlags.Multithreaded | CreateFlags.FpuPreserve;
            if ((int)deviceCap.VertexProcessingCaps != 0)
            {
                m_CreateFlag |= CreateFlags.HardwareVertexProcessing;
            }
            else
            {
                m_CreateFlag |= CreateFlags.SoftwareVertexProcessing;
            }
        }

        private void CreateResource(Format format, int width, int height)
        {
            PresentParameters presentParameters = GetPresentParameters(width, height);

            #region 创建device

            m_Device = r_IsVistaOrBetter ?
                new DeviceEx((Direct3DEx)m_Direct3D, r_AdapterId, DeviceType.Hardware, m_DummyRenderHwnd, m_CreateFlag, presentParameters) :
                new Device(m_Direct3D, r_AdapterId, DeviceType.Hardware, m_DummyRenderHwnd, m_CreateFlag, presentParameters);

            m_Device.SetRenderState(RenderState.CullMode, Cull.None);
            m_Device.SetRenderState(RenderState.ZEnable, ZBufferType.DontUseZBuffer);
            m_Device.SetRenderState(RenderState.Lighting, false);
            m_Device.SetRenderState(RenderState.DitherEnable, true);
            m_Device.SetRenderState(RenderState.MultisampleAntialias, true);
            m_Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            m_Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            m_Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            m_Device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
            m_Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
            m_Device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Clamp);
            m_Device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Clamp);
            m_Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.SelectArg1);
            m_Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
            m_Device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Specular);
            m_Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Modulate);
            m_Device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture);
            m_Device.SetTextureStageState(0, TextureStage.AlphaArg2, TextureArgument.Diffuse);

            #endregion

            #region 创建RenderTarget

            //renderTarget = device.GetRenderTarget(0);
            int vertexSize = Marshal.SizeOf(typeof(Vertex));
            m_RendreTexture = new Texture(m_Device, width, height, 1, Usage.RenderTarget, m_DisplayMode.Format, Pool.Default);
            m_TextureSurface = m_RendreTexture.GetSurfaceLevel(0);

            Vertex[] vertexList = new Vertex[]
            {
                new Vertex(){ pos = new Vector3(0, 0, 0), texPos = new Vector2(0, 0), color = 0xFFFFFFFF}, // 左上
                new Vertex(){ pos = new Vector3(width, 0, 0), texPos = new Vector2(1, 0), color = 0xFFFFFFFF}, // 右上
                new Vertex(){ pos = new Vector3(width, height, 0), texPos = new Vector2(1, 1), color = 0xFFFFFFFF}, // 右下
                new Vertex(){ pos = new Vector3(0, height, 0), texPos = new Vector2(0, 1), color = 0xFFFFFFFF}, // 左下
            };

            m_VertexBuffer = new VertexBuffer(m_Device, vertexSize * 4, Usage.Dynamic | Usage.WriteOnly, CUSTOM_VERTEX, Pool.Default);
            DataStream stream = m_VertexBuffer.Lock(0, 0, LockFlags.Discard);

            // 左上
            IntPtr dataPointer = stream.DataPointer;
            Marshal.StructureToPtr(vertexList[0], dataPointer, true);

            // 右上
            dataPointer += vertexSize;
            Marshal.StructureToPtr(vertexList[1], dataPointer, true);

            // 右下
            dataPointer += vertexSize;
            Marshal.StructureToPtr(vertexList[2], dataPointer, true);

            // 左下
            dataPointer += vertexSize;
            Marshal.StructureToPtr(vertexList[3], dataPointer, true);

            m_VertexBuffer.Unlock();

            #endregion

            SetupMatrices(width, height);

            #region 创建input surface
            m_InputSurface = r_IsVistaOrBetter ?
                Surface.CreateOffscreenPlainEx((DeviceEx)m_Device, width, height, format, Pool.Default, Usage.None) :
                Surface.CreateOffscreenPlain(m_Device, width, height, format, Pool.Default);

            m_Device.ColorFill(m_InputSurface, BlackColor);

            IsBackBufferAvailableChanged?.Invoke(this, EventArgs.Empty);

            #endregion
        }

        private void ReleaseResource()
        {
            SafeRelease(m_InputSurface);
            m_InputSurface = null;

            SafeRelease(m_VertexBuffer);

            SafeRelease(m_RendreTexture);
            SafeRelease(m_TextureSurface);
            m_TextureSurface = null;

            SafeRelease(m_Device);
            m_Device = null;

            ClearBackBuffer();
        }

        private void ClearBackBuffer()
        {
            IsBackBufferAvailableChanged?.Invoke(this, EventArgs.Empty);
        }

        private PresentParameters GetPresentParameters(int width, int height)
        {
            PresentParameters presentParams = new PresentParameters()
            {
                PresentFlags = PresentFlags.Video | PresentFlags.OverlayYCbCr_BT709,
                Windowed = true,
                DeviceWindowHandle = m_DummyRenderHwnd,
                BackBufferWidth = width == 0 ? 1 : width,
                BackBufferHeight = height == 0 ? 1 : height,
                SwapEffect = SwapEffect.Discard,
                //presentParams.Multisample = MultisampleType.NonMaskable;
                PresentationInterval = PresentInterval.Immediate,
                BackBufferFormat = m_DisplayMode.Format,
                BackBufferCount = 1,
                EnableAutoDepthStencil = false
            };
            return presentParams;
        }

        private void FillBuffer(IntPtr bufferPtr)
        {
            DataRectangle rect = m_InputSurface.LockRectangle(LockFlags.None);
            IntPtr surfaceBufferPtr = rect.Data.DataPointer;
            switch (m_FrameFormat)
            {
                case FrameFormat.YV12:
                    #region 填充YV12数据
                    if (rect.Pitch == m_YStride)
                    {
                        Ntdll.Memcpy(surfaceBufferPtr, bufferPtr, m_YSize + m_UVSize + m_UVSize);
                    }
                    else
                    {
                        IntPtr srcPtr = bufferPtr; // Y
                        int yPitch = rect.Pitch;
                        for (int i = 0; i < m_YHeight; i++)
                        {
                            Ntdll.Memcpy(surfaceBufferPtr, srcPtr, m_YStride);
                            surfaceBufferPtr += yPitch;
                            srcPtr += m_SurfaceYStride;
                        }

                        int uvPitch = yPitch >> 1;
                        for (int i = 0; i < m_YHeight; i++) // UV一起copy, uHeight + vHeight = yHeight
                        {
                            Ntdll.Memcpy(surfaceBufferPtr, srcPtr, m_UVStride);
                            surfaceBufferPtr += uvPitch;
                            srcPtr += m_SurfaceUVStride;
                        }
                    }
                    #endregion
                    break;

                case FrameFormat.NV12:
                    #region 填充NV12. uBuffer指向UV打包数据。vBuffer为空

                    if (rect.Pitch == m_YStride)
                    {
                        Ntdll.Memcpy(surfaceBufferPtr, bufferPtr, m_YSize + m_UVSize);
                    }
                    else
                    {
                        // uv打包保存，uvWidth与yWidth相同, 因此可以合并在一个循环
                        IntPtr srcPtr = bufferPtr;
                        for (int i = 0; i < m_YHeight + m_UVHeight; i++)
                        {
                            Ntdll.Memcpy(surfaceBufferPtr, srcPtr, m_YStride);
                            surfaceBufferPtr += rect.Pitch;
                            srcPtr += m_SurfaceYStride;
                        }
                    }
                    #endregion
                    break;

                // 打包格式
                case FrameFormat.YUY2:
                case FrameFormat.UYVY:
                case FrameFormat.RGB15:
                case FrameFormat.RGB16:
                case FrameFormat.RGB24:
                case FrameFormat.RGB32:
                case FrameFormat.ARGB32:
                default:
                    #region 填充buffer。此时，所有数据都在yBuffer里，其他两个buffer无效
                    if (rect.Pitch == m_YStride)
                    {
                        Ntdll.Memcpy(surfaceBufferPtr, bufferPtr, m_YSize); // ySize此时等于整个dataSize
                    }
                    else
                    {
                        IntPtr srcPtr = bufferPtr;
                        for (int i = 0; i < m_YHeight; i++)
                        {
                            Ntdll.Memcpy(surfaceBufferPtr, srcPtr, m_YStride);
                            surfaceBufferPtr += rect.Pitch;
                            srcPtr += m_SurfaceYStride;
                        }
                    }
                    #endregion
                    break;
            }

            m_InputSurface.UnlockRectangle();
        }

        private void FillBuffer(IntPtr yBuffer, IntPtr uBuffer, IntPtr vBuffer)
        {
            if (m_InputSurface == null)
            {
                return;
            }

            DataRectangle rect = m_InputSurface.LockRectangle(LockFlags.None);
            IntPtr surfaceBufferPtr = rect.Data.DataPointer;
            switch (m_FrameFormat)
            {
                case FrameFormat.YV12:
                    #region 填充YV12数据
                    if (rect.Pitch == m_YStride)
                    {
                        Ntdll.Memcpy(surfaceBufferPtr, yBuffer, m_YSize); // Y
                        surfaceBufferPtr += m_YSize;
                        Ntdll.Memcpy(surfaceBufferPtr, vBuffer, m_UVSize); // V 
                        surfaceBufferPtr += m_UVSize;
                        Ntdll.Memcpy(surfaceBufferPtr, uBuffer, m_UVSize); // U
                    }
                    else
                    {
                        int yPitch = rect.Pitch;
                        IntPtr yPtr = yBuffer; // Y

                        for (int i = 0; i < m_YHeight; i++)
                        {
                            Ntdll.Memcpy(surfaceBufferPtr, yPtr, m_YStride);
                            surfaceBufferPtr += yPitch;
                            yPtr += m_SurfaceYStride;
                        }

                        int uvPitch = yPitch >> 1;

                        IntPtr vPtr = vBuffer; // V
                        for (int i = 0; i < m_UVHeight; i++)
                        {
                            Ntdll.Memcpy(surfaceBufferPtr, vPtr, m_UVStride);
                            surfaceBufferPtr += uvPitch;
                            vPtr += m_SurfaceUVStride;
                        }

                        IntPtr uPtr = uBuffer; // U
                        for (int i = 0; i < m_UVHeight; i++)
                        {
                            Ntdll.Memcpy(surfaceBufferPtr, uPtr, m_UVStride);
                            surfaceBufferPtr += uvPitch;
                            uPtr += m_SurfaceUVStride;
                        }
                    }
                    #endregion
                    break;

                case FrameFormat.NV12:
                    #region 填充NV12. uBuffer指向UV打包数据。vBuffer为空

                    if (rect.Pitch == m_YStride)
                    {
                        Ntdll.Memcpy(surfaceBufferPtr, yBuffer, m_YSize); // Copy Y数据
                        surfaceBufferPtr += m_YSize;
                        Ntdll.Memcpy(surfaceBufferPtr, uBuffer, m_UVSize); // Copy UV打包数据
                    }
                    else
                    {
                        // Copy Y数据
                        IntPtr yPtr = yBuffer;
                        for (int i = 0; i < m_YHeight; i++)
                        {
                            Ntdll.Memcpy(surfaceBufferPtr, yPtr, m_YStride);
                            surfaceBufferPtr += rect.Pitch;
                            yPtr += m_SurfaceYStride;
                        }

                        // Copy UV打包数据
                        IntPtr uvPtr = uBuffer;
                        for (int i = 0; i < m_UVHeight; i++)
                        {
                            Ntdll.Memcpy(surfaceBufferPtr, uvPtr, m_UVStride);
                            surfaceBufferPtr += rect.Pitch; // 此时uv打包保存，每行数据与y相同
                            uvPtr += m_SurfaceUVStride;
                        }
                    }
                    #endregion
                    break;

                case FrameFormat.YUY2:
                case FrameFormat.UYVY:
                case FrameFormat.RGB15:
                case FrameFormat.RGB16:
                case FrameFormat.RGB32:
                case FrameFormat.ARGB32:
                default:
                    #region 填充buffer。此时，所有数据都在yBuffer里，其他两个buffer无效
                    if (rect.Pitch == m_YStride)
                    {
                        Ntdll.Memcpy(surfaceBufferPtr, yBuffer, m_YSize); // ySize此时等于整个dataSize
                    }
                    else
                    {
                        IntPtr yPtr = yBuffer;
                        for (int i = 0; i < m_YHeight; i++)
                        {
                            Ntdll.Memcpy(surfaceBufferPtr, yPtr, m_YStride);
                            surfaceBufferPtr += rect.Pitch;
                            yBuffer += m_SurfaceYStride;  // yWidth即为每行图像stride
                        }
                    }
                    #endregion
                    break;
            }

            m_InputSurface.UnlockRectangle();
        }

        private void StretchSurface()
        {
            m_Device.ColorFill(m_TextureSurface, BlackColor);

            m_Device.StretchRectangle(m_InputSurface, m_TextureSurface, TextureFilter.Linear);
        }

        private void CreateScene()
        {
            m_Device.Clear(ClearFlags.Target, BlackColor, 1.0f, 0);
            m_Device.BeginScene();

            m_Device.VertexFormat = CUSTOM_VERTEX;

            m_Device.SetStreamSource(0, m_VertexBuffer, 0, Marshal.SizeOf(typeof(Vertex)));

            m_Device.SetTexture(0, m_RendreTexture);

            m_Device.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
            m_Device.EndScene();
        }

        private void Present()
        {
            if (r_IsVistaOrBetter)
            {
                ((DeviceEx)m_Device).PresentEx(SlimDX.Direct3D9.Present.None);
            }
            else
            {
                m_Device.Present();
            }
        }

        /// <summary>
        /// 检查d3d设备是否正常
        /// </summary>
        /// <returns></returns>
        private bool CheckDevice()
        {
            if (m_Device == null)
                return false;

            if (r_IsVistaOrBetter)
            {
                DeviceState state = ((DeviceEx)m_Device).CheckDeviceState(m_DummyRenderHwnd);
                return state == DeviceState.Ok;
            }
            else
            {
                return false; // xp无法支持ex
            }
        }

        private void SetupMatrices(int width, int height)
        {
            SlimDX.Matrix matOrtho = SlimDX.Matrix.OrthoOffCenterLH(0, width, height, 0, 0.0f, 1.0f);
            SlimDX.Matrix matIdentity = SlimDX.Matrix.Identity;

            m_Device.SetTransform(TransformState.Projection, matOrtho);
            m_Device.SetTransform(TransformState.World, matIdentity);
            m_Device.SetTransform(TransformState.View, matIdentity);
        }

        private bool CheckFormat(Format d3dFormat)
        {
            if (!m_Direct3D.CheckDeviceFormat(r_AdapterId, DeviceType.Hardware, m_DisplayMode.Format, Usage.None, ResourceType.Surface, d3dFormat))
            {
                return false;
            }

            return m_Direct3D.CheckDeviceFormatConversion(r_AdapterId, DeviceType.Hardware, d3dFormat, m_DisplayMode.Format);
        }

        private void GetWindowSize(IntPtr hwnd, out int width, out int height)
        {
            User32.RECT rect = new User32.RECT();
            User32.GetWindowRect(hwnd, ref rect);
            height = rect.Bottom - rect.Top;
            width = rect.Right - rect.Left;
        }

        private static Format ConvertToD3D(FrameFormat format)
        {
            switch (format)
            {
                case FrameFormat.YV12:
                    return D3DFormatYV12;

                case FrameFormat.NV12:
                    return D3DFormatNV12;

                case FrameFormat.YUY2:
                    return Format.Yuy2;

                case FrameFormat.UYVY:
                    return Format.Uyvy;

                case FrameFormat.RGB15:
                    return Format.X1R5G5B5;

                case FrameFormat.RGB16:
                    return Format.R5G6B5;

                case FrameFormat.RGB32:
                    return Format.X8R8G8B8;

                case FrameFormat.ARGB32:
                    return Format.A8R8G8B8;

                case FrameFormat.RGB24:
                    return Format.R8G8B8;

                default:
                    throw new ArgumentException("Unknown pixel format", "format");
            }
        }

        private static bool IsVistaOrBetter
        {
            get
            {
                return Environment.OSVersion.Version.Major >= 6;
            }
        }

        #endregion

        #region IDisposable

        private bool isDisposed = false;

        /// <summary>
        /// 释放此媒体渲染接口的资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                isDisposed = true;

                if (disposing)
                {
                    ReleaseResource();

                    SafeRelease(m_Direct3D);
                    SafeRelease(m_DummyRenderWnd);
                }
            }
        }

        private void SafeRelease(IDisposable item)
        {
            try
            {
                if (item != null)
                {
                    item.Dispose();
                }
            }
            catch
            {
            }
        }

        #endregion
    }
}
