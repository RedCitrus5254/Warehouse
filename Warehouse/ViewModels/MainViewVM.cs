using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Model;
using Warehouse.Model.BL;
using Warehouse.Model.Data;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using Prism.Commands;
using Warehouse.Views;
using System.ComponentModel;
using System.Windows;

namespace Warehouse.ViewModels
{
    class MainViewVM : BindableBase
    {
        private WarehouseManager warehouseManager;

        public MenuVM MenuVM { get; set; }

        public ObservableCollection<Node> Nodes { get; }

        /// <summary>
        /// управляет доступностью к кнопке для изменения имени узла
        /// </summary>
        public bool CanChangeName => changeNodeNameField != null && currentNode != null && !changeNodeNameField.Equals(currentNode.Name) ? true : false;
            

        private string changeNodeNameField;
        public string ChangeNodeNameField
        {
            get
            {
                return changeNodeNameField;
            }
            set
            {
                changeNodeNameField = value;

                RaisePropertyChanged(nameof(CanChangeName));
                RaisePropertyChanged(nameof(ChangeNodeNameField));
            }
        }

        private Node currentNode;
        public Node CurrentNode 
        { 
            get 
            {
                return currentNode;
            }
            set
            {
                currentNode = value;

                ChangeNodeNameField = value?.Name;

                RaisePropertyChanged(nameof(IsCurrentNodeNullVisibility));
                RaisePropertyChanged(nameof(CurrentNode));
            }
        }

        /// <summary>
        /// кнопка создания товара видна, когда выбран какой-то узел
        /// </summary>
        public Visibility IsCurrentNodeNullVisibility => CurrentNode == null ? Visibility.Collapsed : Visibility.Visible;


        private Good currentGood;
        public Good CurrentGood
        {
            get
            {
                return currentGood;
            }
            set
            {
                currentGood = value;
                RaisePropertyChanged(nameof(CurrentGood));
            }
        }

        public MainViewVM()
        {
            warehouseManager = new WarehouseManager();
            Nodes = CreateNodesCopy(warehouseManager.Nodes, null);
            SubscribeAllNodesOnChangeCollection(warehouseManager.Nodes, Nodes, null);

            MenuVM = new MenuVM(warehouseManager);

        }

        /// <summary>
        /// подписка на изменение коллекций для всех узлов
        /// </summary>
        /// <param name="nodes">узлы модели</param>
        /// <param name="localNodes">узлы вьюмодели</param>
        /// <param name="parent">родительский узел</param>
        private void SubscribeAllNodesOnChangeCollection(IEnumerable<Node> nodes, ObservableCollection<Node> localNodes, Node parent)
        {
            ((INotifyCollectionChanged)nodes).CollectionChanged += (s, a) =>
            {
                if (a.NewItems?.Count == 1)
                {
                    var node = CreateNodesCopy(new List<Node> { (Node)a.NewItems[0] }, parent)[0];
                    localNodes.Add(node);
                    ((INotifyPropertyChanged)(Node)a.NewItems[0]).PropertyChanged += (s, a) =>
                     {
                         node.Name = ((Node)s).Name;
                     };

                    SubscribeAllNodesOnChangeCollection(((Node)a.NewItems[0]).Nodes, node.Nodes, node);
                }
                if (a.OldItems?.Count == 1) localNodes.Remove(localNodes.First(x => x.Name.Equals(((Node)a.OldItems[0]).Name)));
            };
            for(int i = 0; i < nodes.Count(); i++)
            {
                SubscribeAllNodesOnChangeCollection(nodes.ToList()[i].Nodes, localNodes[i].Nodes, localNodes[i]);
            }
            foreach(var node in nodes)
            {
                ((INotifyPropertyChanged)node).PropertyChanged += (s, a) =>
                {
                    var index = nodes.ToList().IndexOf(node);
                    localNodes[index].Name = ((Node)s).Name;
                };
            }
        }

