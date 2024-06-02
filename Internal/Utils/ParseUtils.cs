using System.Globalization;
using System.Windows.Forms;

namespace BuildingMaterialsStoreManagement.Internal.Utils
{
    public static class ParseUtils
    {
        public static decimal TryParseDecimal(TextBox textBox)
        {
            decimal price;
            if (!decimal.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out price))
            {
                MessageBox.Show("Введите корректное числовое значение для цены");
                return -1;
            }
            return price;
        }
    }
}