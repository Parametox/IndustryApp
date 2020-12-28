using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Xamarin.Forms;
using Prism.Services;
using Prism.Navigation;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IndustryApp.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private bool validLoginData = false;

        private string userId;
        public string UserId
        {
            get { return userId; }
            set { SetProperty(ref userId, value); }
        }

        private string userPassword;
        public string UserPassword
        {
            get { return userPassword; }
            set { SetProperty(ref userPassword, value); }
        }

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set { SetProperty(ref isLoading, value); }
        }

        private bool idAndPassEnabled;
        public bool IdAndPassEnabled
        {
            get { return idAndPassEnabled; }
            set { SetProperty(ref idAndPassEnabled, value); }
        }

        private DelegateCommand loginCommand;
        public DelegateCommand LoginCommand =>
            loginCommand ?? (loginCommand = new DelegateCommand(ExecuteLoginCommand));
        void ExecuteLoginCommand()
        {
            if (!String.IsNullOrEmpty(UserId))
            {
                IsLoading = true;
                IdAndPassEnabled = false;
                this.ConnectToDb();
                IsLoading = false;
                IdAndPassEnabled = true;
            }
        }

        private async void ConnectToDb()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(base.sqlConnectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = "SELECT TOP 1 * " +
                                                "FROM dbo.UserTable AS ut " +
                                                $"WHERE ut.[UserId] = '{this.UserId}' " +
                                                $"AND ut.[Password] = '{this.UserPassword}'";
                        command.Connection = sqlConnection;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                // navi to main page

                                while (reader.Read())
                                {
                                    ViewModelBase.LoggedUserName = reader.GetString(3);
                                    ViewModelBase.LoggedUserFirstName = reader.GetString(4);
                                    validLoginData = true;
                                }
                            }
                            else
                            {
                                
                                pageDialogService.DisplayAlertAsync("UWAGA",
                                                                     "Niepoprawny login lub hasło",
                                                                     "OK");
                            }
                        }
                    }

                    sqlConnection.Close();
                    s.Stop();
                    var x = s.ElapsedMilliseconds;
                }
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() => pageDialogService.DisplayAlertAsync("SQLAlert", $"[SQLException] {ex.Message}", "OK"));
            }
            finally
            {
                if (validLoginData)
                {
                    this.ChceckDb();
                    var res = await NavigationService.NavigateAsync("MainPage");
                    if (!res.Success)
                    {
                        Device.BeginInvokeOnMainThread(() => pageDialogService.DisplayAlertAsync("Navigation", "Błąd nawigacji: " + res.Exception.Message, "OK"));
                    }
                }
            }
        }

        private void ChceckDb()
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * FROM TemperatureTable where [Date] < @param";

                        cmd.Parameters.AddWithValue("@param", DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss"));
                        //int i = cmd.ExecuteNonQuery();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var x = reader.GetInt64(0);
                                Console.WriteLine(reader.GetInt64(0));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    Device.BeginInvokeOnMainThread(() => pageDialogService.DisplayAlertAsync("SQLAlert", $"[SQLException] {ex.Message}", "OK"));

                }
                finally
                {
                    connection.Close();
                    IsLoading = false;
                    IdAndPassEnabled = true;
                }
            }
        }

        private void InitVM()
        {
            base.Title = "Zaloguj się";
            IdAndPassEnabled = true;
        }
        public LoginPageViewModel(IPageDialogService p, INavigationService ns) : base(p, ns)
        {
            this.InitVM();
        }
    }
}
