namespace RecipeProcessing.Models.Images
{
    public class ImageCategory : CognitiveServicesResult
    {
        public string? Name { get; set; }
        public IEnumerable<ImageCategoryDetail>? CategoryDetails { get; set; }
    }
}
