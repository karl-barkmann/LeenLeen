using System;
using System.Windows;

namespace Leen.Media
{
    /// <summary>
    /// 定义客户端产品中视频智能分析过程中产生的目标坐标参数。
    /// </summary>
    public struct SubjectCordinate
    {
        /// <summary>
        /// 构造一个 <see cref="SubjectCordinate"/>。
        /// </summary>
        /// <param name="x">以左上角为原点的左侧X轴坐标。</param>
        /// <param name="y">以左上角为原点的上侧Y轴坐标。</param>
        /// <param name="width">描述目标的矩形区域的宽度。</param>
        /// <param name="height">描述目标的矩形区域的高度。</param>
        public SubjectCordinate(int x, int y, int width, int height)
        {
            if (x < 0 || y < 0 || width < 1 || height < 1)
            {
                throw new ArgumentException();
            }

            Left = x;
            Top = y;
            Right = x + width;
            Bottom = y + height;
        }

        /// <summary>
        /// 以左上角为原点的左侧X轴坐标。
        /// </summary>
        public int Left;

        /// <summary>
        /// 以左上角为原点的上侧Y轴坐标。
        /// </summary>
        public int Top;

        /// <summary>
        /// 以左上角为原点的右侧X轴坐标。
        /// </summary>
        public int Right;

        /// <summary>
        /// 以左上角为原点的下侧Y轴坐标。
        /// </summary>
        public int Bottom;

        /// <summary>
        /// 获取此区域的宽度。
        /// </summary>
        public int Width
        {
            get
            {
                return Right - Left;
            }
        }

        /// <summary>
        /// 获取此区域的高度。
        /// </summary>
        public int Height
        {
            get
            {
                return Bottom - Top;
            }
        }

        /// <summary>
        /// 获取一个值表示目标坐标区域是否指定的点。
        /// </summary>
        /// <param name="x">点的X轴坐标。</param>
        /// <param name="y">点的Y轴坐标。</param>
        public bool Contains(double x, double y)
        {
            return x >= Left && x <= Right && y >= Top && y <= Bottom;
        }

        /// <summary>
        /// 获取一个值表示目标坐标区域是否指定的点。
        /// </summary>
        /// <param name="point">指定的点。</param>
        public bool Contains(Point point)
        {
            return point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;
        }

        /// <summary>
        /// 获取一个值表示目标坐标区域是否指定的点。
        /// </summary>
        /// <param name="x">点的X轴坐标。</param>
        /// <param name="y">点的Y轴坐标。</param>
        public bool Contains(int x, int y)
        {
            return x >= Left && x <= Right && y >= Top && y <= Bottom;
        }

        /// <summary>
        /// 获取一个值表示目标坐标区域是否指定的点。
        /// </summary>
        /// <param name="point">指定的点。</param>
        public bool Contains(System.Drawing.Point point)
        {
            return point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;
        }
    }
}
