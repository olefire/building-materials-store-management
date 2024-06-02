using System.Data;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.Models;
using Npgsql;

namespace BuildingMaterialsStoreManagement.Internal.Database
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly NpgsqlConnection _connection;
        public readonly PairRepository PairRepository;

        public ProductRepository(NpgsqlConnection connection)
        {
            _connection = connection;
            PairRepository = new PairRepository(connection, GetIdAndNamePairsQuery, GetIdByNameQuery);
        }

        private const string GetIdAndNamePairsQuery = @"
            SELECT Id, Name FROM departments";

        private const string GetIdByNameQuery = @"
            SELECT Id FROM departments WHERE Name = @name";


        private const string AddProductQuery = @"
            INSERT INTO Products (department_id, name, price) VALUES (@ProductTypeId, @Name, @Price)";

        private const string UpdateProductQuery = @"
            UPDATE Products SET department_id = @depId, Name = @Name, Price = @Price WHERE Id = @Id";

        private const string DeleteProductQuery = @"
            DELETE FROM Products WHERE Id = @Id";

        public DataTable GetAll()
        {
            const string getAllProductsQuery = @"
            SELECT p.Id, p.Name, d.name as Department, p.Price
            FROM Products p
            JOIN Departments d ON p.department_id = d.Id";
            var dt = new DataTable();
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(getAllProductsQuery, _connection))
                using (var adapter = new NpgsqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                }

                dt.Columns["Name"].ColumnName = "Название товара";
                dt.Columns["Price"].ColumnName = "Цена";
                dt.Columns["Department"].ColumnName = "Отдел";
            }
            catch (NpgsqlException ex)
            {
                RepoUtils.HandleDbException(ex);
            }
            finally
            {
                _connection.Close();
            }

            return dt;
        }


        public void Add(Product product)
        {
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(AddProductQuery, _connection))
                {
                    command.Parameters.AddWithValue("productTypeId", product.Department.Id);
                    command.Parameters.AddWithValue("name", product.Name);
                    command.Parameters.AddWithValue("price", product.Price);
                    command.ExecuteNonQuery();
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


        public void Update(Product product)
        {
            const string updateProductWarehouseDepIdQuery = @"
    UPDATE department_warehouse SET department_id = @newDepId WHERE product_id = @productId";
            try
            {
                _connection.Open();

                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = new NpgsqlCommand(UpdateProductQuery, _connection, transaction))
                        {
                            command.Parameters.AddWithValue("depId", product.Department.Id);
                            command.Parameters.AddWithValue("Name", product.Name);
                            command.Parameters.AddWithValue("Price", product.Price);
                            command.Parameters.AddWithValue("Id", product.Id);
                            command.ExecuteNonQuery();
                        }

                        using (var command = new NpgsqlCommand(updateProductWarehouseDepIdQuery, _connection, transaction))
                        {
                            command.Parameters.AddWithValue("productId", product.Id);
                            command.Parameters.AddWithValue("newDepId", product.Department.Id);
                            command.ExecuteNonQuery();
                        }

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

        public void Delete(int id)
        {
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(DeleteProductQuery, _connection))
                {
                    command.Parameters.AddWithValue("Id", id);
                    command.ExecuteNonQuery();
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
        
        public int GetDepartmentIdByName(string name)
        {
            const string getDepartmentIdByNameQuery = @"
    SELECT Id
    FROM Departments
    WHERE name = @name";

            var departmentId = 0;

            try
            {
                _connection.Open();

                using (var command = new NpgsqlCommand(getDepartmentIdByNameQuery, _connection))
                {
                    command.Parameters.AddWithValue("name", name);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
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
    }
}