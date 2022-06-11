namespace RecipeProcessing.Models.Images
{
    public class ImageObject : CognitiveServicesResult
    {
        public string? Property { get; set; }
        public int X { get; set; }
        public int Y { get;set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
