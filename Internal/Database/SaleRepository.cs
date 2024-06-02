using System.Data;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.Models;
using Npgsql;

namespace BuildingMaterialsStoreManagement.Internal.Database
{
    public class SaleRepository : ItemRepository, IRepositoryWithItems<Sale>
    {
        private readonly NpgsqlConnection _connection;

        public SaleRepository(NpgsqlConnection connection)
            : base(connection,
                GetSaleItemsQuery,
                AddSaleItemQuery,
                DeleteSaleItemQuery)
        {
            _connection = connection;
        }

        private const string GetAllSalesQuery = @"
            SELECT s.Id, d.name as Отдел, s.sale_date as ""Дата продажи"", s.status as Статус, COALESCE(SUM(si.quantity * p.price), 0) as ""Итоговая сумма""
            FROM Sales s
            JOIN Departments d ON s.department_id = d.Id
            LEFT JOIN Sale_Items si ON s.Id = si.sale_id
            LEFT JOIN Products p ON si.product_id = p.Id
            GROUP BY s.Id, d.name, s.sale_date";


        private const string AddSaleQuery = @"
            INSERT INTO Sales (department_id, sale_date, status) VALUES (@DepartmentId, @SaleDate, 'Чек формируется')";

        private const string AddSaleItemQuery = @"
            INSERT INTO Sale_Items (sale_id, product_id, quantity) 
            VALUES (@id, @productId, @quantity)
            ON CONFLICT (sale_id, product_id) 
            DO UPDATE SET quantity = Sale_Items.quantity + @quantity";

        private const string GetSaleItemsQuery = @"
            SELECT si.Id, p.Name as Product_Name, p.Price as Product_Price, si.Quantity
            FROM Sale_Items si
            JOIN Products p ON si.product_id = p.Id
            JOIN Sales s ON si.sale_id = s.Id
            JOIN Departments d ON s.department_id = d.Id
            WHERE si.sale_id = @id";
        
        private const string DeleteSaleQuery = @"
            DELETE FROM Sale_Items WHERE sale_id = @SaleId;
            DELETE FROM Sales WHERE Id = @SaleId;";

        private const string DeleteSaleItemQuery = @"
            DELETE FROM Sale_Items WHERE Id = @id;";

        private const string UpdateSaleQuery = @"
            UPDATE Sales SET sale_date = @SaleDate, status = @Status::sale_status WHERE Id = @Id;";

        
        public DataTable GetAll()
        {
            var dt = new DataTable();
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(GetAllSalesQuery, _connection))
                using (var adapter = new NpgsqlDataAdapter(command))
                {
                    adapter.Fill(dt);
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


        public void Add(Sale sale)
        {
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(AddSaleQuery, _connection))
                {
                    command.Parameters.AddWithValue("DepartmentId", sale.Department.Id);
                    command.Parameters.AddWithValue("SaleDate", sale.SaleDate);
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


        public void Update(Sale sale)
        {
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(UpdateSaleQuery, _connection))
                {
                    command.Parameters.AddWithValue("SaleDate", sale.SaleDate);
                    command.Parameters.AddWithValue("Status", sale.Status);
                    command.Parameters.AddWithValue("Id", sale.Id);
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
        
        public void Delete(int saleId)
        {
            const string deleteSaleItemsQuery = @"
        DELETE FROM Sale_Items WHERE sale_id = @SaleId";

            try
            {
                _connection.Open();

                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = new NpgsqlCommand(deleteSaleItemsQuery, _connection, transaction))
                        {
                            command.Parameters.AddWithValue("SaleId", saleId);
                            command.ExecuteNonQuery();
                        }

                        using (var command = new NpgsqlCommand(DeleteSaleQuery, _connection, transaction))
                        {
                            command.Parameters.AddWithValue("SaleId", saleId);
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
        
        public string GetSaleStatusById(int saleId)
        {
            string status = null;
            const string getSaleStatusQuery = @"
        SELECT status FROM Sales WHERE Id = @SaleId";

            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(getSaleStatusQuery, _connection))
                {
                    command.Parameters.AddWithValue("SaleId", saleId);
                    status = command.ExecuteScalar() as string;
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

            return status;
        }
    }
}