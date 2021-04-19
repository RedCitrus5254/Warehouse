using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Warehouse.Model.Data;

namespace Warehouse.Model.BL
{
    class XmlWorker
    {
        public List<Node> Deserealize(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Node>));
            using FileStream fs = new FileStream(path, FileMode.OpenOrCreate);

            var nodes = (List<Node>)xmlSerializer.Deserialize(fs);

            AddParentToNodes(nodes, null);

            return nodes;
               
            
        } 
        /// <summary>
        /// добавить родительские узлы. При десериализации они не добавляются
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="parent"></param>
        private void AddParentToNodes(IEnumerable<Node> nodes, Node parent)
        {
            foreach(var node in nodes)
            {
                node.Parent = parent;
                AddParentToNodes(node.Nodes, node);
            }
        }

        public void Serealize(IEnumerable<Node> nodes, string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Node>));
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, nodes.ToList());
            }
        }

        public void GenerateCSVReport(IEnumerable<Node> nodes, string path, int minGoods)
        {
            var headerStr = "Путь, Артикул, Название, Количество" + Environment.NewLine;
            File.WriteAllText(path, headerStr, Encoding.UTF8);

            IteratingNodes(nodes, path, minGoods);
        }

        /// <summary>
        /// пройти по всем узлам для отчёта
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="path"></param>
        /// <param name="minGoods"></param>
        private void IteratingNodes(IEnumerable<Node> nodes, string path, int minGoods)
        {
            foreach(var node in nodes)
            {
                WriteGoodsInfoToFile(node, path, minGoods);

                IteratingNodes(node.Nodes, path, minGoods);
            }
        }

        /// <summary>
        /// запись недостающих товаров в csv-отчёт
        /// </summary>
        /// <param name="node"></param>
        /// <param name="path"></param>
        /// <param name="minGoods"></param>
        private void WriteGoodsInfoToFile(Node node, string path, int minGoods)
        {
            IEnumerable<Good> goods = node.Goods;

            string nodesNames = GetNodesNames(node);

            foreach (Good good in goods)
            {
                var stringForFile = nodesNames + ", ";
                if (good.Count < minGoods)
                {
                    stringForFile += good.Article + ", ";
                    stringForFile += good.Name + ", ";
                    stringForFile += good.Count + Environment.NewLine;

                    File.AppendAllText(path, stringForFile, Encoding.UTF8);
                }
            }
        }

        /// <summary>
        /// получение полной цепочки узлов
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string GetNodesNames(Node node)
        {
            if (node.Parent != null)
            {
                return $"{GetNodesNames(node.Parent)}/{node.Name}";
            }
            else
            {
                return $"{node.Name}";
            }
            
        }
    }
}
