using System.Data;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.Models;
using Npgsql;
using NpgsqlTypes;

namespace BuildingMaterialsStoreManagement.Internal.Database
{
    public class ArrivalRepository : ItemRepository, IRepositoryWithItems<Arrival>
    {
        private readonly NpgsqlConnection _connection;

        public ArrivalRepository(NpgsqlConnection connection)
            : base(connection, GetArrivalItemsQuery, AddArrivalItemQuery, DeleteArrivalItemQuery)
        {
            _connection = connection;
        }

        private const string GetAllArrivalsQuery = @"
            SELECT a.Id, d.name as Отдел, a.arrival_date as ""Дата поступления"", a.status as Статус, COALESCE(SUM(ai.quantity * p.price), 0) as ""Итоговая сумма""
            FROM Arrivals a
            JOIN Departments d ON a.department_id = d.Id
            LEFT JOIN Arrival_Items ai ON a.Id = ai.arrival_id
            LEFT JOIN Products p ON ai.product_id = p.Id
            GROUP BY a.Id, d.name, a.arrival_date;";

        private const string AddArrivalQuery = @"
            INSERT INTO arrivals (department_id, arrival_date, status) VALUES (@departmentId, @arrivalDate, 'Поступление формируется')";

        private const string UpdateArrivalQuery = @"
            UPDATE Arrivals SET arrival_date = @arrivalDate, status = @status::arrival_status WHERE Id = @Id";

        private const string DeleteArrivalQuery = @"
            DELETE FROM Arrivals WHERE Id = @Id";


        private const string GetArrivalItemsQuery = @"
            SELECT ai.Id, p.name as Продукт, p.price as Цена,  ai.quantity as Количество
            FROM arrival_items ai
            JOIN products p ON ai.product_id = p.Id
            JOIN arrivals a ON ai.arrival_id = a.Id
            JOIN departments d ON a.department_id = d.Id
            WHERE ai.arrival_id = @id";

        private const string AddArrivalItemQuery = @"
            INSERT INTO Arrival_Items (arrival_id, product_id, quantity) 
            VALUES (@id, @productId, @quantity)
            ON CONFLICT (arrival_id, product_id) 
            DO UPDATE SET quantity = Arrival_Items.quantity + @quantity";


        private const string DeleteArrivalItemQuery = @"
            DELETE FROM Arrival_Items WHERE Id = @id";

        public DataTable GetAll()
        {
            var dt = new DataTable();
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(GetAllArrivalsQuery, _connection))
                using (var adapter = new NpgsqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                }
            }
            catch
                (NpgsqlException ex)
            {
                RepoUtils.HandleDbException(ex);
            }
            finally
            {
                _connection.Close();
            }

            return dt;
        }


        public void Add(Arrival arrival)
        {
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(AddArrivalQuery, _connection))
                {
                    command.Parameters.AddWithValue("departmentId", arrival.Department.Id);
                    command.Parameters.AddWithValue("arrivalDate", arrival.ArrivalDate);
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


        public void Update(Arrival arrival)
        {
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(UpdateArrivalQuery, _connection))
                {
                    command.Parameters.AddWithValue("Id", arrival.Id);
                    command.Parameters.AddWithValue("arrivalDate", arrival.ArrivalDate);
                    command.Parameters.AddWithValue("status",  arrival.Status);
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
            const string deleteItemElementsQuery = @"
        DELETE FROM arrival_items WHERE arrival_id = @id";

            try
            {
                _connection.Open();

                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        // Удаление элементов товара
                        using (var command = new NpgsqlCommand(deleteItemElementsQuery, _connection))
                        {
                            command.Parameters.AddWithValue("id", id);
                            command.ExecuteNonQuery();
                        }

                        // Удаление самого товара
                        using (var command = new NpgsqlCommand(DeleteArrivalQuery, _connection))
                        {
                            command.Parameters.AddWithValue("id", id);
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
        public string GetArrivalStatusById(int id)
        {
            const string getArrivalStatusByIdQuery = @"
        SELECT status
        FROM arrivals
        WHERE id = @id";

            string status = string.Empty;

            try
            {
                _connection.Open();

                using (var command = new NpgsqlCommand(getArrivalStatusByIdQuery, _connection))
                {
                    command.Parameters.AddWithValue("id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            status = reader.GetString(0);
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

            return status;
        }
    }
}