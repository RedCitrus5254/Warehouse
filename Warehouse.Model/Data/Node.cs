using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Warehouse.Model.Data
{
    [XmlType("Node")]
    public class Node : BindableBase
    {
        [XmlIgnore]
        public Node Parent { get; set; }
        [XmlIgnore]
        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                SetProperty(ref name, value);
                RaisePropertyChanged(nameof(Name));
            }
        }
        [XmlArray("Goods"), XmlArrayItem("Good")]
        public ObservableCollection<Good> Goods { get; set; }

        [XmlArray("Nodes"), XmlArrayItem("Node")]
        public ObservableCollection<Node> Nodes { get; set; }

        public Node()
        {
            Goods = new ObservableCollection<Good>();
            Nodes = new ObservableCollection<Node>();
        }
    }
}
