using Microsoft.ML;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using CryptoGRAPHic.Models;

namespace CryptoGRAPHic.Services
{
    public class MLService
    {
        private readonly MLContext _mlContext;

        public MLService()
        {
            _mlContext = new MLContext();
        }

        public void TrainAndPredict(List<List<double>> priceData)
        {
            var prices = priceData.Select(x => (float)x[1]).ToArray();
            var predictedPrices = TrainModel(prices);

            // Generate future dates for predictions in the same format as price_actual.json
            var lastDateUnix = priceData.Last()[0];  // Get the last date from price_actual.json
            var nextDateUnix = lastDateUnix + 86400000;  // Add one day (86400000 ms)
            var predictionDates = Enumerable.Range(1, predictedPrices.Length)
                                             .Select(i => nextDateUnix + (i - 1) * 86400000)
                                             .ToList();

            // Format the predictions to match price_actual.json
            var predictedData = predictedPrices.Zip(predictionDates, (price, date) => new List<double> { date, price })
                                               .ToList();

            // Save predicted prices with their dates in the same format as price_actual.json
            var predictionsJson = JsonConvert.SerializeObject(new { prices = predictedData });
            File.WriteAllText("wwwroot/price_predicted.json", predictionsJson);
        }

        private float[] TrainModel(float[] data)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(data.Select(x => new TimeSeriesData { Value = x }));

            var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedValues",
                inputColumnName: nameof(TimeSeriesData.Value),
                windowSize: 4,
                seriesLength: data.Length,
                trainSize: (int)(data.Length * 0.95),
                horizon: 7,
                isAdaptive: true,
                discountFactor: 1,
                rankSelectionMethod: Microsoft.ML.Transforms.TimeSeries.RankSelectionMethod.Exact,
                shouldStabilize: true
            );

            var model = forecastingPipeline.Fit(dataView);
            var forecastData = model.Transform(dataView);

            return _mlContext.Data
                .CreateEnumerable<ForecastedResult>(forecastData, reuseRowObject: false)
                .FirstOrDefault()?.ForecastedValues ?? new float[7];
        }

        private class TimeSeriesData
        {
            public float Value { get; set; }
        }

        private class ForecastedResult
        {
            public float[] ForecastedValues { get; set; } = new float[0];
        }
    }
}
