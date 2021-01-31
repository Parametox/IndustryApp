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
using Xamarin.Essentials;
using System.Threading;
using IndustryApp.Model;
using WCF;

namespace IndustryApp.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private bool validLoginData = false;
        private string naviPagePath = "MainPage";

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

        private ValidateUserResponse response;
        public ValidateUserResponse Response
        {
            get { return response; }
            set { SetProperty(ref response, value); }
        }


        private DelegateCommand loginCommand;
        public DelegateCommand LoginCommand =>
            loginCommand ?? (loginCommand = new DelegateCommand(ExecuteLoginCommand));
        async void ExecuteLoginCommand()
        {
            if (!String.IsNullOrEmpty(UserId))
            {
                Loading();
                await Task.Run(() => VerifyUser());
                if (validLoginData && !(String.IsNullOrEmpty(Response.DeviceName)))
                {
                    await NavigationService.NavigateAsync(naviPagePath);
                }
                else
                {
                    if (String.IsNullOrEmpty(Response.DeviceName))
                    {
                        if (client.RegisterDevice(DeviceId))
                            await pageDialogService.DisplayAlertAsync("DEVICE ERROR", "URZĄDZENIE NIE JEST SKONFIGUROWANE Z SYSTEMEM => Sprawdź baze", "OK");

                        else
                            await pageDialogService.DisplayAlertAsync("DEVICE ERROR", "URZĄDZENIE NIE JEST SKONFIGUROWANE Z SYSTEMEM", "OK");
                    }
                    else if (String.IsNullOrEmpty(Response.UserName))
                    {
                        await pageDialogService.DisplayAlertAsync("LOGIN ERROR", "BŁĘDNE ID LUB HASŁO", "OK");
                    }
                    ClearView();
                }
            }
        }

        private void ClearView()
        {
            UserId = UserPassword = "";
            IsLoading = false;
            IdAndPassEnabled = true;
        }

        private async Task<bool> VerifyUser()
        {
            //WCFService.ServiceClient serviceClient = new WCFService.ServiceClient();
            //Response = serviceClient.ValidateUser(this.UserId, this.UserPassword, DeviceId);
             WCF.ServiceClient client11 = new WCF.ServiceClient(ServiceClient.EndpointConfiguration.BasicHttpBinding_IService, "http://192.168.0.105/WCF/Service.svc");
            try
            {
                Response = client11.ValidateUser(this.UserId, this.UserPassword, DeviceId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            } 
            User user = new User
            {
                UserId = response.UserId,
                Name = response.UserName,
            };
            LoggedDevice device = new LoggedDevice()
            {
                ID = response.DeviceId,
                Name = response.DeviceName
            };
            base.SetUser(user);
            base.SetDevice(device);
            validLoginData = Response.IsValid;
            return Response.IsValid;
        }

        private void Loading()
        {
            IsLoading = true;
            IdAndPassEnabled = false;
        }

        private void InitVM()
        {
            base.Title = "Zaloguj się";
            IdAndPassEnabled = true;
            ClearView();
            DeviceId = this.getDeviceId();
            try
            {
                WCF.ServiceClient sc = new WCF.ServiceClient(ServiceClient.EndpointConfiguration.BasicHttpBinding_IService, "http://192.168.0.105/WCF/Service.svc");
                var x = sc.GetData(22);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }

        }

        private string getDeviceId()
        {
            string retStr = String.Empty;
            var deviceId = Preferences.Get("my_deviceId", string.Empty);
            if (String.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = System.Guid.NewGuid().ToString();
                Preferences.Set("my_deviceId", deviceId);
            }
            retStr = deviceId;

#if DEBUG
            retStr = "f1454c14-8f64-4d27-8733-3a1c2ce554fa";
#endif
            return retStr;
        }

        public LoginPageViewModel(IPageDialogService p, INavigationService ns) : base(p, ns)
        {
            this.InitVM();
        }
    }
}
