using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PathFindingAlgorithmsDemo.Algorithms;
using PathFindingAlgorithmsDemo.MapGeneration;

namespace PathFindingAlgorithmsDemo
{
    public class MainWindowViewModel : ObservableObject
    {
        private const int _width = 830;
        private const int _height = 600;
        private double _canvasActualWidth;
        private double _canvasActualHeight;
        private BitmapImage _canvas;
        private readonly DirectBitmap _bitmap;
        private ColorSchemes _colorScheme;
        private PathfindingAlgorithms _pathfindingAlgorithm;
        private readonly DispatcherTimer _timer;
        private bool _mouseDown;
        private Point _previousPosition;
        private Point _currentPosition;
        private readonly NodeGrid _grid;
        private readonly List<Node> _calculatedPath;
        private HashSet<Node> _visitedNodes;
        private NodeType _selectedNodeType;
        private readonly DispatcherTimer _pathfindingTimer;
        private OperatingModes _selectedOperatingMode;
        private int _currentNewNode;
        private int _currentNewPath;
        private List<NewNodeInfo> _newNodes;
        private List<NewPathInfo> _newPaths;
        private bool _isRunning;
        private readonly List<Node> _calculatedPathCopy;
        private readonly HashSet<Node> _visitedNodesCopy;

        public MainWindowViewModel()
        {
            ClearCommand = new RelayCommand(ClearField);
            MouseMoveCommand = new RelayCommand<MouseEventArgs>(MouseMove);
            MouseDownCommand = new RelayCommand<MouseEventArgs>(MouseDown);
            MouseUpCommand = new RelayCommand<MouseEventArgs>(MouseUp);
            GenerateMapCommand = new RelayCommand(GenerateMap);
            StartAlgorithmCommand = new RelayCommand(StartAlgorithm);
            StopAlgorithmCommand = new RelayCommand(StopAlgorithm);

            PathfindingAlgorithmsList = new ObservableCollection<PathfindingAlgorithms>
            {
                PathfindingAlgorithms.AStar,
                PathfindingAlgorithms.BreadthFirstSearch,
                PathfindingAlgorithms.Dijkstra,
                PathfindingAlgorithms.GreedyBestFirstSearch
            };

            _bitmap = new DirectBitmap(_width, _height);

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _timer.Tick += TimerTick;

            var gridWidth = _width / Node.Size;
            var gridHeight = _height / Node.Size;
            _grid = new NodeGrid(gridWidth, gridHeight);
            _grid.Start = _grid[0, 0];
            _grid.End = _grid[0, 1];

            _calculatedPath = new List<Node>();
            _visitedNodes = new HashSet<Node>();

            _mouseDown = false;
            _previousPosition = new Point(0, 0);
            _currentPosition = new Point(0, 0);

            SelectedPathfindingAlgorithm = PathfindingAlgorithms.AStar;
            SelectedColorScheme = ColorSchemes.Blue;
            SelectedNodeType = NodeType.Wall;
            SelectedOperatingMode = OperatingModes.AllAtOnce;

            _pathfindingTimer = new DispatcherTimer();
            _pathfindingTimer.Interval = new TimeSpan(0, 0, 0, 0, 0);
            _pathfindingTimer.Tick += PathfindingTimerTick;

            _currentNewNode = 0;
            _currentNewPath = 0;
            _newNodes = new List<NewNodeInfo>();
            _newPaths = new List<NewPathInfo>();

            _calculatedPathCopy = new List<Node>();
            _visitedNodesCopy = new HashSet<Node>();

            IsRunning = false;

            Render();
        }

        public ICommand ClearCommand { get; }
        public ICommand MouseMoveCommand { get; }
        public ICommand MouseDownCommand { get; }
        public ICommand MouseUpCommand { get; }
        public ICommand GenerateMapCommand { get; }
        public ICommand StartAlgorithmCommand { get; }
        public ICommand StopAlgorithmCommand { get; }
        public ObservableCollection<PathfindingAlgorithms> PathfindingAlgorithmsList { get; }
        public bool IsStepByStepMode => SelectedOperatingMode == OperatingModes.StepByStep;
        public bool IsNotRunning => !IsRunning;

