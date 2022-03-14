﻿using System;
using System.Diagnostics;
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

namespace PathFindingAlgorithmsDemo
{
    public class MainWindowViewModel : ObservableObject
    {
        private const int _width = 800;
        private const int _height = 600;
        private double _canvasActualWidth;
        private double _canvasActualHeight;
        private BitmapImage _canvas;
        private readonly DirectBitmap _bitmap;
        private ColorSchemes _colorScheme;
        private PathfindingAlgorithms _pathfindingAlgorithm;
        private readonly DispatcherTimer _timer;
        private string _message;
        private bool _mouseDown;
        private readonly NodeGrid _grid;
        private readonly List<Node> _calculatedPath;

        public MainWindowViewModel()
        {
            ClearCommand = new RelayCommand(ClearWalls);
            MouseMoveCommand = new RelayCommand<MouseEventArgs>(MouseMove);
            MouseDownCommand = new RelayCommand<MouseEventArgs>(MouseDown);
            MouseUpCommand = new RelayCommand<MouseEventArgs>(MouseUp);

            PathfindingAlgorithmsList = new ObservableCollection<PathfindingAlgorithms>
            {
                PathfindingAlgorithms.AStar
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

            SelectedPathfindingAlgorithm = PathfindingAlgorithms.AStar;

            Render();
        }

        public ICommand ClearCommand { get; }
        public ICommand MouseMoveCommand { get; }
        public ICommand MouseDownCommand { get; }
        public ICommand MouseUpCommand { get; }
        public ObservableCollection<PathfindingAlgorithms> PathfindingAlgorithmsList { get; }

        public string Message
        {
            get => _message;
            private set => SetProperty(ref _message, value);
        }

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

                Debug.WriteLine("Selected Algorithm: " + SelectedPathfindingAlgorithm);
            }
        }

        public BitmapImage Canvas
        {
            get => _canvas;
            private set => SetProperty(ref _canvas, value);
        }

        public void SetActualCanvasSize(double width, double height)
        {
            _canvasActualWidth = width;
            _canvasActualHeight = height;
        }

        private void Render()
        {
            var palette = ColorPalettes.GetPalette(SelectedColorScheme);

            _bitmap.Clear(palette[FieldElements.Empty]);

            for (int i = 0; i < _grid.Width; i++)
            {
                for (int j = 0; j < _grid.Height; j++)
                {
                    if (!_grid[i, j].IsWalkable)
                    {
                        _bitmap.FillRectangle(_grid[i, j].X * Node.Size, _grid[i, j].Y * Node.Size, Node.Size, Node.Size, palette[FieldElements.Wall]);
                    }
                }
            }

            if (!_mouseDown)
            {
                foreach (var node in _calculatedPath)
                {
                    _bitmap.FillRectangle(node.X * Node.Size, node.Y * Node.Size, Node.Size, Node.Size, palette[FieldElements.Path]);
                }
            }

            _bitmap.FillRectangle(_grid.Start.X * Node.Size, _grid.Start.Y * Node.Size, Node.Size, Node.Size, palette[FieldElements.Start]);
            _bitmap.FillRectangle(_grid.End.X * Node.Size, _grid.End.Y * Node.Size, Node.Size, Node.Size, palette[FieldElements.Finish]);

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
            var position = GetNodePosition(e.GetPosition((System.Windows.IInputElement)e.Source));

            if (_mouseDown)
            {
                _grid[position.X, position.Y].IsWalkable = false;
            }
            else
            {
                if (position.X != _grid.End.X || position.Y != _grid.End.Y)
                {
                    RestartTimer();
                }
            }

            _grid.End = _grid[position.X, position.Y];

            Render();
        }

        private void MouseDown(MouseEventArgs e)
        {
            var position = GetNodePosition(e.GetPosition((System.Windows.IInputElement)e.Source));

            switch (e.LeftButton)
            {
                case MouseButtonState.Pressed:
                    _mouseDown = true;
                    _grid[position.X, position.Y].IsWalkable = false;
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
            switch (e.LeftButton)
            {
                case MouseButtonState.Released:
                    _mouseDown = false;
                    break;
            }

            Render();
        }

        private void RestartTimer()
        {
            _timer.Stop();
            _timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            _calculatedPath.Clear();
            _grid.ResetCosts();

            switch (SelectedPathfindingAlgorithm)
            {
                case PathfindingAlgorithms.AStar:
                    _calculatedPath.AddRange(_grid.AStartFindPath());
                    break;
                default:
                    break;
            }

            Render();
        }

        private void ClearWalls()
        {
            _grid.ClearWalls();

            Render();
        }
    }
}
