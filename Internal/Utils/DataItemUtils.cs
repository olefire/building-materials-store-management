using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.Models;

namespace BuildingMaterialsStoreManagement.Internal.Utils
{
    public class DataItemUtils
    {
        private readonly ItemRepository _repository;

        public DataItemUtils(ItemRepository repository)
        {
            _repository = repository;
        }

        public DataTable LoadData(int id)
        {
            var dt = _repository.GetAllItems(id);
            dt.AcceptChanges();
            return dt;
        }

        public void ValidateAndAdd(Item entity)
        {
            var context = new ValidationContext(entity, null, null);
            var results = new List<ValidationResult>();

            if (Validator.TryValidateObject(entity, context, results, true))
            {
                _repository.AddItem(entity);
                MessageBox.Show("Данные успешно добавлены.");
            }
            else
            {
                foreach (var result in results)
                {
                    MessageBox.Show(result.ErrorMessage);
                }
            }
        }

        public void ValidateAndUpdate(Item entity)
        {
            var context = new ValidationContext(entity, null, null);
            var results = new List<ValidationResult>();

            if (Validator.TryValidateObject(entity, context, results, true))
            {
                _repository.UpdateItem(entity);
                MessageBox.Show("Данные успешно обновлены.");
            }
            else
            {
                foreach (var result in results)
                {
                    MessageBox.Show(result.ErrorMessage);
                }
            }
        }
        
        public void Delete(int id)
        {
            _repository.DeleteItem(id);
            MessageBox.Show("Данные успешно удалены.");
        }
    }
}