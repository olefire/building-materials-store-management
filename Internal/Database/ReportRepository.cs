using System;
using System.Collections.Generic;
using Npgsql;

namespace BuildingMaterialsStoreManagement.Internal.Database
{

    public interface IReportRepository
    {
        System.Data.DataTable Get (int arrivalId);
        System.Data.DataTable GetItems (int id);
    }

public class SaleReportRepository: IReportRepository
    {
        private readonly NpgsqlConnection _connection;

        public SaleReportRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }


        private const string GetSaleByIdQuery = @"
            SELECT s.Id, d.name as DepartmentAddress, s.sale_date, SUM(si.quantity * p.Price) as total_sum
            FROM Sales s
            JOIN Departments d ON s.department_id = d.Id
            JOIN Sale_Items si ON s.Id = si.sale_id
            JOIN Products p ON si.product_id = p.Id
            WHERE s.Id = @id
            GROUP BY s.Id, s.sale_date, d.name";

        private const string GetSaleItemsQuery = @"
            SELECT si.Id, p.Name as Product_Name, d.name as Department_Address, p.Price as Product_Price, si.Quantity
            FROM Sale_Items si
            JOIN Products p ON si.product_id = p.Id
            JOIN Sales s ON si.sale_id = s.Id
            JOIN Departments d ON s.department_id = d.Id
            WHERE si.sale_id = @id";



