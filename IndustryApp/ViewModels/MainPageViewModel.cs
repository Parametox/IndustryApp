using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mqtt;
using Prism.Services;
using IndustryApp.Model;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Xamarin.Forms;
using WCF;
using System.Threading;

namespace IndustryApp.ViewModels
{

    public class MainPageViewModel : ViewModelBase
    {
        private int ObjId;

        #region Properities
        public WCF.ServiceClient WCFService { get; set; }
        private bool clockRunning = true;

        private TemperatureTable tempTable;
        public TemperatureTable TempTable
        {
            get { return tempTable; }
            set { SetProperty(ref tempTable, value); }
        }

        private ValidateUserResponse validUserRes;
        public ValidateUserResponse ValidUserRes
        {
            get { return validUserRes; }
            set { SetProperty(ref validUserRes, value); }
        }

        private string timeNow;
        public string TimeNow
        {
            get { return timeNow; }
            set { SetProperty(ref timeNow, value); }
        }

        private ChartSelector selectedItem;
        public ChartSelector SelectedItem
        {
            get { return selectedItem; }
            set 
            {
                SetProperty(ref selectedItem, value);
                if (selectedItem != null)
                {
                    NaviToOxyPlotPage(selectedItem);
                }
            }
        }

        private List<ChartSelector> collectionViewItems;
        public List<ChartSelector> CollectionViewItems
        {
            get { return collectionViewItems; }
            set { SetProperty(ref collectionViewItems, value); }
        }
        #endregion



        #region Methods
        private void InitValues()
        {
            base.Title = "Parametry układu";
            WCFService = new ServiceClient(WCF.ServiceClient.EndpointConfiguration.BasicHttpBinding_IService, "http://192.168.0.105/WCF/Service.svc");
            Thread thd = new Thread(() => TickTockMachine());
            thd.Start();

            CollectionViewItems = new List<ChartSelector>();
            this.declareCollection();
        }

        private void declareCollection()
        {
            CollectionViewItems = new List<ChartSelector>();
            CollectionViewItems.Add(new ChartSelector() {ID = 1, Name = "1 MINUTA" });
            //CollectionViewItems.Add(new ChartSelector() {Name = "2 MINUTY" });
            //CollectionViewItems.Add(new ChartSelector() {Name = "3 MINUTY" });
            //CollectionViewItems.Add(new ChartSelector() {Name = "4 MINUTY" });
        }

        private void TickTockMachine()
        {
            while (clockRunning)
            {
                var x = Thread.CurrentThread.ManagedThreadId;
                TempTable = WCFService.GetLastTemperatureRecord();
                TimeNow = DateTime.Now.ToString("HH:mm:ss");
                Thread.Sleep(500);
            }
        }

        private async void NaviToOxyPlotPage(ChartSelector _charts)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("config", _charts.ID);
            await NavigationService.NavigateAsync("OxyPlotPage",parameters);
        }


        #endregion



        #region Commands
        private DelegateCommand collectionItemSelected;
        public DelegateCommand CollectionItemSelected =>
            collectionItemSelected ?? (collectionItemSelected = new DelegateCommand(ExecuteCollectionItemSelected));
        void ExecuteCollectionItemSelected()
        {
            if (SelectedItem !=null)
            {
                this.NaviToOxyPlotPage(SelectedItem);
            }
        }

      
        #endregion



        public MainPageViewModel(IPageDialogService p, INavigationService ns)
            : base(p, ns)
        {
            InitValues();


        }
    }
}
