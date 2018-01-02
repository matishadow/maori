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
using Maori.Implementations;

namespace Maori.App.Pages
{
    /// <summary>
    /// Interaction logic for Result1.xaml
    /// </summary>
    public partial class Result3 : UserControl
    {
        public Result3()
        {
            InitializeComponent();
            var img = new MaoriBitmap(Properties.Resources.method3, new ColorSpaceConverter());
            Result1Image.Source = img.ToWpfImage();
        }
    }
}
