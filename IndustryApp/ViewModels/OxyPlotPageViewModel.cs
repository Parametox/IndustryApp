using IndustryApp.Model;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace IndustryApp.ViewModels
{
    public class OxyPlotPageViewModel : ViewModelBase
    {
        private PlotModel model;
        public PlotModel Model
        {
            get { return model; }
            set { SetProperty(ref model, value); }
        }

        private IPlotView plotView;
        public IPlotView PlotView
        {
            get { return plotView; }
            set { SetProperty(ref plotView, value); }
        }

        private List<DataPoint> pionts;
        public List<DataPoint> Points
        {
            get { return pionts; }
            set { SetProperty(ref pionts, value); }
        }

        private WCF.TemperatureCollection tempCollection;
        public WCF.TemperatureCollection TempCollection
        {
            get { return tempCollection; }
            set { SetProperty(ref tempCollection, value); }
        }

        private SelectableLineSeries ss1;
        public SelectableLineSeries s1
        {
            get { return ss1; }
            set { SetProperty(ref ss1, value); }
        }
        //public SelectableLineSeries s1 { get; set; }




        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("config"))
            {
                var config = parameters.GetValue<short>("config");
                switch (config)
                {
                    case 1:
                        this.OneMinuteChart();
                        Thread t = new Thread(x);
                        t.Start();
                        break;
                    default:
                        break;
                }
            }
        }
        private void x()
        {
            while (true)
            {
                this.OneMinuteChart();
            }
        }
        private static void dd()
        {

        }
        private void OneMinuteChart()
        {

            try
            {
                TempCollection = client.GetMunuteTemperature();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            Points = new List<DataPoint>();

            try
            {

                Model = new PlotModel();
                Model.Title = "Zestawienie";
                Model.PlotType = PlotType.XY;
                Model.InvalidatePlot(true);

                var startDate = DateTime.Now.AddMinutes(-1);
                var endDate = DateTime.Now;
                var minValue = DateTimeAxis.ToDouble(startDate);
                var maxValue = DateTimeAxis.ToDouble(endDate);

                Model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minValue, Maximum = maxValue, StringFormat = "HH:mm:ss" });
                Model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 20, Maximum = 100 });
                Model.ResetAllAxes();

                for (int i = 0; i < TempCollection.TemperatureTables.Length; i++)
                {
                    var item = TempCollection.TemperatureTables[i];
                    double x, y;
                    x = DateTimeAxis.ToDouble(item.Date);
                    y = double.Parse(item.Temperature.Replace('.',','));
                    var point = new DataPoint(x,y);

                    Points.Add(point);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //throw;
            }




            //https://bytelanguage.net/2019/11/07/oxyplot-selectable-point/

            s1 = new SelectableLineSeries()
            {
                Title = "Temperature",
                MarkerType = MarkerType.Circle,
                SelectionMode = SelectionMode.Single,
                IsDataPointSelectable = true,
                MarkerFill = OxyColors.Blue,
                LineStyle = LineStyle.Solid,
                Color = OxyColors.Blue,
                MarkerSize = 5,
                ItemsSource = Points
            };
            Model.InvalidatePlot(true);
            Model.Series.Add(s1);

        }

        public OxyPlotPageViewModel(IPageDialogService p, INavigationService ns)
            : base(p, ns)
        {

            NumberFormatInfo nfi = new CultureInfo("pl-PL").NumberFormat;
            nfi.NumberDecimalSeparator = ".";
        }
    }
}
