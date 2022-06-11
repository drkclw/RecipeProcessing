using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace RecipeFunctionApp
{
    public class RecipeImageProcess
    {
        private readonly ImageProcessor _imageProcessor;

        public RecipeImageProcess(ImageProcessor imageProcessor)
        {
            _imageProcessor = imageProcessor;
        }

        [FunctionName("RecipeImageProcess")]
        public void Run([BlobTrigger("recipeimages/{name}", Connection = "")]Stream myBlob, string name, ILogger log)
        {            
            //_imageProcessor.ExtractText(myBlob);
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
