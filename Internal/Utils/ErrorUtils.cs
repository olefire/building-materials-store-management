using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BuildingMaterialsStoreManagement.Internal.Utils
{
    public class ErrorUtils
    {
        public enum OperationType
        {
            Loading,
            Adding,
            Updating,
            Deleting
        }

        private static readonly Dictionary<OperationType, string> OperationTypeStrings =
            new Dictionary<OperationType, string>
            {
                { OperationType.Loading, "загрузки" },
                { OperationType.Adding, "добавления" },
                { OperationType.Updating, "обновления" },
                { OperationType.Deleting, "удаления" }
            };

        public static void ShowErrorMessage(Exception ex, OperationType operation)
        {
            if (!OperationTypeStrings.TryGetValue(operation, out var operationString))
            {
                throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }

            MessageBox.Show(
                $"Произошла ошибка при {operationString}: {ex.GetType().FullName}, {ex.Message}, {ex.StackTrace}");
        }
    }
}