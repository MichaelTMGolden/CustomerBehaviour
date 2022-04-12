using CustomerBehaviour.Definitions;
using CustomerBehaviour.Interfaces;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace CustomerBehaviour.Domain.LinearRegression
{
    public class LinearRegression
    {
        private readonly IDataReader _reader;

        private const string _trainDataLocation = @"C:\CustomerBehaviour\Analysis\Normalized-Data\PointsbetCustomerData1.csv";

        private const string _testDataLocation = @"C:\CustomerBehaviour\Analysis\Normalized-Data\testData.csv";
        public LinearRegression(IDataReader reader)
        {
            _reader = reader;
        }

        public LinearRegressionResults GetLinearRegression()
        {
            var mlContext = new MLContext();

            var dataview = mlContext.Data.LoadFromTextFile<NormalizedCustomer>(_trainDataLocation, hasHeader: true, separatorChar: ',');

            var featureColumnArray = GetFeatureColumnArray(dataview);

            var model = Train(featureColumnArray, dataview, mlContext);

            var testDataview = mlContext.Data.LoadFromTextFile<NormalizedCustomer>(_testDataLocation, hasHeader: true, separatorChar: ',');

            PFI(mlContext, dataview);

            Evaluate(dataview, mlContext, model);

            Evaluate(testDataview, mlContext, model);

            CrossValidate(mlContext, featureColumnArray, dataview, testDataview);
             
            throw new NotImplementedException();
        }

        private string[] GetFeatureColumnArray(IDataView data)
        {
            return data.Schema.Select(column => column.Name).Where(columnName => columnName != "Label").ToArray();
             
        }

        private void PFI(MLContext mlContext, IDataView data)
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
            IEstimator<ITransformer> dataPrepEstimator =mlContext.Transforms.Concatenate("Features", featureColumnNames).Append(mlContext.Transforms.NormalizeMinMax("Features"));

            // 3. Create transformer using the data pre-processing estimator
            ITransformer dataPrepTransformer = dataPrepEstimator.Fit(data);

            // 4. Pre-process the training data
            IDataView preprocessedTrainData = dataPrepTransformer.Transform(data);

            // 5. Define Stochastic Dual Coordinate Ascent machine learning estimator
            var sdcaEstimator = mlContext.Regression.Trainers.Ols(options);

            // 6. Train machine learning model
            var sdcaModel = sdcaEstimator.Fit(preprocessedTrainData);

            ImmutableArray<RegressionMetricsStatistics> permutationFeatureImportance =mlContext.Regression.PermutationFeatureImportance(sdcaModel, preprocessedTrainData, permutationCount: 3);

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

        private void CrossValidate(MLContext mlContext, string[] featureColumnNames, IDataView data, IDataView testDataview)
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

            IEstimator<ITransformer> dataPrepEstimator = mlContext.Transforms.Concatenate("Features", featureColumnNames);

            ITransformer dataPrepTransformer = dataPrepEstimator.Fit(data);

            IDataView transformedData = dataPrepTransformer.Transform(data);

            // Define StochasticDualCoordinateAscent algorithm estimator
            IEstimator<ITransformer> sdcaEstimator = mlContext.Regression.Trainers.Ols(options);

            // Apply 5-fold cross validation
            var cvResults = mlContext.Regression.CrossValidate(transformedData, sdcaEstimator, numberOfFolds: 10);

            IEnumerable<double> rSquared = cvResults.Select(fold => fold.Metrics.RSquared);

            // Select all models
            ITransformer[] models =
                cvResults
                    .OrderByDescending(fold => fold.Metrics.RSquared)
                    .Select(fold => fold.Model)
                    .ToArray();

            // Get Top Model
            ITransformer topModel = models[0];

            IDataView testTransformedData = dataPrepTransformer.Transform(testDataview);

            Evaluate(testTransformedData, mlContext, topModel);
        }

        public static ITransformer Train(string[] featureColumnNames, IDataView dataview, MLContext mlContext)
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
            var pipeline = mlContext.Transforms.Concatenate("Features", featureColumnNames)
                .Append(mlContext.Regression.Trainers.Ols(options));

            //Create the model
            var model = pipeline.Fit(dataview);

            //Return the trained model
            return model;
        }

        private static void Evaluate(IDataView testSet, MLContext mlContext, ITransformer model)
        {
           
            var predictions = model.Transform(testSet);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics output         ");
            Console.WriteLine($"*------------------------------------------------");

            Console.WriteLine($"*       R Squared Score:      {metrics.RSquared:0.###}");

            Console.WriteLine($"*       Mean Absolute Error:      {metrics.MeanAbsoluteError:#.###}");

            Console.WriteLine($"*       Root-Mean-Squared Error:      {metrics.RootMeanSquaredError:#.###}");

        }

    }
}
