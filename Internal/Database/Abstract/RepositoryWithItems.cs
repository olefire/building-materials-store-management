using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using BuildingMaterialsStoreManagement.Internal.Models;

using System.Windows.Forms;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.Database.Abstract
{
    public class RepositoryWithItems<T> where T : class
    {
        private readonly IRepositoryWithItems<T> _repository;

        public RepositoryWithItems(IRepositoryWithItems<T> repository)
        {
            _repository = repository;
        }
        
        public DataTable LoadData()
        {
            try
            {
                var dt = _repository.GetAll();
                dt.AcceptChanges();
                return dt;
            }
            catch (Exception e)
            {
                ErrorUtils.ShowErrorMessage(e, ErrorUtils.OperationType.Loading);
                return null;
            }
        }
        
        public void ValidateAndAdd(T entity)
        {
            var context = new ValidationContext(entity, null, null);
            var results = new List<ValidationResult>();

            if (Validator.TryValidateObject(entity, context, results, true))
            {
                _repository.Add(entity);
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
        
        public void ValidateAndUpdate(T entity)
        {
            var context = new ValidationContext(entity, null, null);
            var results = new List<ValidationResult>();

            if (Validator.TryValidateObject(entity, context, results, true))
            {
                _repository.Update(entity);
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
            try
            {
                _repository.Delete(id);
                MessageBox.Show("Данные успешно удалены.");
            }
            catch (Exception e)
            {
                ErrorUtils.ShowErrorMessage(e, ErrorUtils.OperationType.Deleting);
            }
        }
        
        public DataTable LoadItems(int id)
        {
            try
            {
                var dt = _repository.GetAllItems(id);
                dt.AcceptChanges();
                return dt;
            }
            catch (Exception e)
            {
                ErrorUtils.ShowErrorMessage(e, ErrorUtils.OperationType.Loading);
                return null;
            }
        }
        
        public void ValidateAndAddItem(Item entity)
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
        
        public void ValidateAndUpdateItem(Item entity)
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
        
        public void DeleteItem(int id)
        {
            try
            {
                _repository.DeleteItem(id);
                MessageBox.Show("Данные успешно удалены.");
            }
            catch (Exception e)
            {
                ErrorUtils.ShowErrorMessage(e, ErrorUtils.OperationType.Deleting);
            }
        }
        
    }
}