        public ColorSchemes SelectedColorScheme
        {
            get => _colorScheme;
            set
            {
                SetProperty(ref _colorScheme, value);

                Render();
            }
        }

        public PathfindingAlgorithms SelectedPathfindingAlgorithm
        {
            get => _pathfindingAlgorithm;
            set
            {
                SetProperty(ref _pathfindingAlgorithm, value);

                switch (SelectedOperatingMode)
                {
                    case OperatingModes.StepByStep:
                        break;

                    case OperatingModes.AllAtOnce:
                        CalculatePath();
                        Render();
                        break;
                }
            }
        }

        public NodeType SelectedNodeType
        {
            get => _selectedNodeType;
            set => SetProperty(ref _selectedNodeType, value);
        }

        public OperatingModes SelectedOperatingMode
        {
            get => _selectedOperatingMode;
            set
            {
                SetProperty(ref _selectedOperatingMode, value);
                OnPropertyChanged(nameof(IsStepByStepMode));

                switch (SelectedOperatingMode)
                {
                    case OperatingModes.StepByStep:
                        _timer.Stop();
                        ResetPath();
                        Render();
                        break;

                    case OperatingModes.AllAtOnce:
                        CalculatePath();
                        Render();
                        break;
                }
            }
        }

        public BitmapImage Canvas
        {
            get => _canvas;
            private set => SetProperty(ref _canvas, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                SetProperty(ref _isRunning, value);
                OnPropertyChanged(nameof(IsNotRunning));
            }
        }

        public void SetActualCanvasSize(double width, double height)
        {
            _canvasActualWidth = width;
            _canvasActualHeight = height;
        }

        private void Render()
        {
            var palette = ColorPalettes.GetPalette(SelectedColorScheme);

            _bitmap.Clear(palette[FieldElements.Default]);

            for (int i = 0; i < _grid.Width; i++)
            {
                for (int j = 0; j < _grid.Height; j++)
                {
                    if (!_grid[i, j].IsWalkable)
                    {
                        _bitmap.FillRectangle(_grid[i, j].X * Node.Size - 1, _grid[i, j].Y * Node.Size - 1, Node.Size - 1, Node.Size - 1, palette[FieldElements.Wall]);
                    }
                    else
                    if (_grid[i, j].IsExpensive)
                    {
                        _bitmap.BlendRectangle(_grid[i, j].X * Node.Size - 1, _grid[i, j].Y * Node.Size - 1, Node.Size - 1, Node.Size - 1, palette[FieldElements.Expensive]);
                    }
                }
            }

            if (!_mouseDown)
            {
                foreach (var node in _visitedNodes)
                {
                    _bitmap.FillRectangle(node.X * Node.Size - 1, node.Y * Node.Size - 1, Node.Size - 1, Node.Size - 1, palette[FieldElements.Visited]);

                    if (node.IsExpensive)
                    {
                        _bitmap.BlendRectangle(node.X * Node.Size - 1, node.Y * Node.Size - 1, Node.Size - 1, Node.Size - 1, palette[FieldElements.Expensive]);
                    }
                }

                foreach (var node in _calculatedPath)
                {
                    _bitmap.FillRectangle(node.X * Node.Size - 1, node.Y * Node.Size - 1, Node.Size - 1, Node.Size - 1, palette[FieldElements.Path]);

                    if (node.IsExpensive)
                    {
                        _bitmap.BlendRectangle(node.X * Node.Size - 1, node.Y * Node.Size - 1, Node.Size - 1, Node.Size - 1, palette[FieldElements.Expensive]);
                    }
                }
            }

            _bitmap.FillRectangle(_grid.Start.X * Node.Size - 1, _grid.Start.Y * Node.Size - 1, Node.Size - 1, Node.Size - 1, palette[FieldElements.Start]);
            _bitmap.FillRectangle(_grid.End.X * Node.Size - 1, _grid.End.Y * Node.Size - 1, Node.Size - 1, Node.Size - 1, palette[FieldElements.Finish]);

            UpdateCanvas();
        }

