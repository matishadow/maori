using System;
using System.Collections.Generic;
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
using FirstFloor.ModernUI.Windows;
using Maori.App.Models;
using Maori.Implementations;
using FragmentNavigationEventArgs = FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs;
using NavigatingCancelEventArgs = FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs;
using NavigationEventArgs = FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs;

namespace Maori.App.Pages
{
    /// <summary>
    /// Interaction logic for Binary.xaml
    /// </summary>
    public partial class Binary : UserControl, IContent
    {
        public Binary()
        {
            InitializeComponent();

            Dispatcher.Invoke(Reload);
        }

        public void Reload()
        {
            if (MaoriViewModel.ProcessedImage != null)
                BinaryImage.Source = MaoriViewModel.ProcessedImage.ToWpfImage();
            else if (MaoriViewModel.OriginalImage != null)
                BinaryImage.Source = MaoriViewModel.OriginalImage.ToWpfImage();
        }

        private void BtnBinary_OnClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(ConvertToBinary);
        }

        private void ConvertToBinary()
        {
            MaoriBitmap image;
            if (MaoriViewModel.ProcessedImage != null)
                image = MaoriViewModel.ProcessedImage;
            else if (MaoriViewModel.OriginalImage != null)
            {
                MaoriViewModel.ProcessedImage =
                    new MaoriBitmap(MaoriViewModel.OriginalImage.Bitmap, new ColorSpaceConverter());
                image = MaoriViewModel.ProcessedImage;
            }
            else return;

            double threshold = double.TryParse(ThresholdTextBox.Text, out threshold) ? threshold : 0.5;
            image.ConvertToBinary(threshold);
            BinaryImage.Source = image.ToWpfImage();
        }

        public void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
            Dispatcher.Invoke(Reload);
        }

        public void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
        }

        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(Reset);
        }

        private void Reset()
        {
            MaoriViewModel.ProcessedImage =
                new MaoriBitmap(MaoriViewModel.OriginalImage.Bitmap, new ColorSpaceConverter());
            BinaryImage.Source = MaoriViewModel.ProcessedImage.ToWpfImage();
        }
    }
}
