using IndustryApp.Model;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.ServiceModel;
using System.Text;

namespace IndustryApp.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {

        public static User sLoggedUser { get; private set; }
        public static LoggedDevice sDevice { get; private set; }
        public static readonly EndpointAddress EndPoint = new EndpointAddress("http://192.168.0.105/WCF/Service.svc");
        public static BasicHttpBinding binding = CreateBasicHttp();



        protected WCF.ServiceClient client = new WCF.ServiceClient(binding,EndPoint);


        private User loggedUser;
        public User LoggedUser
        {
            get { return loggedUser; }
            set { SetProperty(ref loggedUser, value); }
        }

        private LoggedDevice loggedDevice;
        public LoggedDevice LoggedDevice
        {
            get { return loggedDevice; }
            set { SetProperty(ref loggedDevice, value); }
        }
        protected static string DeviceId;

        protected INavigationService NavigationService { get; private set; }
        protected string sqlConnectionString { get; private set; }
        protected IPageDialogService pageDialogService { get; set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        protected void SetUser(User _user)
        {
            if (_user != null)
            {
                sLoggedUser = _user;
            }
        }

        private static BasicHttpBinding CreateBasicHttp()
        {
            BasicHttpBinding binding = new BasicHttpBinding
            {
                Name = "basicHttpBinding"                
            };
            TimeSpan timeout = new TimeSpan(0, 1, 30);
            binding.SendTimeout = timeout;
            binding.OpenTimeout = timeout;
            binding.ReceiveTimeout = timeout;
            return binding;
        }

        protected void SetDevice(LoggedDevice device)
        {
            if (device != null)
            {
                sDevice = device;
            }
        }

       
        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }

        public ViewModelBase(IPageDialogService dialogService, INavigationService navigationService)

        {
          
           
            pageDialogService = dialogService;
            this.NavigationService = navigationService;

            if (sLoggedUser != null)
                this.LoggedUser = sLoggedUser;

            if (sDevice != null)
                LoggedDevice = sDevice;



       
        }


    }
}