        /// <summary>
        /// копирование классификатора для того, чтобы не оказывать прямого влияния на модель
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private ObservableCollection<Node> CreateNodesCopy(IEnumerable<Node> nodes, Node parent)
        {
            return new ObservableCollection<Node>(nodes.Select(x =>
            {
                var node = new Node
                {
                    Name = x.Name,
                    Parent = parent,
                    Goods = x.Goods
                };
                node.Nodes = CreateNodesCopy(x.Nodes, node);
                return node;
            }));
        }

        
        /// <summary>
        /// команда создания узла
        /// </summary>
        public DelegateCommand CreateNode => new DelegateCommand(PerformCreateNode);
        private void PerformCreateNode()
        {
            var nodeIndexes = GetNodeIndexes(CurrentNode);

            var createNodeWindow = new CreateNodeWindow();
            
            if (createNodeWindow.ShowDialog() == true)
            {
                if (string.IsNullOrEmpty(createNodeWindow.NodeName))
                {
                    MessageBox.Show("Имя узла должно быть непустым");
                    return;
                }

                if (currentNode != null)
                {
                    if (CurrentNode.Nodes.FirstOrDefault(x => x.Name.Equals(createNodeWindow.NodeName)) == null)
                    {
                        warehouseManager.CreateNode(nodeIndexes, createNodeWindow.NodeName);
                    }
                    else
                    {
                        MessageBox.Show("Узел с таким именем уже есть");
                    }
                }
                else
                {
                    if(Nodes.FirstOrDefault(x => x.Name.Equals(createNodeWindow.NodeName)) == null)
                    {
                        warehouseManager.CreateNode(nodeIndexes, createNodeWindow.NodeName);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Узел с таким именем уже есть");
                    }
                }
            }

            
        }
        /// <summary>
        /// команда обновления узла
        /// </summary>
        public DelegateCommand UpdateNodeName => new DelegateCommand(PerformUpdateNodeName);

        private void PerformUpdateNodeName()
        {
            var indexes = GetNodeIndexes(CurrentNode);

            warehouseManager.UpdateNodeName(indexes, ChangeNodeNameField);

            RaisePropertyChanged(nameof(CanChangeName));
        }

        /// <summary>
        /// команда удаления узла
        /// </summary>
        public DelegateCommand DeleteNode => new DelegateCommand(PerformDeleteNode);

        private void PerformDeleteNode()
        {
            if (currentNode.Nodes.Count > 0 || currentNode.Goods.Count > 0)
            {
                MessageBox.Show("Этот узел удалять нельзя -- он содержит товары или другие узлы");
                return;
            }
            var indexes = GetNodeIndexes(CurrentNode);

            warehouseManager.DeleteNode(indexes);
        }

        /// <summary>
        /// команда обновления информации о товаре
        /// </summary>
        public DelegateCommand UpdateGood => new DelegateCommand(PerformUpdateGood);

        private void PerformUpdateGood()
        {
            GoodCard goodCard = new GoodCard();
            var viewModel = (GoodCardVM)goodCard.DataContext;

            var nodeIndexes = GetNodeIndexes(CurrentNode);

            var goodIndex = GetGoodIndex(CurrentGood, CurrentNode);

            viewModel.PutData(warehouseManager, CurrentGood, nodeIndexes, goodIndex);

            goodCard.Show();
        }

        /// <summary>
        /// команда создания товара
        /// </summary>
        public DelegateCommand CreateGood => new DelegateCommand(PerformCreateGood);

        private void PerformCreateGood()
        {
            GoodCard goodCard = new GoodCard();
            var viewModel = (GoodCardVM)goodCard.DataContext;

            var nodeIndexes = GetNodeIndexes(CurrentNode);

            viewModel.PutData(warehouseManager, new Good(), nodeIndexes, -1);

            goodCard.Show();
        }

        /// <summary>
        /// команда удаления товара
        /// </summary>
        public DelegateCommand DeleteGood => new DelegateCommand(PerformDeleteGood);

        private void PerformDeleteGood()
        {
            var nodeIndexes = GetNodeIndexes(CurrentNode);
            var goodIndex = GetGoodIndex(CurrentGood, CurrentNode);

            warehouseManager.DeleteGood(nodeIndexes, goodIndex);
        }

        /// <summary>
        /// получить путь к node, чтобы в модели её было легко найти
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<int> GetNodeIndexes(Node node)
        {
            var indexes = new List<int>();
            while (node != null && node.Parent != null)
            {
                var index = node.Parent.Nodes.IndexOf(node);
                indexes.Add(index);
                node = node.Parent;
            }
            indexes.Add(Nodes.IndexOf(node));

            indexes.Reverse();

            return indexes;
        }

        private int GetGoodIndex(Good good, Node node)
        {
            return node.Goods.IndexOf(good);
        }

    }
}
