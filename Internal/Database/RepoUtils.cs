using System;
using Npgsql;

namespace BuildingMaterialsStoreManagement.Internal.Database
{
    public class RepoUtils
    {
        public static void HandleDbException(Exception ex)
        {
            var npgsqlEx = ex as PostgresException;
            if (npgsqlEx == null)
            {
                throw new Exception("Произошла непредвиденная ошибка.", ex);
            }

            var errorMessage = "Произошла ошибка при работе с базой данных.";
            switch (npgsqlEx.SqlState)
            {
                case "23505":
                    errorMessage = "Попытка добавить дублирующиеся данные.";
                    break;
                case "23503":
                    errorMessage = "Существуют связанные данные, которые не позволяют выполнить операцию.";
                    break;
                case "42601":
                    errorMessage = "Ошибка синтаксиса запроса.";
                    break;
                default:
                    errorMessage += $" Сообщение: {npgsqlEx.Message}";
                    break;
            }
            throw new Exception(errorMessage, npgsqlEx);
        }
    }
}