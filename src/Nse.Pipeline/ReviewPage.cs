using System.Text.Json;

static class ReviewPage
{
    public static string Render(AnnotationDocument document, Dictionary<string, string> images, bool readOnly)
    {
        using var stream = typeof(ReviewPage).Assembly.GetManifestResourceStream("Nse.Pipeline.ReviewPage.html")!;
        using var reader = new StreamReader(stream);
        // Default JSON escaping prevents user strings from ending the script element.
        var payload = JsonSerializer.Serialize(new { document, images, readOnly }, ReviewWorkflow.Json);
        return reader.ReadToEnd().Replace("/*REVIEW_DATA*/ null", payload);
    }
}
