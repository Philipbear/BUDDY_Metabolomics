//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BUDDY.MgfHandler;
//using OxyPlot;
//using OxyPlot.Annotations;
//using OxyPlot.Axes;
//using OxyPlot.Series;

//namespace BUDDY
//{
//    public class SpectrumModel
//    {
//        public SpectrumModel()
//        {
//            List<MgfRecord> mgf = new List<MgfRecord>();
//            mgf = MgfParser.ReadMgf(@"X:\Users\Shipei_Xing\Bottom-up_2020Dec\BUDDY_input_demo\demo_1.mgf");

//            var model = new PlotModel { Title = "Precursor m/z: " + mgf[0].Pepmass };
//            var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };

//            for (int i = 0; i < mgf[0].Spectrum.Count; i++)
//            {
//                var x = mgf[0].Spectrum[i].Mz;
//                var y = mgf[0].Spectrum[i].Intensity;
//                var size = 2;
//                var colorValue = 1;
//                scatterSeries.Points.Add(new ScatterPoint(x, y, size, colorValue));

//                var annotation = new LineAnnotation();
//                annotation.Color = OxyColors.MediumPurple;
//                annotation.MinimumY = 0;
//                annotation.MaximumY = mgf[0].Spectrum[i].Intensity;
//                annotation.X = mgf[0].Spectrum[i].Mz;
//                annotation.LineStyle = LineStyle.Solid;
//                annotation.Type = LineAnnotationType.Vertical;
//                annotation.StrokeThickness = 1.7;
//                model.Annotations.Add(annotation);
//            }

//            var customAxis = new RangeColorAxis { Key = "customColors" };
//            customAxis.AddRange(0, 2000, OxyColors.Purple);
//            model.Axes.Add(customAxis);
//            model.Series.Add(scatterSeries);

//            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "m/z", AbsoluteMinimum = 0});
//            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Relative Abundance", AbsoluteMinimum = 0 });


//            this.MyModel = model;
//        }

//        public PlotModel MyModel { get; private set; }
//    }
//}
