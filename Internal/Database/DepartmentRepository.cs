using System.Data;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using Npgsql;
using BuildingMaterialsStoreManagement.Internal.Models;

namespace BuildingMaterialsStoreManagement.Internal.Database
{
    public class DepartmentRepository: ItemRepository, IRepositoryWithItems<Department>
    {
        private readonly NpgsqlConnection _connection;
        
        private const string GetAllDepartmentWarehousesQuery = @"
            SELECT dw.id, p.name as Продукт, p.price as Цена, dw.quantity as Количество
            FROM department_warehouse dw
            JOIN Departments d ON dw.department_id = d.Id
            JOIN Products p ON dw.product_id = p.Id
            WHERE dw.department_id = @id";
        

        private const string AddDepartmentWarehouseQuery = @"
            INSERT INTO department_warehouse (department_id, product_id, quantity) 
            VALUES (@id, @productId, @quantity)
            ON CONFLICT (department_id, product_id) 
            DO UPDATE SET quantity = department_warehouse.quantity + @quantity";

        private const string DeleteDepartmentWarehouseQuery = @"
            DELETE FROM department_warehouse WHERE Id = @id";
        

        public DepartmentRepository(NpgsqlConnection connection)
        : base(connection, GetAllDepartmentWarehousesQuery, AddDepartmentWarehouseQuery, DeleteDepartmentWarehouseQuery)
        {
            _connection = connection;
        }
        
        private const string GetAllDepartmentsQuery = @"
            SELECT id, name as Отдел FROM Departments";
        
        private const string AddDepartmentQuery = @"
            INSERT INTO Departments (name) VALUES (@name)";

        private const string UpdateDepartmentQuery = @"
            UPDATE Departments SET name = @name WHERE Id = @Id";

        private const string DeleteDepartmentQuery = @"
            DELETE FROM Departments WHERE Id = @Id";

        public DataTable GetAll()
        {
            var dt = new DataTable();
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(GetAllDepartmentsQuery, _connection))
                {
                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
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
            return dt;
        }
        
        
        public void Add(Department department)
        {
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(AddDepartmentQuery, _connection))
                {
                    command.Parameters.AddWithValue("name", department.Name);
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

        public void Update(Department department)
        {
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(UpdateDepartmentQuery, _connection))
                {
                    command.Parameters.AddWithValue("name", department.Name);
                    command.Parameters.AddWithValue("Id", department.Id);
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

        public void Delete(int id)
        {
            const string deleteDepartmentWarehouseItemsQuery = @"
        DELETE FROM department_warehouse WHERE department_id = @Id";

            try
            {
                _connection.Open();

                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = new NpgsqlCommand(deleteDepartmentWarehouseItemsQuery, _connection, transaction))
                        {
                            command.Parameters.AddWithValue("Id", id);
                            command.ExecuteNonQuery();
                        }
                        using (var command = new NpgsqlCommand(DeleteDepartmentQuery, _connection, transaction))
                        {
                            command.Parameters.AddWithValue("Id", id);
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
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }
        
    }
}