        private void UpdateCanvas()
        {
            using MemoryStream memory = new MemoryStream();
            _bitmap.Bitmap.Save(memory, ImageFormat.Bmp);
            memory.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            Canvas = bitmapImage;
        }

        private Point GetNodePosition(System.Windows.Point mousePosition)
        {
            var xScale = mousePosition.X / _canvasActualWidth;
            var yScale = mousePosition.Y / _canvasActualHeight;
            var x = xScale * _width / Node.Size;
            var y = yScale * _height / Node.Size;

            return new Point((int)x, (int)y);
        }

        private void MouseMove(MouseEventArgs e)
        {
            if (IsRunning)
            {
                return;
            }

            var position = GetNodePosition(e.GetPosition((System.Windows.IInputElement)e.Source));

            _previousPosition = _currentPosition;
            _currentPosition = position;

            if (_mouseDown)
            {
                DrawNodes();
            }
            else
            {
                if ((position.X != _grid.End.X || position.Y != _grid.End.Y) &&
                    SelectedOperatingMode == OperatingModes.AllAtOnce)
                {
                    RestartTimer();
                }
            }

            _grid.End = _grid[position.X, position.Y];

            Render();
        }

        private void MouseDown(MouseEventArgs e)
        {
            if (IsRunning)
            {
                return;
            }

            var position = GetNodePosition(e.GetPosition((System.Windows.IInputElement)e.Source));

            switch (e.LeftButton)
            {
                case MouseButtonState.Pressed:
                    _mouseDown = true;

                    switch (SelectedNodeType)
                    {
                        case NodeType.Default:
                            _grid[position.X, position.Y].SetToDefault();
                            break;

                        case NodeType.Expensive:
                            _grid[position.X, position.Y].SetToExpensive();
                            break;

                        case NodeType.Wall:
                            _grid[position.X, position.Y].SetToWall();
                            break;
                    }
                    break;
            }

            switch (e.RightButton)
            {
                case MouseButtonState.Pressed:
                    if (_grid[position.X, position.Y].IsWalkable && !_mouseDown)
                    {
                        _grid.Start = _grid[position.X, position.Y];
                    }
                    break;
            }

            Render();
        }

        private void MouseUp(MouseEventArgs e)
        {
            if (IsRunning)
            {
                return;
            }

            switch (e.LeftButton)
            {
                case MouseButtonState.Released:
                    _mouseDown = false;
                    break;
            }

            Render();
        }

        private void DrawNodes()
        {
            var x0 = _previousPosition.X;
            var y0 = _previousPosition.Y;
            var x1 = _currentPosition.X;
            var y1 = _currentPosition.Y;

            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);
            var sx = (x0 < x1) ? 1 : -1;
            var sy = (y0 < y1) ? 1 : -1;
            var err = dx - dy;

