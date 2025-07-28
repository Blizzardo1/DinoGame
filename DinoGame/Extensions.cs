using SharpSDL3.Structs;


namespace DinoGame; 
internal static class Extensions {
    public static bool IsEmpty(this string s) =>
            string.IsNullOrEmpty(s)
         || string.IsNullOrWhiteSpace(s);

    public static string ToStringRepresentation(this Color color) =>
        $"{{R: {color.R}, G: {color.G}, B: {color.B}, A: {color.A}}}";
    public static string ToStringRepresentation(this FRect rect) =>
        $"{{X: {rect.X}, Y: {rect.Y}, W: {rect.W}, H: {rect.H}}}";
    public static string ToStringRepresentation(this Rect rect) =>
        $"{{X: {rect.X}, Y: {rect.Y}, W: {rect.W}, H: {rect.H}}}";
}
