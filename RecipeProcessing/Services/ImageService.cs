using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using RecipeProcessing.Models.Images;

namespace RecipeProcessing.Services
{
    public class ImageService
    {
        private readonly IConfiguration _configuration;

        public ImageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private ComputerVisionClient Authenticate()
        {
            var visionEndpoint = _configuration["visionEndpoint"];
            var visionKey = _configuration["visionKey"];

            var client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(visionKey))
              { Endpoint = visionEndpoint };
            return client;
        }

        public async Task<List<string>> ExtractText(Stream imageStream)
        {
            var client = Authenticate();
            // Read text from URL
            var textHeaders = await client.ReadInStreamAsync(imageStream);
            // After the request, get the operation location (operation ID)
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);

            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;            
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));

            // Display the found text.
            //Console.WriteLine();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            var lines = new List<string>();
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    //Console.WriteLine(line.Text);
                    lines.Add(line.Text);
                }
            }
            //Console.WriteLine();

            return lines;
        }

        public async Task<ImageAnalysisResult> AnalyzeImage(string imageUrl)
        {
            // Creating a list that defines the features to be extracted from the image. 

            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };

            List<Details?> details = new List<Details?>() { Details.Landmarks, Details.Celebrities };

            Console.WriteLine($"Analyzing the image {Path.GetFileName(imageUrl)}...");
            Console.WriteLine();
            // Analyze the URL image 
            var client = Authenticate();
            ImageAnalysis results = await client.AnalyzeImageAsync(imageUrl, visualFeatures: features, details);

            return ProcessAnalysisResults(results);
        }

        public async Task<ImageAnalysisResult> AnalyzeImage(Stream imageStream)
        {
            // Creating a list that defines the features to be extracted from the image. 

            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };

            List<Details?> details = new List<Details?>() { Details.Landmarks, Details.Celebrities };


            // Analyze the URL image 
            var client = Authenticate();
            ImageAnalysis results = await client.AnalyzeImageInStreamAsync(imageStream, visualFeatures: features, details);

            return ProcessAnalysisResults(results);
        }

        private ImageAnalysisResult ProcessAnalysisResults(ImageAnalysis results)
        {
            var imageAnalysisResult = new ImageAnalysisResult();

            imageAnalysisResult.Captions = results.Description.Captions.Select(cap => new Caption
            {
                Text = cap.Text,
                Score = cap.Confidence
            });

            //IMAGE CATEGORY
            imageAnalysisResult.Categories = results.Categories.Select(cat => new ImageCategory
            {
                Name = cat.Name,
                Score = cat.Score,
                CategoryDetails = cat.Detail?.Celebrities?.Select(details => new ImageCategoryDetail
                {
                    Name = details.Name,
                    Score = details.Confidence
                })
            });

            //IMAGE TAGS
            // Image tags and their confidence score
            imageAnalysisResult.Tags = results.Tags.Select(tag => new Models.Images.ImageTag
            {
                Name = tag.Name,
                Score = tag.Confidence
            });


            // Objects
            imageAnalysisResult.Objects = results.Objects.Select(obj => new ImageObject
            {
                Property = obj.ObjectProperty,
                Score = obj.Confidence,
                X = obj.Rectangle.X,
                Y = obj.Rectangle.Y,
                Height = obj.Rectangle.H,
                Width = obj.Rectangle.W
            });            

            return imageAnalysisResult;
        }
    }
}
