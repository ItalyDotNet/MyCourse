using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Options;
using MyCourse.Models.Options;

namespace MyCourse.Models.Services.Infrastructure
{
    public class MicrosoftAzureImageValidator : IImageValidator
    {
        private readonly IOptionsMonitor<ImageValidationOptions> imageValidationOptions;

        public MicrosoftAzureImageValidator(IOptionsMonitor<ImageValidationOptions> imageValidationOptions)
        {
            this.imageValidationOptions = imageValidationOptions;
        }

        public async Task<bool> IsValidAsync(IFormFile formFile)
        {
            var options = imageValidationOptions.CurrentValue;
            var credentials = new ApiKeyServiceClientCredentials(options.Key);
            using var client = new ComputerVisionClient(credentials)
            {
                Endpoint = options.Endpoint
            };
            var features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Adult
            };
            using Stream stream = formFile.OpenReadStream();
            ImageAnalysis result = await client.AnalyzeImageInStreamAsync(stream, features);
            bool isValid = result.Adult.AdultScore <= options.MaximumAdultScore;
            return isValid;
        }
    }
}