            while (true)
            {
                switch (SelectedNodeType)
                {
                    case NodeType.Default:
                        _grid[x0, y0].SetToDefault();
                        break;

                    case NodeType.Expensive:
                        _grid[x0, y0].SetToExpensive();
                        break;

                    case NodeType.Wall:
                        _grid[x0, y0].SetToWall();
                        break;
                }

                if ((x0 == x1) && (y0 == y1))
                {
                    break;
                }

                var e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        private void RestartTimer()
        {
            _timer.Stop();
            _timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _timer.Stop();

            CalculatePath();
            Render();
        }

        private void ResetPath()
        {
            _calculatedPath.Clear();
            _visitedNodes.Clear();
            _grid.ResetCosts();
        }

        private void CalculatePath()
        {
            ResetPath();

            switch (SelectedPathfindingAlgorithm)
            {
                case PathfindingAlgorithms.AStar:
                    _calculatedPath.AddRange(_grid.AStartFindPath(ref _visitedNodes));
                    break;

                case PathfindingAlgorithms.BreadthFirstSearch:
                    _calculatedPath.AddRange(_grid.BreadthFirstSearchFindPath(ref _visitedNodes));
                    break;

                case PathfindingAlgorithms.Dijkstra:
                    _calculatedPath.AddRange(_grid.DijkstraFindPath(ref _visitedNodes));
                    break;

                case PathfindingAlgorithms.GreedyBestFirstSearch:
                    _calculatedPath.AddRange(_grid.GreedyBestFirstSearchFindPath(ref _visitedNodes));
                    break;
            }
        }

        private void ClearField()
        {
            _calculatedPath.Clear();
            _visitedNodes.Clear();
            _grid.Clear();

            Render();
        }

        private void GenerateMap()
        {
            _calculatedPath.Clear();
            _visitedNodes.Clear();

            _grid.Clear();

            var iterationsNumber = RandomUtils.Next(1, 7);
            var density = RandomUtils.Next(40, 65);

            var mapGenerator = new MapGenerator(_grid.Width, _grid.Height)
                .SetSeed(RandomUtils.GetRandomString(20))
                .GenerateNoise(density)
                .PerformCellularAutomata(iterationsNumber);

            _grid.SetWallsFromMap(mapGenerator.Map);

            Render();
        }

        private void StopPathfindingTimer()
        {
            _pathfindingTimer.Stop();

            _calculatedPath.Clear();
            _calculatedPath.AddRange(_calculatedPathCopy);

            _visitedNodes.Clear();
            foreach (var visitedNode in _visitedNodesCopy)
            {
                _visitedNodes.Add(visitedNode);
            }

            IsRunning = false;

            Render();
        }

        private void PathfindingTimerTick(object sender, EventArgs e)
        {
            if (_currentNewNode == _newNodes.Count &&
                _currentNewPath == _newPaths.Count)
            {
                StopPathfindingTimer();
                return;
            }

            if (_currentNewNode < _newNodes.Count)
            {
                _visitedNodes.Add(_newNodes[_currentNewNode].Node);

                _currentNewNode += 1;
            }
            
            if (_currentNewPath < _newPaths.Count)
            {
                _calculatedPath.Clear();
                _calculatedPath.AddRange(_newPaths[_currentNewPath].Path);

                _currentNewPath += 1;
            }

            Render();
        }

        private void StartAlgorithm()
        {
            IsRunning = true;

            ResetPath();

            _currentNewNode = 0;
            _currentNewPath = 0;
            _newNodes.Clear();
            _newPaths.Clear();
            _calculatedPathCopy.Clear();
            _visitedNodesCopy.Clear();

            switch (SelectedPathfindingAlgorithm)
            {
                case PathfindingAlgorithms.AStar:
                    _calculatedPath.AddRange(_grid.AStartFindPath(ref _visitedNodes, ref _newNodes, ref _newPaths));
                    break;

                case PathfindingAlgorithms.BreadthFirstSearch:
                    _calculatedPath.AddRange(_grid.BreadthFirstSearchFindPath(ref _visitedNodes, ref _newNodes, ref _newPaths));
                    break;

                case PathfindingAlgorithms.Dijkstra:
                    _calculatedPath.AddRange(_grid.DijkstraFindPath(ref _visitedNodes, ref _newNodes, ref _newPaths));
                    break;

                case PathfindingAlgorithms.GreedyBestFirstSearch:
                    _calculatedPath.AddRange(_grid.GreedyBestFirstSearchFindPath(ref _visitedNodes, ref _newNodes, ref _newPaths));
                    break;
            }

            _calculatedPathCopy.AddRange(_calculatedPath);
            _calculatedPath.Clear();

            foreach (var visitedNode in _visitedNodes)
            {
                _visitedNodesCopy.Add(visitedNode);
            }
            _visitedNodes.Clear();

            _pathfindingTimer.Start();
        }

        private void StopAlgorithm()
        {
            StopPathfindingTimer();
        }
    }
}
