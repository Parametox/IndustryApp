using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace IndustryApp.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        protected static string LoggedUserName;
        protected static string LoggedUserFirstName;

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
            //sqlConnectionString = "Data Source=DESKTOP-2DH49DG/SQLEXPRESS;" +
            //                                "Initial Catalog=InzDatabase;" +
            //                                //"Integrated Security=SSPI;" +
            //                                "User ID=pci;" +
            //                                "Password=pass#pass;";

            sqlConnectionString = "Server=192.168.0.105;" +
                                    "Database=InzDatabase;" +
                                    "User Id=pci;" +
                                    "Password=pass#pass;";

            pageDialogService = dialogService;
            this.NavigationService = navigationService;

        }
    }
}
