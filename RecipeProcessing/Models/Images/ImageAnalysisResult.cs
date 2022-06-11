namespace RecipeProcessing.Models.Images
{
    public class ImageAnalysisResult
    {
        public IEnumerable<Caption>? Captions { get; set; }
        public IEnumerable<ImageCategory>? Categories { get; set; }        
        public IEnumerable<ImageTag>? Tags { get; set; }
        public IEnumerable<ImageObject>? Objects { get; set; }
    }
}
