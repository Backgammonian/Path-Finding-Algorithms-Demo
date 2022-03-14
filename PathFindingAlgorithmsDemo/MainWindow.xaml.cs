using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PathFindingAlgorithmsDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((MainWindowViewModel)DataContext).SetActualCanvasSize(e.NewSize.Width, e.NewSize.Height);
        }
    }
}
