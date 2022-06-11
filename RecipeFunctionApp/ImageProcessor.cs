using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RecipeFunctionApp
{
    public class ImageProcessor
    {
        private readonly IConfiguration _configuration;

        public ImageProcessor(IConfiguration configuration)
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

        public async Task<List<string>> ExtractText(ComputerVisionClient client, Stream imageStream)
        {           
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
    }
}
