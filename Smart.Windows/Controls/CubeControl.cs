//zhongying 2012.9.7

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Smart.Windows.Controls
{
    /// <summary>
    /// 立方体控件，提供水平和垂直 90度旋转的命令
    /// </summary>
    internal class CubeControl : Viewport3D
    {
        #region 成员变量

        double xAngle;
        double yAngle;
        //所有Viewprod2dVisual3d公用的材质
        DiffuseMaterial material = new DiffuseMaterial(Brushes.White);
        Viewport2DVisual3D backViewPort;
        Viewport2DVisual3D frontViewPort;
        Viewport2DVisual3D leftViewPort;
        Viewport2DVisual3D rightViewPort;
        Viewport2DVisual3D upViewPort;
        Viewport2DVisual3D bottomViewPort;

        #endregion

        #region 构造函数

        /// <summary>
        /// Initializes the <see cref="CubeControl"/> class.
        /// </summary>
        static CubeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CubeControl), new FrameworkPropertyMetadata(typeof(CubeControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CubeControl"/> class.
        /// </summary>
        public CubeControl()
        {
            material.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);
            Loaded += new RoutedEventHandler(CubeControl_Loaded);
        }

        void CubeControl_Loaded(object sender, RoutedEventArgs e)
        {
            BuildCube();
        }

        /// <summary>
        /// 在派生类中重写时，会参与由布局系统控制的呈现操作。调用此方法时，不直接使用此元素的呈现指令，而是将其保留供布局和绘制在以后异步使用。
        /// </summary>
        /// <param name="drawingContext">特定元素的绘制指令。此上下文是为布局系统提供的。</param>
        protected override void OnRender(DrawingContext drawingContext)
        {

            base.OnRender(drawingContext);
        }

        #endregion

        #region 公开属性

        #region 依赖属性

        #region Duration

        /// <summary>
        /// The <see cref="Duration" /> dependency property's name.
        /// </summary>
        public const string DurationPropertyName = "Duration";

        /// <summary>
        /// 获取或设置 动画过度毫秒
        /// </summary>
        public double Duration
        {
            get
            {
                return (double)GetValue(DurationProperty);
            }
            set
            {
                SetValue(DurationProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Duration" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
            DurationPropertyName,
            typeof(double),
            typeof(CubeControl),
            new PropertyMetadata(300d));

        #endregion

        #region CubeWidth

        /// <summary>
        /// The <see cref="CubeWidth" /> dependency property's name.
        /// </summary>
        public const string CubeWidthPropertyName = "CubeWidth";

        /// <summary>
        /// 获取或设置 矩形宽度
        /// </summary>
        public double CubeWidth
        {
            get
            {
                return (double)GetValue(CubeWidthProperty);
            }
            set
            {
                SetValue(CubeWidthProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CubeWidth" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CubeWidthProperty = DependencyProperty.Register(
            CubeWidthPropertyName,
            typeof(double),
            typeof(CubeControl),
            new PropertyMetadata(5d, new PropertyChangedCallback(PropertyChangedCallback)));

        static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CubeControl control = d as CubeControl;
            control.BuildCube();
        }

        #endregion

        #region CubeHeight

        /// <summary>
        /// The <see cref="CubeHeight" /> dependency property's name.
        /// </summary>
        public const string CubeHeightPropertyName = "CubeHeight";

        /// <summary>
        /// Gets or sets the value of the <see cref="CubeHeight" />
        /// property. This is a dependency property.
        /// </summary>
        public double CubeHeight
        {
            get
            {
                return (double)GetValue(CubeHeightProperty);
            }
            set
            {
                SetValue(CubeHeightProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CubeHeight" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CubeHeightProperty = DependencyProperty.Register(
            CubeHeightPropertyName,
            typeof(double),
            typeof(CubeControl),
            new PropertyMetadata(3d, new PropertyChangedCallback(PropertyChangedCallback)));

        #endregion

        #region CubeLength

        /// <summary>
        /// The <see cref="CubeLength" /> dependency property's name.
        /// </summary>
        public const string CubeLengthPropertyName = "CubeLength";

        /// <summary>
        /// Gets or sets the value of the <see cref="CubeLength" />
        /// property. This is a dependency property.
        /// </summary>
        public double CubeLength
        {
            get
            {
                return (double)GetValue(CubeLengthProperty);
            }
            set
            {
                SetValue(CubeLengthProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CubeLength" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CubeLengthProperty = DependencyProperty.Register(
            CubeLengthPropertyName,
            typeof(double),
            typeof(CubeControl),
            new PropertyMetadata(10d, new PropertyChangedCallback(PropertyChangedCallback)));

        #endregion

        #region FrontContent

        /// <summary>
        /// The <see cref="FrontContent" /> dependency property's name.
        /// </summary>
        public const string FrontContentPropertyName = "FrontContent";

        /// <summary>
        /// Gets or sets the value of the <see cref="FrontContent" />
        /// property. This is a dependency property.
        /// </summary>
        public FrameworkElement FrontContent
        {
            get
            {
                return (FrameworkElement)GetValue(FrontContentProperty);
            }
            set
            {
                SetValue(FrontContentProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="FrontContent" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrontContentProperty = DependencyProperty.Register(
            FrontContentPropertyName,
            typeof(FrameworkElement),
            typeof(CubeControl),
            new PropertyMetadata(new PropertyChangedCallback(FrontPropertyChangedCallback)));

        static void FrontPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CubeControl control = d as CubeControl;
            if (control.frontViewPort != null)
                control.frontViewPort.Visual = e.NewValue as FrameworkElement;
        }

        #endregion

        #region BackContent

        /// <summary>
        /// The <see cref="BackContent" /> dependency property's name.
        /// </summary>
        public const string BackContentPropertyName = "BackContent";

        /// <summary>
        /// 获取或设置 背面显示控件
        /// </summary>
        public FrameworkElement BackContent
        {
            get
            {
                return (FrameworkElement)GetValue(BackContentProperty);
            }
            set
            {
                SetValue(BackContentProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="BackContent" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackContentProperty = DependencyProperty.Register(
            BackContentPropertyName,
            typeof(FrameworkElement),
            typeof(CubeControl),
            new PropertyMetadata(BackPropertyChangedCallback));

        static void BackPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CubeControl control = d as CubeControl;
            if (control.backViewPort != null)
                control.backViewPort.Visual = e.NewValue as FrameworkElement;
        }

        #endregion

        #region UpContent

        /// <summary>
        /// The <see cref="UpContent" /> dependency property's name.
        /// </summary>
        public const string UpContentPropertyName = "UpContent";

        /// <summary>
        /// 获取或设置 顶部内容
        /// </summary>
        public FrameworkElement UpContent
        {
            get
            {
                return (FrameworkElement)GetValue(UpContentProperty);
            }
            set
            {
                SetValue(UpContentProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="UpContent" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpContentProperty = DependencyProperty.Register(
            UpContentPropertyName,
            typeof(FrameworkElement),
            typeof(CubeControl),
            new PropertyMetadata(UpPropertyChangedCallback));

        static void UpPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CubeControl control = d as CubeControl;
            if (control.upViewPort != null)
                control.upViewPort.Visual = e.NewValue as FrameworkElement;
        }

        #endregion

        #region BottomContent

        /// <summary>
        /// The <see cref="BottomContent" /> dependency property's name.
        /// </summary>
        public const string BottomContentPropertyName = "BottomContent";

        /// <summary>
        /// 获取或设置 底部内容
        /// </summary>
        public FrameworkElement BottomContent
        {
            get
            {
                return (FrameworkElement)GetValue(BottomContentProperty);
            }
            set
            {
                SetValue(BottomContentProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="BottomContent" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomContentProperty = DependencyProperty.Register(
            BottomContentPropertyName,
            typeof(FrameworkElement),
            typeof(CubeControl),
            new PropertyMetadata(BottomPropertyChangedCallback));

        static void BottomPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CubeControl control = d as CubeControl;
            if (control.bottomViewPort != null)
                control.bottomViewPort.Visual = e.NewValue as FrameworkElement;
        }

        #endregion

        #region LeftContent

        /// <summary>
        /// The <see cref="LeftContent" /> dependency property's name.
        /// </summary>
        public const string LeftContentPropertyName = "LeftContent";

        /// <summary>
        /// 获取或设置 左侧内容
        /// </summary>
        public FrameworkElement LeftContent
        {
            get
            {
                return (FrameworkElement)GetValue(LeftContentProperty);
            }
            set
            {
                SetValue(LeftContentProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="LeftContent" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftContentProperty = DependencyProperty.Register(
            LeftContentPropertyName,
            typeof(FrameworkElement),
            typeof(CubeControl),
            new PropertyMetadata(LeftPropertyChangedCallback));


        static void LeftPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CubeControl control = d as CubeControl;
            if (control.leftViewPort != null)
                control.leftViewPort.Visual = e.NewValue as FrameworkElement;
        }

        #endregion

        #region RightContent

        /// <summary>
        /// The <see cref="RightContent" /> dependency property's name.
        /// </summary>
        public const string RightContentPropertyName = "RightContent";

        /// <summary>
        /// 获取或设置 右侧内容
        /// </summary>
        public FrameworkElement RightContent
        {
            get
            {
                return (FrameworkElement)GetValue(RightContentProperty);
            }
            set
            {
                SetValue(RightContentProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="RightContent" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightContentProperty = DependencyProperty.Register(
            RightContentPropertyName,
            typeof(FrameworkElement),
            typeof(CubeControl),
            new PropertyMetadata(RightPropertyChangedCallback));

        static void RightPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CubeControl control = d as CubeControl;
            if (control.rightViewPort != null)
                control.rightViewPort.Visual = e.NewValue as FrameworkElement;
        }

        #endregion

        #region Surface

        /// <summary>
        /// 表示立方体的各个面
        /// </summary>
        public enum Surface
        {
            /// <summary>
            /// 上面
            /// </summary>
            Up,
            /// <summary>
            /// 左侧
            /// </summary>
            Left,
            /// <summary>
            /// 正面
            /// </summary>
            Front,
            /// <summary>
            /// 右侧
            /// </summary>
            Right,
            /// <summary>
            /// 下面
            /// </summary>
            Down,
            /// <summary>
            /// 背面
            /// </summary>
            Back
        }

        #endregion

        #region CurrentSurface

        /// <summary>
        /// The <see cref="CurrentSurface" /> dependency property's name.
        /// </summary>
        public const string CurrentSurfacePropertyName = "CurrentSurface";

        /// <summary>
        /// Gets or sets the value of the <see cref="CurrentSurface" />
        /// property. This is a dependency property.
        /// </summary>
        public Surface CurrentSurface
        {
            get
            {
                return (Surface)GetValue(CurrentSurfaceProperty);
            }
            set
            {
                SetValue(CurrentSurfaceProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CurrentSurface" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentSurfaceProperty = DependencyProperty.Register(
            CurrentSurfacePropertyName,
            typeof(Surface),
            typeof(CubeControl),
            new PropertyMetadata(Surface.Front, new PropertyChangedCallback(CurrentSurfacePropertyChangedCallback)));

        static void CurrentSurfacePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CubeControl control = d as CubeControl;
            Surface surface = (Surface)e.NewValue;
            Surface oldSurface = (Surface)e.OldValue;
            control.Turn(oldSurface, surface);
        }

        #endregion

        #endregion

        #endregion

        #region 重写

        private void BuildCube()
        {
            //Children.Clear();
            BuildCamera();
            //以x轴为长度Length，y轴为高度Height,z轴为宽度Width

            //坐标点,全部除以2，已0,0,0坐标点为立方体的正中心
            BuildFront();
            BuildBack();
            BuildUp();
            BuildBottom();
            BuildLeft();
            BuildRight();
            BuildLight();
        }

        private void BuildCamera()
        {
            if (Camera != null)
                return;
            //创建摄像机
            var camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, CubeHeight + 25);
            camera.FieldOfView = 45;
            Camera = camera;
        }

        private void BuildLight()
        {
            //创建光线
            ModelVisual3D modelVisual3d = new ModelVisual3D();
            modelVisual3d.Content = new AmbientLight(Colors.White);
            Children.Add(modelVisual3d);
        }

        private void BuildFront()
        {
            //构造图形
            var geometry = new MeshGeometry3D();

            double x = CubeLength / 2;
            double y = CubeHeight / 2;
            double z = CubeWidth / 2;
            geometry.Positions = new Point3DCollection((new Point3D[] 
            { 
                new Point3D(-x, -y,z), //左下角
                new Point3D(x,-y,z), //右下角
                new Point3D(x,y,z), //右上角
                new Point3D(-x,y,z) //左上角
            }));

            //三角形构造,影响呈现的正反面
            geometry.TriangleIndices = new Int32Collection(new int[] { 0, 1, 2, 2, 3, 0 });
            //纹理
            geometry.TextureCoordinates = new PointCollection(new Point[] { new Point(0, 1), new Point(1, 1), new Point(1, 0), new Point() });

            //如果已经有了，只更新图形
            if (frontViewPort != null)
            {
                frontViewPort.Geometry = geometry;
                return;
            }
            //正面
            frontViewPort = new Viewport2DVisual3D();

            frontViewPort.Geometry = geometry;

            if (FrontContent != null)
            {
                frontViewPort.Visual = FrontContent;
            }
            else
            {
                var grid = new Grid();
                grid.Children.Add(new TextBlock() { Text = "正面", Background = Brushes.BlueViolet });
                frontViewPort.Visual = grid;
            }
            frontViewPort.Material = material;

            Children.Add(frontViewPort);
        }

        private void BuildBack()
        {
            var geometry = new MeshGeometry3D();

            double x = CubeLength / 2;
            double y = CubeHeight / 2;
            double z = CubeWidth / 2;
            geometry.Positions = new Point3DCollection((new Point3D[] 
            { 
                new Point3D(-x, -y,-z), //左下角
                new Point3D(x,-y,-z), //右下角
                new Point3D(x,y,-z), //右上角
                new Point3D(-x,y,-z) //左上角
            }));

            //三角形构造,影响呈现的正反面
            geometry.TriangleIndices = new Int32Collection(new int[] { 0, 2, 1, 0, 3, 2 });
            //纹理
            geometry.TextureCoordinates = new PointCollection(new Point[] { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(0, 1) });


            if (backViewPort != null)
            {
                backViewPort.Geometry = geometry;
                return;
            }
            //背面            
            backViewPort = new Viewport2DVisual3D();

            backViewPort.Geometry = geometry;
            var grid = new Grid();
            if (BackContent != null)
                backViewPort.Visual = BackContent;
            else
            {
                grid.Children.Add(new TextBlock() { Text = "背面", Background = Brushes.Blue });
                backViewPort.Visual = grid;
            }

            backViewPort.Material = material;


            Children.Add(backViewPort);
        }

        private void BuildUp()
        {
            var geometry = new MeshGeometry3D();

            double x = CubeLength / 2;
            double y = CubeHeight / 2;
            double z = CubeWidth / 2;
            geometry.Positions = new Point3DCollection((new Point3D[] 
            { 
                new Point3D(-x, y,z), 
                new Point3D(x,y,z), 
                new Point3D(x,y,-z), 
                new Point3D(-x,y,-z) 
            }));

            //三角形构造,影响呈现的正反面
            geometry.TriangleIndices = new Int32Collection(new int[] { 0, 1, 2, 2, 3, 0 });
            //纹理
            geometry.TextureCoordinates = new PointCollection(new Point[] { new Point(0, 1), new Point(1, 1), new Point(1, 0), new Point(0, 0) });

            if (upViewPort != null)
            {
                upViewPort.Geometry = geometry;
                return;
            }

            //顶层            
            upViewPort = new Viewport2DVisual3D();

            upViewPort.Geometry = geometry;
            var grid = new Grid();
            if (UpContent != null)
                upViewPort.Visual = UpContent;
            else
            {
                grid.Children.Add(new TextBlock() { Text = "顶部", Background = Brushes.Blue });
                upViewPort.Visual = grid;
            }

            upViewPort.Material = material;


            Children.Add(upViewPort);
        }

        private void BuildBottom()
        {
            var geometry = new MeshGeometry3D();

            double x = CubeLength / 2;
            double y = CubeHeight / 2;
            double z = CubeWidth / 2;
            geometry.Positions = new Point3DCollection((new Point3D[] 
            { 
                new Point3D(-x, -y,z), 
                new Point3D(x,-y,z), 
                new Point3D(x,-y,-z), 
                new Point3D(-x,-y,-z) 
            }));

            //三角形构造,影响呈现的正反面
            geometry.TriangleIndices = new Int32Collection(new int[] { 0, 3, 2, 2, 1, 0 });
            //纹理
            geometry.TextureCoordinates = new PointCollection(new Point[] { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(0, 1) });


            if (bottomViewPort != null)
            {
                bottomViewPort.Geometry = geometry;
                return;
            }

            //底层            
            bottomViewPort = new Viewport2DVisual3D();

            bottomViewPort.Geometry = geometry;
            var grid = new Grid();
            if (BottomContent != null)
                bottomViewPort.Visual = BottomContent;
            else
            {
                grid.Children.Add(new TextBlock() { Text = "底部", Background = Brushes.Blue });
                bottomViewPort.Visual = grid;
            }

            bottomViewPort.Material = material;


            Children.Add(bottomViewPort);
        }

        private void BuildLeft()
        {
            var geometry = new MeshGeometry3D();

            double x = CubeLength / 2;
            double y = CubeHeight / 2;
            double z = CubeWidth / 2;
            geometry.Positions = new Point3DCollection((new Point3D[] 
            { 
                new Point3D(-x, -y,-z), 
                new Point3D(-x,-y,z), 
                new Point3D(-x,y,z), 
                new Point3D(-x,y,-z) 
            }));

            //三角形构造,影响呈现的正反面
            geometry.TriangleIndices = new Int32Collection(new int[] { 0, 1, 2, 2, 3, 0 });
            //纹理
            geometry.TextureCoordinates = new PointCollection(new Point[] { new Point(0, 1), new Point(1, 1), new Point(1, 0), new Point(0, 0) });


            if (leftViewPort != null)
            {
                leftViewPort.Geometry = geometry;
                return;
            }

            leftViewPort = new Viewport2DVisual3D();

            leftViewPort.Geometry = geometry;
            var grid = new Grid();
            if (LeftContent != null)
                leftViewPort.Visual = LeftContent;
            else
            {
                grid.Children.Add(new TextBlock() { Text = "左侧", Background = Brushes.Blue });
                leftViewPort.Visual = grid;
            }

            leftViewPort.Material = material;


            Children.Add(leftViewPort);
        }

        private void BuildRight()
        {
            var geometry = new MeshGeometry3D();

            double x = CubeLength / 2;
            double y = CubeHeight / 2;
            double z = CubeWidth / 2;
            geometry.Positions = new Point3DCollection((new Point3D[] 
            { 
                new Point3D(x, -y,-z), 
                new Point3D(x,-y,z), 
                new Point3D(x,y,z), 
                new Point3D(x,y,-z) 
            }));

            //三角形构造,影响呈现的正反面
            geometry.TriangleIndices = new Int32Collection(new int[] { 0, 3, 2, 0, 2, 1 });
            //纹理
            geometry.TextureCoordinates = new PointCollection(new Point[] { new Point(1, 1), new Point(0, 1), new Point(0, 0), new Point(1, 0) });


            if (rightViewPort != null)
            {
                rightViewPort.Geometry = geometry;
                return;
            }

            rightViewPort = new Viewport2DVisual3D();

            rightViewPort.Geometry = geometry;
            var grid = new Grid();
            if (RightContent != null)
                rightViewPort.Visual = RightContent;
            else
            {
                grid.Children.Add(new TextBlock() { Text = "右侧", Background = Brushes.Blue });
                rightViewPort.Visual = grid;
            }

            rightViewPort.Material = material;


            Children.Add(rightViewPort);
        }

        #endregion

        #region 公开命令

        #region TurnLeftCommmand

        private ICommand turnLeftCommand;

        /// <summary>
        /// 获取 LeftCommmand。
        /// </summary>
        public ICommand TurnLeftCommmand
        {
            get
            {
                if (turnLeftCommand == null)
                    turnLeftCommand = new RelayCommand(ExecuteTurnLeftCommmand);
                return turnLeftCommand;
            }
        }

        private void ExecuteTurnLeftCommmand()
        {
            var surface = GetSurface(xAngle - 90, true);

            CurrentSurface = surface;
        }

        #endregion

        #region TurnRightCommand

        private RelayCommand turnRightCommand;

        /// <summary>
        /// 获取 TurnRightCommand。
        /// </summary>
        public RelayCommand TurnRightCommand
        {
            get
            {
                if (turnRightCommand == null)
                    turnRightCommand = new RelayCommand(ExecuteTurnRightCommand);
                return turnRightCommand;
            }
        }

        private void ExecuteTurnRightCommand()
        {
            var surface = GetSurface(xAngle + 90, true);

            CurrentSurface = surface;
        }

        #endregion

        #region TurnUpCommand

        private RelayCommand turnUpCommand;

        /// <summary>
        /// 获取 TurnUpCommand。
        /// </summary>
        public RelayCommand TurnUpCommand
        {
            get
            {
                if (turnUpCommand == null)
                    turnUpCommand = new RelayCommand(ExecuteTurnUpCommand);
                return turnUpCommand;
            }
        }

        private void ExecuteTurnUpCommand()
        {
            var surface = GetSurface(yAngle - 90, false);

            CurrentSurface = surface;

            //    TurnUp();
        }

        #endregion

        #region TurnDownCommand

        private RelayCommand turnDownCommand;

        /// <summary>
        /// 获取 TurnDownCommand。
        /// </summary>
        public RelayCommand TurnDownCommand
        {
            get
            {
                if (turnDownCommand == null)
                    turnDownCommand = new RelayCommand(ExecuteTurnDownCommand);
                return turnDownCommand;
            }
        }

        private void ExecuteTurnDownCommand()
        {
            var surface = GetSurface(yAngle + 90, false);

            CurrentSurface = surface;
            //TurnDown();
        }

        #endregion

        #endregion

        #region 私有方法

        private Surface GetSurface(double angle, bool isX)
        {
            Surface result = Surface.Up;
            if (isX)
            {
                if (angle == 0 || Math.Abs(angle) == 360)
                    result = Surface.Front;
                else if (angle == -90 || angle == 270)
                    result = Surface.Right;
                else if (angle == -180 || angle == 180)
                    result = Surface.Back;
                else if (angle == -270 || angle == 90)
                    result = Surface.Left;
            }
            else
            {
                if (angle == 0 || Math.Abs(angle) == 360)
                    result = Surface.Front;
                else if (angle == -90 || angle == 270)
                    result = Surface.Down;
                else if (angle == 180 || angle == -180)
                    result = Surface.Back;
                else if (angle == -270 || angle == 90)
                    result = Surface.Up;
            }
            return result;
        }

        private void Turn(double x, double y, double z, double fromAngle, double toAngle)
        {
            if (Children != null)
            {
                foreach (var item in Children)
                {
                    if (!(item is Viewport2DVisual3D))
                        continue;

                    Viewport2DVisual3D viewport2DVisual3D = item as Viewport2DVisual3D;
                    Transform3DGroup group = viewport2DVisual3D.Transform as Transform3DGroup;
                    if (group == null)
                    {
                        group = new Transform3DGroup();
                        viewport2DVisual3D.Transform = group;
                        RotateTransform3D transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 0), 0));

                        group.Children.Add(transform);
                    }

                    RotateTransform3D rotateTransform = group.Children[0] as RotateTransform3D;
                    rotateTransform.Rotation.SetValue(AxisAngleRotation3D.AxisProperty, new Vector3D(x, y, z));

                    if (rotateTransform != null)
                    {
                        DoubleAnimation animation = CreateAnimation(fromAngle, toAngle);

                        rotateTransform.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
                    }
                }
            }
        }

        private Surface TurnLeft()
        {
            Turn(0, 1, 0, xAngle, xAngle - 90);

            xAngle -= 90;
            if (xAngle <= -360)
                xAngle = 0;

            var result = GetSurface(xAngle, true);
            return result;
        }

        private Surface TurnRight()
        {
            Turn(0, 1, 0, xAngle, xAngle + 90);
            xAngle += 90;
            if (xAngle == 360)
                xAngle = 0;

            var result = GetSurface(xAngle, true);
            return result;
        }

        private Surface TurnUp()
        {
            Turn(1, 0, 0, yAngle, yAngle - 90);
            yAngle -= 90;
            if (yAngle == -360)
                yAngle = 0;

            var result = GetSurface(yAngle, false);
            return result;
        }

        private Surface TurnDown()
        {
            Turn(1, 0, 0, yAngle, yAngle + 90);
            yAngle += 90;
            if (yAngle == 360)
                yAngle = 0;

            var result = GetSurface(yAngle, false);
            return result;
        }


        /// <summary>
        /// 转换到指定面
        /// </summary>
        /// <param name="oldSurface">原来的面.</param>
        /// <param name="surface">要到的面.</param>
        private void Turn(Surface oldSurface, Surface surface)
        {
            //旋转方向
            bool toLeft = false;
            bool toRight = false;
            bool toUp = false;
            bool toDown = false;

            if ((oldSurface == Surface.Left && surface == Surface.Front) ||
                (oldSurface == Surface.Front && surface == Surface.Right) ||
                (oldSurface == Surface.Right && surface == Surface.Back) ||
                (oldSurface == Surface.Back && surface == Surface.Left) ||
                (oldSurface == Surface.Up && surface == Surface.Right) ||
                (oldSurface == Surface.Down && surface == Surface.Right))
                toLeft = true;

            if ((oldSurface == Surface.Left && surface == Surface.Back) ||
                (oldSurface == Surface.Front && surface == Surface.Left) ||
                (oldSurface == Surface.Right && surface == Surface.Front) ||
                (oldSurface == Surface.Back && surface == Surface.Right) ||
                (oldSurface == Surface.Up && surface == Surface.Right) ||
                (oldSurface == Surface.Down && surface == Surface.Right))
                toRight = true;

            if ((oldSurface == Surface.Left && surface == Surface.Back) ||
                (oldSurface == Surface.Front && surface == Surface.Down) ||
                (oldSurface == Surface.Right && surface == Surface.Front) ||
                (oldSurface == Surface.Back && surface == Surface.Up) ||
                (oldSurface == Surface.Up && surface == Surface.Front) ||
                (oldSurface == Surface.Down && surface == Surface.Back))
                toUp = true;

            if ((oldSurface == Surface.Left && surface == Surface.Back) ||
               (oldSurface == Surface.Front && surface == Surface.Up) ||
               (oldSurface == Surface.Right && surface == Surface.Front) ||
               (oldSurface == Surface.Back && surface == Surface.Down) ||
               (oldSurface == Surface.Up && surface == Surface.Back) ||
               (oldSurface == Surface.Down && surface == Surface.Up))
                toDown = true;

            if (toDown || toUp)
                xAngle = 0;
            else
                yAngle = 0;

            switch (surface)
            {
                case Surface.Back:
                    while (oldSurface != Surface.Back)
                    {
                        if (toLeft)
                            oldSurface = TurnLeft();
                        else if (toRight)
                            oldSurface = TurnRight();
                        else if (toUp)
                            oldSurface = TurnUp();
                        else
                            oldSurface = TurnDown();
                    }
                    break;
                case Surface.Front:
                    while (oldSurface != Surface.Front)
                    {
                        if (toLeft)
                            oldSurface = TurnLeft();
                        else if (toRight)
                            oldSurface = TurnRight();
                        else if (toUp)
                            oldSurface = TurnUp();
                        else
                            oldSurface = TurnDown();
                    }
                    break;
                case Surface.Down:
                    while (oldSurface != Surface.Down)
                    {
                        if (toUp)
                            oldSurface = TurnUp();
                        else
                            oldSurface = TurnDown();
                    }
                    break;
                case Surface.Left:
                    while (oldSurface != Surface.Left)
                    {
                        if (oldSurface == Surface.Back || oldSurface == Surface.Right)
                            oldSurface = TurnLeft();
                        else
                            oldSurface = TurnRight();
                    }
                    break;
                case Surface.Right:
                    while (oldSurface != Surface.Right)
                    {
                        if (oldSurface == Surface.Back)
                            oldSurface = TurnRight();
                        else
                            oldSurface = TurnLeft();
                    }
                    break;
                case Surface.Up:
                    while (oldSurface != Surface.Up)
                    {
                        if (toUp)
                            oldSurface = TurnUp();
                        else
                            oldSurface = TurnDown();
                    }
                    break;
            }
        }

        private DoubleAnimation CreateAnimation(double from, double to)
        {
            DoubleAnimation animation = new DoubleAnimation();

            animation.To = to;
            animation.From = from;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(Duration));

            animation.Freeze();

            return animation;
        }

        #endregion
    }
}

#region 坐标计算参考

/*
<Viewport3D x:Name="PART_Viewport3D" Height="312" Width="509">
                		<Viewport3D.Camera>
                			<PerspectiveCamera FieldOfView="45" Position="0,0,5" />
                		</Viewport3D.Camera>

                <!--背面-->
                		<Viewport2DVisual3D x:Name="back">                			
                			<Viewport2DVisual3D.Material>
                				<DiffuseMaterial Viewport2DVisual3D.IsVisualHostMaterial="True" Brush="White"/>
                			</Viewport2DVisual3D.Material>

                			<Viewport2DVisual3D.Geometry>
                				<MeshGeometry3D Positions="-1,-1,-1 1,-1,-1 1,1,-1 -1,1,-1" 
                					TriangleIndices="0,2,1  0,3,2" TextureCoordinates="0,0 1,0 1,1 0,1"/>
                			</Viewport2DVisual3D.Geometry>

                			<Grid>
                				<TextBox Text="背面" d:IsLocked="True"/>
                			</Grid>
                		</Viewport2DVisual3D>

                <!--正面-->
                		<Viewport2DVisual3D x:Name="front">                			
                			<Viewport2DVisual3D.Material>
                				<DiffuseMaterial Viewport2DVisual3D.IsVisualHostMaterial="True" Brush="White"/>
                			</Viewport2DVisual3D.Material>

                			<Viewport2DVisual3D.Geometry>
                				<MeshGeometry3D Positions="-1,-1,1 1,-1,1 1,1,1 -1,1,1" 
                					TriangleIndices="0,1,2  2,3,0" TextureCoordinates="0,1 1,1 1,0 0,0"/>
                			</Viewport2DVisual3D.Geometry>

                			<Grid>
                				<TextBox Text="正面"/>
                			</Grid>
                		</Viewport2DVisual3D>

                <!--左侧面-->
                		<Viewport2DVisual3D x:Name="left">                			
                			<Viewport2DVisual3D.Material>
                				<DiffuseMaterial Viewport2DVisual3D.IsVisualHostMaterial="True" Brush="White"/>
                			</Viewport2DVisual3D.Material>

                			<Viewport2DVisual3D.Geometry>
                				<MeshGeometry3D Positions="-1,-1,-1 -1,-1,1 -1,1,1 -1,1,-1" 
                					TriangleIndices="0,1,2  2,3,0" TextureCoordinates="0,1 1,1 1,0 0,0"/>
                			</Viewport2DVisual3D.Geometry>

                			<Grid>
                				<TextBox Text="左侧面"/>
                			</Grid>
                		</Viewport2DVisual3D>

                <!--右侧面-->
                		<Viewport2DVisual3D x:Name="right">                		
                			<Viewport2DVisual3D.Material>
                				<DiffuseMaterial Viewport2DVisual3D.IsVisualHostMaterial="True" Brush="White"/>
                			</Viewport2DVisual3D.Material>

                			<Viewport2DVisual3D.Geometry>
                				<MeshGeometry3D Positions="1,-1,-1 1,-1,1 1,1,1 1,1,-1" 
                					TriangleIndices="0,3,2 0,2,1" TextureCoordinates="1,1 0,1 0,0 1,0"/>
                			</Viewport2DVisual3D.Geometry>

                			<Grid>
                				<TextBox Text="右侧面"/>
                			</Grid>
                		</Viewport2DVisual3D>

                <!--顶部-->
                		<Viewport2DVisual3D x:Name="top">                			
                			<Viewport2DVisual3D.Material>
                				<DiffuseMaterial Viewport2DVisual3D.IsVisualHostMaterial="True" Brush="White"/>
                			</Viewport2DVisual3D.Material>

                			<Viewport2DVisual3D.Geometry>
                				<MeshGeometry3D Positions="-1,1,1 1,1,1 1,1,-1 -1,1,-1" 
                					TriangleIndices="0,1,2  2,3,0" TextureCoordinates="0,1 1,1 1,0 0,0"/>
                			</Viewport2DVisual3D.Geometry>

                			<Grid>
                				<TextBox Text="顶部"/>
                			</Grid>
                		</Viewport2DVisual3D>

                <!--底部-->
                		<Viewport2DVisual3D x:Name="bottom">                			
                			<Viewport2DVisual3D.Material>
                				<DiffuseMaterial Viewport2DVisual3D.IsVisualHostMaterial="True" Brush="White"/>
                			</Viewport2DVisual3D.Material>

                			<Viewport2DVisual3D.Geometry>
                				<MeshGeometry3D Positions="-1,-1,1 1,-1,1 1,-1,-1 -1,-1,-1" 
                					TriangleIndices="0,3,2  2,1,0" TextureCoordinates="0,0 1,0 1,1 0,1"/>
                			</Viewport2DVisual3D.Geometry>

                			<Grid>
                				<TextBox Text="底部"/>
                			</Grid>
                		</Viewport2DVisual3D>

                <!--光源-->
                		<ModelVisual3D>
                			<ModelVisual3D.Content>
                				<AmbientLight Color="White" />
                			</ModelVisual3D.Content>
                		</ModelVisual3D>
                		<ModelVisual3D>
                			<ModelVisual3D.Content>
                				<DirectionalLight Color="Red" Direction="0,0,1" />

                			</ModelVisual3D.Content>
                		</ModelVisual3D>
                	</Viewport3D>

 * */
#endregion