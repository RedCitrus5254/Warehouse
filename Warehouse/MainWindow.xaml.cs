using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Warehouse.Model;
using Warehouse.Model.Data;
using Warehouse.ViewModels;

namespace Warehouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //поиск узла, по которому прошёл клик
            TreeViewItem ClickedTreeViewItem = new TreeViewItem(); 

            UIElement ClickedItem = VisualTreeHelper.GetParent(e.OriginalSource as UIElement) as UIElement;

            while ((ClickedItem != null) && !(ClickedItem is TreeViewItem))
            {
                ClickedItem = VisualTreeHelper.GetParent(ClickedItem) as UIElement;
            }

            ClickedTreeViewItem = ClickedItem as TreeViewItem;

            var viewmodel = (MainViewVM)DataContext;
            if (ClickedTreeViewItem == null)
            {
                viewmodel.CurrentNode = null;
            }
            else
            {
                ClickedTreeViewItem.IsSelected = true;
                ClickedTreeViewItem.Focus();

                //выбор текущего узла
                viewmodel.CurrentNode = (Node)ClickedTreeViewItem.DataContext; 
            }

            //вызов элемента для выбора команд узлов
            var point = e.GetPosition(this);
            Canvas.SetLeft(NodeCreator, point.X-5);
            Canvas.SetTop(NodeCreator, point.Y-5);
            
            NodeCreator.Visibility = Visibility.Visible;
        }

        private void ClassifierTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var viewmodel = (MainViewVM)DataContext;
            viewmodel.CurrentNode = (Node)ClassifierTreeView.SelectedItem;
        }

        private void GoodsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewmodel = (MainViewVM)DataContext;
            viewmodel.CurrentGood = (Good)GoodsListView.SelectedItem;
        }

        /// <summary>
        /// вызов элемента для команд товаров
        /// </summary>
        private void ListViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(GoodsListView);
            Canvas.SetLeft(GoodCommands, point.X);
            Canvas.SetTop(GoodCommands, point.Y+15);

            GoodCommands.Visibility = Visibility.Visible;
        }
    }
}
