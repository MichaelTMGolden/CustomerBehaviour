using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace CustomerBehaviour.Domain.LinearRegression
{
    public class LinearRegressionPFI
    {
        public static void PFI(MLContext mlContext, IDataView data)
        {
            // Define trainer options.
            var options = new OlsTrainer.Options
            {
                // Larger values leads to smaller (closer to zero) model parameters.
                L2Regularization = 0.03f,
                // Whether to compute standard error and other statistics of model
                // parameters.
                CalculateStatistics = false
            };

            // 1. Get the column name of input features.
            string[] featureColumnNames = data.Schema.Select(column => column.Name).Where(columnName => columnName != "Label").ToArray();

            // 2. Define estimator with data pre-processing steps
            IEstimator<ITransformer> dataPrepEstimator = mlContext.Transforms.Concatenate("Features", featureColumnNames).Append(mlContext.Transforms.NormalizeMinMax("Features"));

            // 3. Create transformer using the data pre-processing estimator
            ITransformer dataPrepTransformer = dataPrepEstimator.Fit(data);

            // 4. Pre-process the training data
            IDataView preprocessedTrainData = dataPrepTransformer.Transform(data);

            // 5. Define Stochastic Dual Coordinate Ascent machine learning estimator
            var sdcaEstimator = mlContext.Regression.Trainers.Ols(options);

            // 6. Train machine learning model
            var sdcaModel = sdcaEstimator.Fit(preprocessedTrainData);

            ImmutableArray<RegressionMetricsStatistics> permutationFeatureImportance = mlContext.Regression.PermutationFeatureImportance(sdcaModel, preprocessedTrainData, permutationCount: 3);

            // Order features by importance
            var featureImportanceMetrics =
                permutationFeatureImportance
                    .Select((metric, index) => new { index, metric.RSquared })
                    .OrderByDescending(myFeatures => Math.Abs(myFeatures.RSquared.Mean));

            Console.WriteLine("Feature\tPFI");

            foreach (var feature in featureImportanceMetrics)
            {
                Console.WriteLine($"{featureColumnNames[feature.index],-20}|\t{feature.RSquared.Mean:F6}");
            }
        }
    }
}
