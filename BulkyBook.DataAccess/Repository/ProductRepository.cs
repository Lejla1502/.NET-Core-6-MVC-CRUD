using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ML;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;

        //creating MLContext to be shared accross the model creation workflow objects
        //this is neccessary so we know that the model has not been trined yet
        static MLContext mlContext = null;
        static ITransformer model = null;
        public ProductRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public IEnumerable<Product> GetRecommended(string appUserId, int productId)
        {
            if (mlContext == null)
            {
                mlContext = new MLContext();


                var tmpData = _db.Reviews.ToList();
                var data = new List<ProductRating>();

                var trainData = _db.Reviews.ToList().Select(s => new { userId = s.ApplicationUserId, productId = s.ProductId, rating = (float)s.Rating });
                foreach (var x in trainData)
                {
                    data.Add(new ProductRating()
                    {
                        userId = x.userId,
                        productId = x.productId,
                        Label = x.rating
                    });

                }


                //STEP 2: Read the training data which will be used to train the movie recommendation model
                //The schema for training data is defined by type 'TInput' in LoadFromTextFile<TInput>() method.
                IDataView trainingDataView = mlContext.Data.LoadFromEnumerable(data);

                //STEP 3: Transform your data by encoding the two features userId and movieID. These encoded features will be provided as
                //        to our MatrixFactorizationTrainer.
                var dataProcessingPipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName:nameof(ProductRating.userId))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "productIdEncoded", inputColumnName: nameof(ProductRating.productId)));

                //Specify the options for MatrixFactorization trainer
                MatrixFactorizationTrainer.Options options = new MatrixFactorizationTrainer.Options();
                options.MatrixColumnIndexColumnName = "userIdEncoded";
                options.MatrixRowIndexColumnName = "productIdEncoded";
                options.LabelColumnName = "Label";

                options.NumberOfIterations = 20;
                options.ApproximationRank = 100;

                //STEP 4: Create the training pipeline
                var trainingPipeLine = dataProcessingPipeline.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));

                //STEP 5: Train the model fitting to the DataSet
                Console.WriteLine("=============== Training the model ===============");
                model = trainingPipeLine.Fit(trainingDataView);


                var testData = new List<ProductRating>()
            {
                new ProductRating { userId = "1", productId = 1, Label=4 },
                new ProductRating { userId = "1", productId = 2, Label=5 },
                new ProductRating { userId = "1", productId = 3, Label=3 },
                new ProductRating { userId = "2", productId = 1, Label=4 },
                new ProductRating { userId = "3", productId = 3, Label=2 },
                new ProductRating { userId = "2", productId = 4, Label=1 },
                new ProductRating { userId = "3", productId = 1, Label=3 },
                new ProductRating { userId = "3", productId = 3, Label=1 },
                new ProductRating { userId = "4", productId = 1, Label=3 },
                new ProductRating { userId = "5", productId = 2, Label=4 }

            };



                //STEP 6: Evaluate the model performance 
                Console.WriteLine("=============== Evaluating the model ===============");
                IDataView testDataView = mlContext.Data.LoadFromEnumerable(testData);
                var prediction = model.Transform(testDataView);
                var metrics = mlContext.Regression.Evaluate(prediction, labelColumnName: "Label", scoreColumnName: "Score");
                Console.WriteLine("The model evaluation metrics RootMeanSquaredError:" + metrics.RootMeanSquaredError);

            }
            // ITransformer model = null;

            //STEP 7:  Try/test a single prediction by predicting a single movie rating for a specific user
            var predictionengine = mlContext.Model.CreatePredictionEngine<ProductRating, ProductRatingPrediction>(model);
            /* Make a single movie rating prediction, the scores are for a particular user and will range from 1 - 5. 
               The higher the score the higher the likelyhood of a user liking a particular movie.
               You can recommend a movie to a user if say rating > 3.5.*/


            //NEED TO FIGURE OUT THIS PART
            //in my example on github it seems like its taking users that have added the product
            //here we need all buyers, not admins
            //   |
            //   |
            //   |
            //  \|/

            /*var allItems = _db.Products.Where(p => p.Id != productId).ToList();

            var listRecommendedProducts = new List<Tuple<Product, float>>();

            foreach (var x in allItems)
            {
                var productRatingPrediction = predictionengine.Predict(
                    new ProductRating()
                    {
                        //Example rating prediction for userId = 6, movieId = 10 (GoldenEye)
                        userId = ,
                        productId = x.Id
                    }
                );

                listRecommendedProducts.Add(new Tuple<Product, float>(x, productRatingPrediction.Score));

            }

            var finalResult = listRecommendedProducts.OrderByDescending(o => o.Item2).Select(s => s.Item1).Take(3).ToList();


            return finalResult; // _mapper.Map<List<Model.Proizvod>>(finalResult);
            */
            return null;
        }

        public void Update(Product obj)
        {
            //here we use a better practice
            //instead of working directly with DB model, we place it in variable and then update
            //properties we need
            //this way we can restrict Update

            var objFromDb = _db.Products.FirstOrDefault(x=>x.Id== obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title= obj.Title;
                objFromDb.Description= obj.Description;
                objFromDb.ISBN= obj.ISBN;
                objFromDb.Author= obj.Author;
                objFromDb.ListPrice= obj.ListPrice;
                objFromDb.Price= obj.Price;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Price100 = obj.Price100;
                objFromDb.CategoryId= obj.CategoryId;
                objFromDb.CoverTypeId= obj.CoverTypeId;

                if (obj.ImageUrl != null)
                    objFromDb.ImageUrl = obj.ImageUrl;
            }
        }

        public void UpdateStatus(int id)
        {
            
            var productFromDb = _db.Products.FirstOrDefault(x => x.Id == id);
           
            if (productFromDb != null)
            {
                if (productFromDb.IsFavourite)
                    productFromDb.IsFavourite = false;
                else
                    productFromDb.IsFavourite = true;
                
            }
            //_db.OrderHeaders.Update(orderFromDb);
            
        }
    }
}
