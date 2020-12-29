using OxyPlot;
using OxyPlot.Series;
using System;
using System.Linq;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using IndustryApp.ViewModels;

namespace IndustryApp.Model
{

    public class SelectedLineSeries : LineSeries
    {
    }

    public class SelectableLineSeries : LineSeries
    {
        OxyPlotPageViewModel vm { get; }
        public bool IsDataPointSelectable { get; set; }

        public DataPoint CurrentSelection { get; set; }

        public OxyColor SelectedDataPointColor { get; set; } = OxyColors.Red;

        public double SelectedMarkerSize { get; set; }

        public SelectableLineSeries()
        {
            
            SelectedMarkerSize = MarkerSize;
            TouchCompleted += SelectedLineSeries_TouchCompleted;
            TouchStarted += SelectedLineSeries_TouchCompleted;
            MouseDown += SelectableLineSeries_MouseDown;
        }

        private void SelectedLineSeries_TouchCompleted(object sender, OxyTouchEventArgs e)
        {
            if (IsDataPointSelectable)
            {
                var activeSeries = (sender as OxyPlot.Series.Series);
                var currentPlotModel = activeSeries.PlotModel;
                var nearestPoint = activeSeries.GetNearestPoint(e.Position, false);
                CurrentSelection = nearestPoint.DataPoint;



                currentPlotModel = ClearCurrentSelection(currentPlotModel);

                var selectedSeries = new SelectedLineSeries
                {
                    MarkerSize = MarkerSize + 2,
                    MarkerFill = SelectedDataPointColor,
                    MarkerType = MarkerType
                };

                selectedSeries.Points.Add(CurrentSelection);
                currentPlotModel.Series.Add(selectedSeries);
                currentPlotModel.InvalidatePlot(true);
            }
        }

        private void SelectableLineSeries_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (IsDataPointSelectable)
            {
                var activeSeries = (sender as OxyPlot.Series.Series);
                var currentPlotModel = activeSeries.PlotModel;
                var nearestPoint = activeSeries.GetNearestPoint(e.Position, false);
                CurrentSelection = nearestPoint.DataPoint;

                currentPlotModel = ClearCurrentSelection(currentPlotModel);

                var selectedSeries = new SelectedLineSeries
                {
                    MarkerSize = MarkerSize + 2,
                    MarkerFill = SelectedDataPointColor,
                    MarkerType = MarkerType
                };

                selectedSeries.Points.Add(CurrentSelection);
                currentPlotModel.Series.Add(selectedSeries);
                currentPlotModel.InvalidatePlot(true);
            }
        }

        private PlotModel ClearCurrentSelection(PlotModel plotModel)
        {
            while (plotModel.Series.Any(x => x is SelectedLineSeries))
            {
                plotModel.Series.Remove(plotModel.Series.FirstOrDefault(x => x is SelectedLineSeries));
            }
            return plotModel;
        }
    }
}
