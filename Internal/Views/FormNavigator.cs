namespace BuildingMaterialsStoreManagement.Internal.Views
{
    public class FormNavigator
    {
        private readonly Npgsql.NpgsqlConnection _connection;

        public FormNavigator(Npgsql.NpgsqlConnection conn)
        {
            _connection = conn;   
        }

        public void OpenHomeForm()
        {
            var homeForm = new HomeForm(_connection);
            homeForm.Show();
        }

    }
}