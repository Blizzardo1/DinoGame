using SharpSDL3.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoGame; 
internal static class Extensions {
    public static bool IsEmpty(this string s) =>
            string.IsNullOrEmpty(s)
         || string.IsNullOrWhiteSpace(s);

    public static string ToStringRepresentation(this Color color) =>
        $"{{R: {color.R}, G: {color.G}, B: {color.B}, A: {color.A}}}";
}
