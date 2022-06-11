namespace RecipeProcessing.Models.TextAnalytics
{
    public class EntitiesResult
    {
        public string? Phrase { get; set; }
        public IEnumerable<string>? Categories { get; set; }
    }
}
