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

        public void GetLinearRegression()
        {
            var mlContext = new MLContext();

            var dataview = mlContext.Data.LoadFromTextFile<NormalizedCustomer>(_trainDataLocation, hasHeader: true, separatorChar: ',');

            var featureColumnArray = GetFeatureColumnArray(dataview);

            var model = Train(featureColumnArray, dataview, mlContext);

            var testDataview = mlContext.Data.LoadFromTextFile<NormalizedCustomer>(_testDataLocation, hasHeader: true, separatorChar: ',');

            LinearRegressionPFI.PFI(mlContext, dataview);

            LinearRegressionResults.Evaluate(dataview, mlContext, model);

            LinearRegressionResults.Evaluate(testDataview, mlContext, model);

            CrossValidate(mlContext, featureColumnArray, dataview, testDataview);

        }

        private string[] GetFeatureColumnArray(IDataView data)
        {
            return data.Schema.Select(column => column.Name).Where(columnName => columnName != "Label").ToArray();
             
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

            LinearRegressionResults.Evaluate(testTransformedData, mlContext, topModel);
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

    }
}
