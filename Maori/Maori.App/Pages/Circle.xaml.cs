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
    /// Interaction logic for Grayscale.xaml
    /// </summary>
    public partial class Circle : UserControl, IContent
    {
        public Circle()
        {
            InitializeComponent();

            Dispatcher.Invoke(Reload);
        }

        public void Reload()
        {
            if (MaoriViewModel.ProcessedImage != null)
                CircledImage.Source = MaoriViewModel.ProcessedImage.ToWpfImage();
            else if (MaoriViewModel.OriginalImage != null)
                CircledImage.Source = MaoriViewModel.OriginalImage.ToWpfImage();
        }

        private void btnCircles_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(DetectCircles);
        }

        private void DetectCircles()
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

            int minR = int.TryParse(MinR.Text, out minR) ? minR : 10;
            int maxR = int.TryParse(MaxR.Text, out maxR) ? maxR: 100;

            image.DetectCircle(minR, maxR);
            CircledImage.Source = image.ToWpfImage();
        }

        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(Reset);
        }

        private void Reset()
        {
            MaoriViewModel.ProcessedImage =
                new MaoriBitmap(MaoriViewModel.OriginalImage.Bitmap, new ColorSpaceConverter());
            CircledImage.Source = MaoriViewModel.ProcessedImage.ToWpfImage();
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
    }
}
