using System;
using System.Collections.Generic;
using System.Data;
using BuildingMaterialsStoreManagement.Internal.Models;
using Npgsql;

namespace BuildingMaterialsStoreManagement.Internal.Database.Abstract
{
    public abstract class ItemRepository
    {
        private readonly NpgsqlConnection _connection;
        private readonly string _getItemsByIdQuery;
        private readonly string _insertItemQuery;
        private readonly string _deleteItemQuery;

        protected ItemRepository(NpgsqlConnection connection, string getItemsByIdQuery, string insertItemQuery,
            string deleteItemQuery)
        {
            _connection = connection;
            _getItemsByIdQuery = getItemsByIdQuery;
            _insertItemQuery = insertItemQuery;
            _deleteItemQuery = deleteItemQuery;
        }

        public DataTable GetAllItems(int id)
        {
            var dataTable = new DataTable();

            try
            {
                _connection.Open();

                using (var command = new NpgsqlCommand(_getItemsByIdQuery, _connection))
                {
                    command.Parameters.AddWithValue("id", id);

                    using (var dataAdapter = new NpgsqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }

                if (dataTable.Columns.Contains("product_name"))
                    dataTable.Columns["product_name"].ColumnName = "Продукт";
                if (dataTable.Columns.Contains("product_type"))
                    dataTable.Columns["product_type"].ColumnName = "Тип продукта";
                if (dataTable.Columns.Contains("department_address"))
                    dataTable.Columns["department_address"].ColumnName = "Отдел";
                if (dataTable.Columns.Contains("product_price"))
                    dataTable.Columns["product_price"].ColumnName = "Цена";
                if (dataTable.Columns.Contains("quantity"))
                    dataTable.Columns["quantity"].ColumnName = "Количество";
            }
            catch (NpgsqlException ex)
            {
                RepoUtils.HandleDbException(ex);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return dataTable;
        }

        public void AddItem(Item item)
        {
            try
            {
                _connection.Open();

                using (var command = new NpgsqlCommand(_insertItemQuery, _connection))
                {
                    command.Parameters.AddWithValue("id", item.RefId);
                    command.Parameters.AddWithValue("productId", item.Product.Id);
                    command.Parameters.AddWithValue("quantity", item.Quantity);

                    command.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                RepoUtils.HandleDbException(ex);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        public void UpdateItem(Item item)
        {
            try
            {
                _connection.Open();

                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        DeleteItemInTransaction(item, transaction);
                        InsertItemInTransaction(item, transaction);

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                RepoUtils.HandleDbException(ex);
            }
            finally
            {
                _connection.Close();
            }
        }

        private void DeleteItemInTransaction(Item item, NpgsqlTransaction transaction)
        {
            using (var deleteCommand = new NpgsqlCommand(_deleteItemQuery, _connection, transaction))
            {
                deleteCommand.Parameters.AddWithValue("id", item.Id);
                deleteCommand.ExecuteNonQuery();
            }
        }

        private void InsertItemInTransaction(Item item, NpgsqlTransaction transaction)
        {
            using (var insertCommand = new NpgsqlCommand(_insertItemQuery, _connection, transaction))
            {
                insertCommand.Parameters.AddWithValue("id", item.RefId);
                insertCommand.Parameters.AddWithValue("productId", item.Product.Id);
                insertCommand.Parameters.AddWithValue("quantity", item.Quantity);
                insertCommand.ExecuteNonQuery();
            }
        }

        public void DeleteItem(int id)
        {
            try
            {
                _connection.Open();

                using (var command = new NpgsqlCommand(_deleteItemQuery, _connection))
                {
                    command.Parameters.AddWithValue("id", id);

                    command.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                RepoUtils.HandleDbException(ex);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }
        
        public int GetProductIdByName(string name)
        {
            const string productIdQuery = @"
                SELECT id
                FROM products
                WHERE name = @name";
        
            var productId = 0;

            try
            {
                _connection.Open();

                using (var command = new NpgsqlCommand(productIdQuery, _connection))
                {
                    command.Parameters.AddWithValue("name", name);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            productId = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                RepoUtils.HandleDbException(ex);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return productId;
        }

        public Product GetProductDataById(int id)
        {
            const string productDataQuery = @"
            SELECT d.name as Department, p.price as Price 
            FROM products p
            INNER JOIN departments d ON p.department_id = d.id
            WHERE p.id = @id";
            
            var product = new Product();

            try
            {
                _connection.Open();

                using (var command = new NpgsqlCommand(productDataQuery, _connection))
                {
                    command.Parameters.AddWithValue("id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product.Department = new Department()
                            {
                                Name = reader["Department"].ToString()
                            };
                            product.Price = Convert.ToDecimal(reader["Price"]);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                RepoUtils.HandleDbException(ex);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return product;
        }
        
        public List<Tuple<int, string>> GetProductIdsAndNames()
        {
            const string productIdsAndNamesQuery = @"
            SELECT id, name
            FROM products";

            var productIdsAndNames = new List<Tuple<int, string>>();

            try
            {
                _connection.Open();

                using (var command = new NpgsqlCommand(productIdsAndNamesQuery, _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var name = reader.GetString(1);
                            productIdsAndNames.Add(Tuple.Create(id, name));
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                RepoUtils.HandleDbException(ex);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return productIdsAndNames;
        }
        
        public List<Tuple<int, string>> GetProductIdsAndNamesByDepartment(int departmentId)
        {
            const string productIdsAndNamesByDepartmentQuery = @"
                SELECT p.id, p.name
                FROM products p
                WHERE p.department_id = @departmentId";

            var productIdsAndNames = new List<Tuple<int, string>>();

            try
            {
                _connection.Open();

                using (var command = new NpgsqlCommand(productIdsAndNamesByDepartmentQuery, _connection))
                {
                    command.Parameters.AddWithValue("departmentId", departmentId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var name = reader.GetString(1);
                            productIdsAndNames.Add(Tuple.Create(id, name));
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                RepoUtils.HandleDbException(ex);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return productIdsAndNames;
        }
        
        public List<Tuple<int, string>> GetDepartmentIdsAndNames()
        {
            const string departmentIdsAndNamesQuery = @"
        SELECT id, name
        FROM departments";

            var departmentIdsAndNames = new List<Tuple<int, string>>();

            try
            {
                _connection.Open();

                using (var command = new NpgsqlCommand(departmentIdsAndNamesQuery, _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var name = reader.GetString(1);
                            departmentIdsAndNames.Add(Tuple.Create(id, name));
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                RepoUtils.HandleDbException(ex);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return departmentIdsAndNames;
        }

        public int GetDepartmentIdByName(string name)
        {
            const string departmentIdQuery = @"
        SELECT id
        FROM departments
        WHERE name = @name";

            var departmentId = 0;

            try
            {
                _connection.Open();

                using (var command = new NpgsqlCommand(departmentIdQuery, _connection))
                {
                    command.Parameters.AddWithValue("name", name);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            departmentId = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                RepoUtils.HandleDbException(ex);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return departmentId;
        }
        
        public int GetProductQuantityById(int id)
        {
            const string getItemQuantityByIdQuery = @"
        SELECT quantity
        FROM department_warehouse
        WHERE product_id = @id";

            int quantity = 0;

            try
            {
                _connection.Open();

                using (var command = new NpgsqlCommand(getItemQuantityByIdQuery, _connection))
                {
                    command.Parameters.AddWithValue("id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quantity = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                RepoUtils.HandleDbException(ex);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }

            return quantity;
        }
        
        public void AddProductQuantityById(int id, int newQuantity)
        {
            const string updateItemQuantityByIdQuery = @"
        UPDATE department_warehouse
        SET quantity = quantity + @quantity
        WHERE product_id = @id";

            try
            {
                _connection.Open();

                using (var command = new NpgsqlCommand(updateItemQuantityByIdQuery, _connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("quantity", newQuantity);

                    command.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                RepoUtils.HandleDbException(ex);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }
    }
}