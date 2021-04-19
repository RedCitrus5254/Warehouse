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
using System.Windows.Shapes;

namespace Warehouse.Views
{
    /// <summary>
    /// Логика взаимодействия для CreateNodeWindow.xaml
    /// </summary>
    public partial class CreateNodeWindow : Window
    {
        public CreateNodeWindow()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string NodeName
        {
            get { return NodeNameBox.Text; }
        }

    }
}
