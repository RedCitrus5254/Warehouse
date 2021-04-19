using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Warehouse.Model.BL;
using Warehouse.Model.Data;

namespace Warehouse.ViewModels
{
    class MenuVM : BindableBase
    {
        public WarehouseManager WarehouseManager { get; set; }


        public MenuVM(WarehouseManager warehouseManager)
        {
            WarehouseManager = warehouseManager;
        }

        /// <summary>
        /// настройки
        /// </summary>
        public DelegateCommand OpenSettings => new DelegateCommand(PerformOpenSettings);

        private void PerformOpenSettings()
        {
        }
        /// <summary>
        /// создание csv-отчёта
        /// </summary>
        public DelegateCommand GenerateCSVReport => new DelegateCommand(PerformGenerateCSVReport);

        private void PerformGenerateCSVReport()
        {
            string path = "";

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Directory|*.this.directory";
            saveFileDialog.FileName = "report.csv";

            if (saveFileDialog.ShowDialog() == true)
            {
                path = saveFileDialog.FileName;
            }

            try
            {
                var count = Int32.Parse(Resource1.ResourceManager.GetObject("MinCountOfGoods").ToString());
                WarehouseManager.GenerateSCVReport(path, count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
                
        }

        /// <summary>
        /// загрузка товаров и классификатора из файла
        /// </summary>
        public DelegateCommand LoadGoods => new DelegateCommand(PerformLoadGoods);

        private void PerformLoadGoods()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    WarehouseManager.LoadGoodsFromXML(openFileDialog.FileName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            
        }

        /// <summary>
        /// сохранение товаров и классификатора в файл
        /// </summary>
        public DelegateCommand SaveGoods => new DelegateCommand(PerformSaveGoods);

        private void PerformSaveGoods()
        {
            string path = "";

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Directory|*.this.directory";
            saveFileDialog.FileName = "Goods.xml";

            if (saveFileDialog.ShowDialog() == true)
            {
                path = saveFileDialog.FileName;
            }

            try
            {
                WarehouseManager.SaveGoodsXml(path);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
