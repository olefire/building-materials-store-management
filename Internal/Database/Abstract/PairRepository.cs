using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace BuildingMaterialsStoreManagement.Internal.Database.Abstract
{
    public class PairRepository
    {
        private readonly NpgsqlConnection _connection;
        private readonly string _getPairQuery;
        private readonly string _getIdByNameQuery;

        public PairRepository(NpgsqlConnection connection, string getPairQuery, string getIdByNameQuery)
        {
            _connection = connection;
            _getPairQuery = getPairQuery;
            _getIdByNameQuery = getIdByNameQuery;
        }

        public List<Tuple<int, string>> GetIdAndNamePairs()
        {
            var clients = new List<Tuple<int, string>>();

            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(_getPairQuery, _connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var name = reader.GetString(1);

                            clients.Add(new Tuple<int, string>(id, name));
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

            return clients;
        }
        
        public int GetIdByName(string name)
        {
            var id = 0;

            try
            {
                _connection.Open();
                using (var command = new NpgsqlCommand(_getIdByNameQuery, _connection))
                {
                    command.Parameters.AddWithValue("name", name);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id = reader.GetInt32(0);
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

            return id;
        }
    }
}