using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Model.Data
{
    [Serializable]
    public class Good
    {
        public string Article { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public void UpdateFields(Good good)
        {
            this.Article = good.Article;
            this.Name = good.Name;
            this.Count = good.Count;
            this.Description = good.Description;
            this.Price = good.Price;
        }
    }
}