        public System.Data.DataTable Get(int saleId)
        {
            var dt = new System.Data.DataTable();
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(GetSaleByIdQuery, _connection))
                {
                    command.Parameters.AddWithValue("id", saleId);
                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
                if (dt.Columns.Count > 1)
                {
                    dt.Columns[1].ColumnName = "Адрес Отдела";
                }

                if (dt.Columns.Count > 2)
                {
                    dt.Columns[2].ColumnName = "Дата продажи";
                }

                if (dt.Columns.Count > 3)
                {
                    dt.Columns[3].ColumnName = "Общая сумма";
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

        public System.Data.DataTable GetItems(int saleId)
        {
            var dt = new System.Data.DataTable();
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(GetSaleItemsQuery, _connection))
                {
                    command.Parameters.AddWithValue("id", saleId);
                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
                if (dt.Columns.Contains("product_name"))
                    dt.Columns["product_name"].ColumnName = "Продукт";
                if (dt.Columns.Contains("product_type"))
                    dt.Columns["product_type"].ColumnName = "Тип продукта";
                if (dt.Columns.Contains("department_address"))
                    dt.Columns["department_address"].ColumnName = "Отдел";
                if (dt.Columns.Contains("product_price"))
                    dt.Columns["product_price"].ColumnName = "Цена";
                if (dt.Columns.Contains("quantity"))
                    dt.Columns["quantity"].ColumnName = "Количество";
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
    }

    public class ArrivalReportRepository: IReportRepository
    {
        private readonly NpgsqlConnection _connection;

        public ArrivalReportRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }
        
        
        private const string GetArrivalsById = @"
            SELECT a.Id, d.name as department_address, a.arrival_date, SUM(ai.quantity * p.price) as total_sum
            FROM Arrivals a
            JOIN Departments d ON a.department_id = d.Id
            JOIN Arrival_Items ai ON a.Id = ai.arrival_id
            JOIN Products p ON ai.product_id = p.Id
            WHERE a.Id = @id
            GROUP BY a.Id, a.arrival_date, d.name";
        
        private const string GetArrivalItemsQuery = @"
            SELECT ai.Id, p.name as product_name,  d.name as department_address, p.price as product_price, ai.quantity
            FROM arrival_items ai
            JOIN products p ON ai.product_id = p.Id
            JOIN arrivals a ON ai.arrival_id = a.Id
            JOIN departments d ON a.department_id = d.Id
            WHERE ai.arrival_id = @id";
        
        
        public System.Data.DataTable Get(int arrivalId)
        {
            var dt = new System.Data.DataTable();
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(GetArrivalsById, _connection))
                {
                    command.Parameters.AddWithValue("id", arrivalId);
                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
                if (dt.Columns.Count > 1)
                {
                    dt.Columns[1].ColumnName = "Адрес Отдела";
                }

                if (dt.Columns.Count > 2)
                {
                    dt.Columns[2].ColumnName = "Дата поступления";
                }

                if (dt.Columns.Count > 3)
                {
                    dt.Columns[3].ColumnName = "Общая сумма";
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

        public System.Data.DataTable GetItems(int arrivalId)
        {
            var dt = new System.Data.DataTable();
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(GetArrivalItemsQuery, _connection))
                {
                    command.Parameters.AddWithValue("id", arrivalId);
                    using (var adapter = new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
                if (dt.Columns.Contains("product_name"))
                    dt.Columns["product_name"].ColumnName = "Продукт";
                if (dt.Columns.Contains("product_type"))
                    dt.Columns["product_type"].ColumnName = "Тип продукта";
                if (dt.Columns.Contains("department_address"))
                    dt.Columns["department_address"].ColumnName = "Отдел";
                if (dt.Columns.Contains("product_price"))
                    dt.Columns["product_price"].ColumnName = "Цена";
                if (dt.Columns.Contains("quantity"))
                    dt.Columns["quantity"].ColumnName = "Количество";
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
    }

    public class ProductReportRepository
    {
        private readonly NpgsqlConnection _connection;
        
        public ProductReportRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

private const string ReportQuery = @"
    WITH initial_stock AS (
        SELECT
            d.id AS department_id,
            p.id AS product_id,
            COALESCE(SUM(dw.quantity), 0) AS initial_stock
        FROM
            departments d
        JOIN products p ON p.department_id = d.id
        LEFT JOIN department_warehouse dw ON dw.department_id = d.id AND dw.product_id = p.id
        WHERE d.id = ANY(@productTypeIds)
        GROUP BY
            d.id, p.id
    ),

    total_arrivals AS (
        SELECT
            d.id AS department_id,
            p.id AS product_id,
            COALESCE(SUM(ai.quantity), 0) AS total_arrived
        FROM
            departments d
        JOIN products p ON p.department_id = d.id
        LEFT JOIN arrivals a ON a.department_id = d.id
        LEFT JOIN arrival_items ai ON ai.arrival_id = a.id AND ai.product_id = p.id
        WHERE
            a.arrival_date >= @endDate
            AND d.id = ANY(@productTypeIds)
        GROUP BY
            d.id, p.id
    ),

    period_arrivals AS (
        SELECT
            d.id AS department_id,
            p.id AS product_id,
            COALESCE(SUM(ai.quantity), 0) AS period_arrived
        FROM
            departments d
        JOIN products p ON p.department_id = d.id
        LEFT JOIN arrivals a ON a.department_id = d.id
        LEFT JOIN arrival_items ai ON ai.arrival_id = a.id AND ai.product_id = p.id
        WHERE
            a.arrival_date BETWEEN @startDate AND @endDate
            AND d.id = ANY(@productTypeIds)
        GROUP BY
            d.id, p.id
    ),

    total_sales AS (
        SELECT
            d.id AS department_id,
            p.id AS product_id,
            COALESCE(SUM(si.quantity), 0) AS total_sold
        FROM
            departments d
        JOIN products p ON p.department_id = d.id
        LEFT JOIN sales s ON s.department_id = d.id
        LEFT JOIN sale_items si ON si.sale_id = s.id AND si.product_id = p.id
        WHERE
            s.sale_date >= @endDate
            AND d.id = ANY(@productTypeIds)
        GROUP BY
            d.id, p.id
    ),

    period_sales AS (
        SELECT
            d.id AS department_id,
            p.id AS product_id,
            COALESCE(SUM(si.quantity), 0) AS period_sold
        FROM
            departments d
        JOIN products p ON p.department_id = d.id
        LEFT JOIN sales s ON s.department_id = d.id
        LEFT JOIN sale_items si ON si.sale_id = s.id AND si.product_id = p.id
        WHERE
            s.sale_date BETWEEN @startDate AND @endDate
            AND d.id = ANY(@productTypeIds)
        GROUP BY
            d.id, p.id
    )

    SELECT
        CASE WHEN row_num = 1 THEN cs.department_id ELSE NULL END AS ""ID отдела"",
        CASE WHEN row_num = 1 THEN cs.department_name ELSE NULL END AS ""Название отдела"",
        cs.product_name AS ""Название товара"",
        COALESCE(per_arr.period_arrived, 0) AS ""Всего поступило за период"",
        COALESCE(per_sale.period_sold, 0) AS ""Оборот"",
        (COALESCE(init.initial_stock, 0) + COALESCE(total_arr.total_arrived, 0) - COALESCE(total_sale.total_sold, 0)) AS ""Остаток"",
        COALESCE(per_sale.period_sold, 0) * cs.product_price AS ""Оборот (сумма)"",
        (COALESCE(init.initial_stock, 0) + COALESCE(total_arr.total_arrived, 0) - COALESCE(total_sale.total_sold, 0)) * cs.product_price AS ""Остаток (сумма)""
    FROM (
        SELECT
            d.id AS department_id,
            d.name AS department_name,
            p.id AS product_id,
            p.name AS product_name,
            p.price AS product_price,
            ROW_NUMBER() OVER (PARTITION BY d.id ORDER BY p.name) AS row_num
        FROM
            departments d
        JOIN products p ON p.department_id = d.id
        WHERE d.id = ANY(@productTypeIds)
    ) cs
    LEFT JOIN initial_stock init ON init.department_id = cs.department_id AND init.product_id = cs.product_id
    LEFT JOIN total_arrivals total_arr ON total_arr.department_id = cs.department_id AND total_arr.product_id = cs.product_id
    LEFT JOIN period_arrivals per_arr ON per_arr.department_id = cs.department_id AND per_arr.product_id = cs.product_id
    LEFT JOIN total_sales total_sale ON total_sale.department_id = cs.department_id AND total_sale.product_id = cs.product_id
    LEFT JOIN period_sales per_sale ON per_sale.department_id = cs.department_id AND per_sale.product_id = cs.product_id
    ORDER BY
        cs.department_id, cs.product_name;
";



        
        public System.Data.DataTable GetReport(DateTime startDate, DateTime endDate, int[] productTypeIds)
        {
            var dt = new System.Data.DataTable();
            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(ReportQuery, _connection))
                {
                    command.Parameters.AddWithValue("startDate", startDate);
                    command.Parameters.AddWithValue("endDate", endDate);
                    command.Parameters.AddWithValue("productTypeIds", productTypeIds);
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
        
        public List<Tuple<int, string>> GetAllProductTypes()
        {
            var productTypes = new List<Tuple<int, string>>();
            const string query = "SELECT Id, Name FROM departments";

            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(query, _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var name = reader.GetString(1);
                            productTypes.Add(new Tuple<int, string>(id, name));
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
                _connection.Close();
            }

            return productTypes;
        }
    }
}