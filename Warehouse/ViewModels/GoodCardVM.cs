using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Model.BL;
using Warehouse.Model.Data;
using System.Windows.Input;
using Prism.Commands;
using System.Windows;

namespace Warehouse.ViewModels
{
    public class GoodCardVM : BindableBase
    {
        private WarehouseManager warehouseManager;
        private Good good;

        public Good Good
        {
            get
            {
                return good;
            }
            set
            {
                good = value;
                RaisePropertyChanged(nameof(Good));
            }
        }
        private List<int> nodeIndexes;
        private int goodIndex;

        /// <summary>
        /// Заполняем информацию, связанную с товаром
        /// </summary>
        /// <param name="warehouseManager"></param>
        /// <param name="good"></param>
        /// <param name="nodeIndexes">путь к узлу этого товара</param>
        /// <param name="goodIndex">индекс товара в узле</param>
        public void PutData(WarehouseManager warehouseManager, Good good, List<int> nodeIndexes, int goodIndex)
        {
            this.warehouseManager = warehouseManager;
            this.Good = good;
            this.nodeIndexes = nodeIndexes;
            this.goodIndex = goodIndex;
        }

        /// <summary>
        /// создать/обновить товар
        /// </summary>
        public DelegateCommand SaveGood => new DelegateCommand(PerformSaveGood);

        private void PerformSaveGood()
        {
            warehouseManager.UpdateGood(nodeIndexes, goodIndex, Good);
        }
    }
}
