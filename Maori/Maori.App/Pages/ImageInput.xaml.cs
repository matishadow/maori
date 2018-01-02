using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Maori.App.Models;
using Maori.Implementations;
using Microsoft.Win32;

namespace Maori.App.Pages
{
    /// <summary>
    /// Interaction logic for ImageInput.xaml
    /// </summary>
    public partial class ImageInput : UserControl
    {
        public ImageInput()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog
            {
                Title = "Wybierz obrazek",
                Filter = "Dowolny format graficzny|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };
            if (op.ShowDialog() != true) return;

            InputImage.Source = new BitmapImage(new Uri(op.FileName));
            MaoriViewModel.OriginalImage = MaoriBitmap.FromWpfImage((BitmapSource) InputImage.Source, new ColorSpaceConverter());
            MaoriViewModel.ProcessedImage = null;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            InputImage.Source = null;
            MaoriViewModel.OriginalImage = null;
            MaoriViewModel.ProcessedImage = null;
        }
    }
}
