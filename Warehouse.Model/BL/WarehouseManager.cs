using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Model.Data;

namespace Warehouse.Model.BL
{
    public class WarehouseManager : BindableBase
    {
        //классификатор
        public ReadOnlyObservableCollection<Node> Nodes { get; }
        private readonly ObservableCollection<Node> _nodes;

        private XmlWorker XmlWorker = new XmlWorker();

        public WarehouseManager()
        {
            _nodes = new ObservableCollection<Node>( new List<Node>{ new Node { Name = "Главная папка" } });
            Nodes = new ReadOnlyObservableCollection<Node>(_nodes);
        }

        /// <summary>
        /// создать узел
        /// </summary>
        /// <param name="nodeIndexes">положение узла в классификаторе</param>
        /// <param name="nodeName"></param>
        public void CreateNode(List<int> nodeIndexes, string nodeName)
        {
            var node = GetNode(nodeIndexes);

            if (node != null)
            {
                node.Nodes.Add(new Node
                {
                    Name = nodeName,
                    Parent = node
                });
            }
            else
            {
                _nodes.Add(new Node
                {
                    Name = nodeName
                });
            }
            
        }
        /// <summary>
        /// изменить имя узла
        /// </summary>
        /// <param name="nodeIndexes">положение узла в классификаторе</param>
        /// <param name="name"></param>
        public void UpdateNodeName(List<int> nodeIndexes, string name)
        {
            var updatingNode = GetNode(nodeIndexes);

            updatingNode.Name = name;
        }

        /// <summary>
        /// удалить узел
        /// </summary>
        /// <param name="nodeIndexes">положение узла в классификаторе</param>
        public void DeleteNode(List<int> nodeIndexes)
        {
            var deletingNode = GetNode(nodeIndexes);

            var parent = deletingNode.Parent;
            if (parent != null)
            {
                parent.Nodes.Remove(deletingNode);
            }
            else
            {
                _nodes.Remove(deletingNode);
            }
        }

        /// <summary>
        /// обновить данные о товаре
        /// </summary>
        /// <param name="nodeIndexes">положение узла, в котором находится товар</param>
        /// <param name="goodIndex">положение товара в узле</param>
        /// <param name="good"></param>
        public void UpdateGood(List<int> nodeIndexes, int goodIndex, Good good)
        {
            var node = GetNode(nodeIndexes);

            if (goodIndex != -1)
            {
                var updatingGood = node.Goods[goodIndex];

                updatingGood.UpdateFields(good);
            }
            else
            {
                var newGood = new Good();
                newGood.UpdateFields(good);

                node.Goods.Add(newGood);
            }
        }
        /// <summary>
        /// удалить товар
        /// </summary>
        /// <param name="nodeIndexes">положение узла, в котором находится товар</param>
        /// <param name="goodIndex">положение товара в узле</param>
        public void DeleteGood(List<int> nodeIndexes, int goodIndex)
        {
            var node = GetNode(nodeIndexes);
            node.Goods.RemoveAt(goodIndex);
        }
        private Node GetNode(List<int> nodeIndexes)
        {
            if (nodeIndexes[0] == -1)
            {
                return null;
            }
            var updatingNode = _nodes[nodeIndexes[0]];
            for (int i = 1; i < nodeIndexes.Count; i++)
            {
                updatingNode = updatingNode.Nodes[nodeIndexes[i]];
            }

            return updatingNode;
        }

        /// <summary>
        /// сохранить классификатор и товары в xml
        /// </summary>
        /// <param name="path"></param>
        public void SaveGoodsXml(string path)
        {
            XmlWorker.Serealize(_nodes, path);
        }
        /// <summary>
        /// загрузить товары и классификатор из xml
        /// </summary>
        /// <param name="path"></param>
        public void LoadGoodsFromXML(string path)
        {
            while (_nodes.Count > 0)
            {
                _nodes.RemoveAt(0);
            }

            _nodes.AddRange(XmlWorker.Deserealize(path));
        }
        /// <summary>
        /// создать csv-отчёт
        /// </summary>
        /// <param name="path"></param>
        /// <param name="minGoodsCount"></param>
        public void GenerateSCVReport(string path, int minGoodsCount)
        {
            XmlWorker.GenerateCSVReport(_nodes, path, minGoodsCount);
        }
    }
}
