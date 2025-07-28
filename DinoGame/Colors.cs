/***
     A Game where you need to evade the enemy to gain points.
    Copyright (C) 2025  Adonis Deliannis (Blizzardo1)

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using SharpSDL3.Structs;

namespace DinoGame;

public enum KnownColor {
    Transparent = 0x00FFFFFF,
    AliceBlue = unchecked((int)0xFFF0F8FF),
    AntiqueWhite = unchecked((int)0xFFFAEBD7),
    Aqua = unchecked((int)0xFF00FFFF),
    Aquamarine = unchecked((int)0xFF7FFFD4),
    Azure = unchecked((int)0xFFF0FFFF),
    Beige = unchecked((int)0xFFF5F5DC),
    Bisque = unchecked((int)0xFFFFE4C4),
    Black = unchecked((int)0xFF000000),
    BlanchedAlmond = unchecked((int)0xFFFFEBCD),
    Blue = unchecked((int)0xFF0000FF),
    BlueViolet = unchecked((int)0xFF8A2BE2),
    Brown = unchecked((int)0xFFA52A2A),
    BurlyWood = unchecked((int)0xFFDEB887),
    CadetBlue = unchecked((int)0xFF5F9EA0),
    Chartreuse = unchecked((int)0xFF7FFF00),
    Chocolate = unchecked((int)0xFFD2691E),
    Coral = unchecked((int)0xFFFF7F50),
    CornflowerBlue = unchecked((int)0xFF6495ED),
    Cornsilk = unchecked((int)0xFFFFF8DC),
    Crimson = unchecked((int)0xFFDC143C),
    Cyan = unchecked((int)0xFF00A0FF),
    DarkBlue = unchecked((int)0xFF00008B),
    DarkCyan = unchecked((int)0xFF008B8B),
    DarkGoldenrod = unchecked((int)0xFFB8860B),
    DarkGray = unchecked((int)0xFFA9A9A9),
    DarkGreen = unchecked((int)0xFF006400),
    DarkKhaki = unchecked((int)0xFFBDB76B),
    DarkMagenta = unchecked((int)0xFF8B008B),
    DarkOliveGreen = unchecked((int)0xFF556B2F),
    DarkOrange = unchecked((int)0xFFFF8C00),
    DarkOrchid = unchecked((int)0xFF9932CC),
    DarkRed = unchecked((int)0xFF8B0000),
    DarkSalmon = unchecked((int)0xFFE9967A),
    DarkSeaGreen = unchecked((int)0xFF8FBC8B),
    DarkSlateBlue = unchecked((int)0xFF483D8B),
    DarkSlateGray = unchecked((int)0xFF2F4F4F),
    DarkTurquoise = unchecked((int)0xFF00CED1),
    DarkViolet = unchecked((int)0xFF9400D3),
    DeepPink = unchecked((int)0xFFFF1493),
    DeepSkyBlue = unchecked((int)0xFF00BFFF),
    DimGray = unchecked((int)0xFF696969),
    DodgerBlue = unchecked((int)0xFF1E90FF),
    Firebrick = unchecked((int)0xFFB22222),
    FloralWhite = unchecked((int)0xFFFFFAF0),
    ForestGreen = unchecked((int)0xFF228B22),
    Gainsboro = unchecked((int)0xFFDCDCDC),
    GhostWhite = unchecked((int)0xFFF8F8FF),
    Gold = unchecked((int)0xFFFFD700),
    Goldenrod = unchecked((int)0xFFDAA520),
    Gray = unchecked((int)0xFF808080),
    Green = unchecked((int)0xFF008000),
    GreenYellow = unchecked((int)0xFFADFF2F),
    Honeydew = unchecked((int)0xFFF0FFF0),
    HotPink = unchecked((int)0xFFFF69B4),
    IndianRed = unchecked((int)0xFFCD5C5C),
    Indigo = unchecked((int)0xFF4B0082),
    Ivory = unchecked((int)0xFFFFFFF0),
    Khaki = unchecked((int)0xFFF0E68C),
    Lavender = unchecked((int)0xFFE6E6FA),
    LavenderBlush = unchecked((int)0xFFFFF0F5),
    LawnGreen = unchecked((int)0xFF7CFC00),
    LemonChiffon = unchecked((int)0xFFFFFACD),
    LightBlue = unchecked((int)0xFFADD8E6),
    LightCoral = unchecked((int)0xFFF08080),
    LightCyan = unchecked((int)0xFFE0FFFF),
    LightGoldenrodYellow = unchecked((int)0xFFFAFAD2),
    LightGray = unchecked((int)0xFFD3D3D3),
    LightGreen = unchecked((int)0xFF90EE90),
    LightPink = unchecked((int)0xFFFFB6C1),
    LightSalmon = unchecked((int)0xFFFFA07A),
    LightSeaGreen = unchecked((int)0xFF20B2AA),
    LightSkyBlue = unchecked((int)0xFF87CEFA),
    LightSlateGray = unchecked((int)0xFF778899),
    LightSteelBlue = unchecked((int)0xFFB0C4DE),
    LightYellow = unchecked((int)0xFFFFFFE0),
    Lime = unchecked((int)0xFF00FF00),
    LimeGreen = unchecked((int)0xFF32CD32),
    Linen = unchecked((int)0xFFFAF0E6),
    Magenta = unchecked((int)0xFFFF00FF),
    Maroon = unchecked((int)0xFF800000),
    MediumAquamarine = unchecked((int)0xFF66CDAA),
    MediumBlue = unchecked((int)0xFF0000CD),
    MediumOrchid = unchecked((int)0xFFBA55D3),
    MediumPurple = unchecked((int)0xFF9370DB),
    MediumSeaGreen = unchecked((int)0xFF3CB371),
    MediumSlateBlue = unchecked((int)0xFF7B68EE),
    MediumSpringGreen = unchecked((int)0xFF00FA9A),
    MediumTurquoise = unchecked((int)0xFF48D1CC),
    MediumVioletRed = unchecked((int)0xFFC71585),
    MidnightBlue = unchecked((int)0xFF191970),
    MintCream = unchecked((int)0xFFF5FFFA),
    MistyRose = unchecked((int)0xFFFFE4E1),
    Moccasin = unchecked((int)0xFFFFE4B5),
    NavajoWhite = unchecked((int)0xFFFFDEAD),
    Navy = unchecked((int)0xFF000080),
    OldLace = unchecked((int)0xFFFDF5E6),
    Olive = unchecked((int)0xFF808000),
    OliveDrab = unchecked((int)0xFF6B8E23),
    Orange = unchecked((int)0xFFFFA500),
    OrangeRed = unchecked((int)0xFFFF4500),
    Orchid = unchecked((int)0xFFDA70D6),
    PaleGoldenrod = unchecked((int)0xFFEEE8AA),
    PaleGreen = unchecked((int)0xFF98FB98),
    PaleTurquoise = unchecked((int)0xFFAFEEEE),
    PaleVioletRed = unchecked((int)0xFFDB7093),
    PapayaWhip = unchecked((int)0xFFFFEFD5),
    PeachPuff = unchecked((int)0xFFFFDAB9),
    Peru = unchecked((int)0xFFCD853F),
    Pink = unchecked((int)0xFFFFC0CB),
    Plum = unchecked((int)0xFFDDA0DD),
    PowderBlue = unchecked((int)0xFFB0E0E6),
    Purple = unchecked((int)0xFF800080),
    Red = unchecked((int)0xFFFF0000),
    RosyBrown = unchecked((int)0xFFBC8F8F),
    RoyalBlue = unchecked((int)0xFF4169E1),
    SaddleBrown = unchecked((int)0xFF8B4513),
    Salmon = unchecked((int)0xFFFA8072),
    SandyBrown = unchecked((int)0xFFF4A460),
    SeaGreen = unchecked((int)0xFF2E8B57),
    SeaShell = unchecked((int)0xFFFFF5EE),
    Sienna = unchecked((int)0xFFA0522D),
    Silver = unchecked((int)0xFFC0C0C0),
    SkyBlue = unchecked((int)0xFF87CEEB),
    SlateBlue = unchecked((int)0xFF6A5ACD),
    SlateGray = unchecked((int)0xFF708090),
    Snow = unchecked((int)0xFFFFFAFA),
    SpringGreen = unchecked((int)0xFF00FF7F),
    SteelBlue = unchecked((int)0xFF4682B4),
    Tan = unchecked((int)0xFFD2B48C),
    Teal = unchecked((int)0xFF008080),
    Thistle = unchecked((int)0xFFD8BFD8),
    Tomato = unchecked((int)0xFFFF6347),
    Turquoise = unchecked((int)0xFF40E0D0),
    Violet = unchecked((int)0xFFEE82EE),
    Wheat = unchecked((int)0xFFF5DEB3),
    White = unchecked((int)0xFFFFFFFF),
    WhiteSmoke = unchecked((int)0xFFF5F5F5),
    Yellow = unchecked((int)0xFFFFFF00),
    YellowGreen = unchecked((int)0xFF9ACD32)
}

public static class Colors {
    public static Color Transparent => FromKnownColor(KnownColor.Transparent);
    public static Color Black => FromKnownColor(KnownColor.Black);
    public static Color Navy => FromKnownColor(KnownColor.Navy);
    public static Color DarkBlue => FromKnownColor(KnownColor.DarkBlue);
    public static Color MediumBlue => FromKnownColor(KnownColor.MediumBlue);
    public static Color Blue => FromKnownColor(KnownColor.Blue);
    public static Color DarkGreen => FromKnownColor(KnownColor.DarkGreen);
    public static Color Green => FromKnownColor(KnownColor.Green);
    public static Color Teal => FromKnownColor(KnownColor.Teal);
    public static Color DarkCyan => FromKnownColor(KnownColor.DarkCyan);
    public static Color DeepSkyBlue => FromKnownColor(KnownColor.DeepSkyBlue);
    public static Color DarkTurquoise => FromKnownColor(KnownColor.DarkTurquoise);
    public static Color MediumSpringGreen => FromKnownColor(KnownColor.MediumSpringGreen);
    public static Color Lime => FromKnownColor(KnownColor.Lime);
    public static Color SpringGreen => FromKnownColor(KnownColor.SpringGreen);
    public static Color Aqua => FromKnownColor(KnownColor.Aqua);
    public static Color Cyan => FromKnownColor(KnownColor.Cyan);
    public static Color MidnightBlue => FromKnownColor(KnownColor.MidnightBlue);
    public static Color DodgerBlue => FromKnownColor(KnownColor.DodgerBlue);
    public static Color LightSeaGreen => FromKnownColor(KnownColor.LightSeaGreen);
    public static Color ForestGreen => FromKnownColor(KnownColor.ForestGreen);
    public static Color SeaGreen => FromKnownColor(KnownColor.SeaGreen);
    public static Color DarkSlateGray => FromKnownColor(KnownColor.DarkSlateGray);
    public static Color LimeGreen => FromKnownColor(KnownColor.LimeGreen);
    public static Color MediumSeaGreen => FromKnownColor(KnownColor.MediumSeaGreen);
    public static Color Turquoise => FromKnownColor(KnownColor.Turquoise);
    public static Color RoyalBlue => FromKnownColor(KnownColor.RoyalBlue);
    public static Color SteelBlue => FromKnownColor(KnownColor.SteelBlue);
    public static Color DarkSlateBlue => FromKnownColor(KnownColor.DarkSlateBlue);
    public static Color MediumTurquoise => FromKnownColor(KnownColor.MediumTurquoise);
    public static Color Indigo => FromKnownColor(KnownColor.Indigo);
    public static Color DarkOliveGreen => FromKnownColor(KnownColor.DarkOliveGreen);
    public static Color CadetBlue => FromKnownColor(KnownColor.CadetBlue);
    public static Color CornflowerBlue => FromKnownColor(KnownColor.CornflowerBlue);
    public static Color MediumAquamarine => FromKnownColor(KnownColor.MediumAquamarine);
    public static Color DimGray => FromKnownColor(KnownColor.DimGray);
    public static Color SlateBlue => FromKnownColor(KnownColor.SlateBlue);
    public static Color OliveDrab => FromKnownColor(KnownColor.OliveDrab);
    public static Color SlateGray => FromKnownColor(KnownColor.SlateGray);
    public static Color LightSlateGray => FromKnownColor(KnownColor.LightSlateGray);
    public static Color MediumSlateBlue => FromKnownColor(KnownColor.MediumSlateBlue);
    public static Color LawnGreen => FromKnownColor(KnownColor.LawnGreen);
    public static Color Chartreuse => FromKnownColor(KnownColor.Chartreuse);
    public static Color Aquamarine => FromKnownColor(KnownColor.Aquamarine);
    public static Color Maroon => FromKnownColor(KnownColor.Maroon);
    public static Color Purple => FromKnownColor(KnownColor.Purple);
    public static Color Olive => FromKnownColor(KnownColor.Olive);
    public static Color Gray => FromKnownColor(KnownColor.Gray);
    public static Color SkyBlue => FromKnownColor(KnownColor.SkyBlue);
    public static Color LightSkyBlue => FromKnownColor(KnownColor.LightSkyBlue);
    public static Color BlueViolet => FromKnownColor(KnownColor.BlueViolet);
    public static Color DarkRed => FromKnownColor(KnownColor.DarkRed);
    public static Color DarkMagenta => FromKnownColor(KnownColor.DarkMagenta);
    public static Color SaddleBrown => FromKnownColor(KnownColor.SaddleBrown);
    public static Color DarkSeaGreen => FromKnownColor(KnownColor.DarkSeaGreen);
    public static Color LightGreen => FromKnownColor(KnownColor.LightGreen);
    public static Color MediumPurple => FromKnownColor(KnownColor.MediumPurple);
    public static Color DarkViolet => FromKnownColor(KnownColor.DarkViolet);
    public static Color PaleGreen => FromKnownColor(KnownColor.PaleGreen);
    public static Color DarkOrchid => FromKnownColor(KnownColor.DarkOrchid);
    public static Color YellowGreen => FromKnownColor(KnownColor.YellowGreen);
    public static Color Sienna => FromKnownColor(KnownColor.Sienna);
    public static Color Brown => FromKnownColor(KnownColor.Brown);
    public static Color DarkGray => FromKnownColor(KnownColor.DarkGray);
    public static Color LightBlue => FromKnownColor(KnownColor.LightBlue);
    public static Color GreenYellow => FromKnownColor(KnownColor.GreenYellow);
    public static Color PaleTurquoise => FromKnownColor(KnownColor.PaleTurquoise);
    public static Color LightSteelBlue => FromKnownColor(KnownColor.LightSteelBlue);
    public static Color PowderBlue => FromKnownColor(KnownColor.PowderBlue);
    public static Color Firebrick => FromKnownColor(KnownColor.Firebrick);
    public static Color DarkGoldenrod => FromKnownColor(KnownColor.DarkGoldenrod);
    public static Color MediumOrchid => FromKnownColor(KnownColor.MediumOrchid);
    public static Color RosyBrown => FromKnownColor(KnownColor.RosyBrown);
    public static Color DarkKhaki => FromKnownColor(KnownColor.DarkKhaki);
    public static Color Silver => FromKnownColor(KnownColor.Silver);
    public static Color MediumVioletRed => FromKnownColor(KnownColor.MediumVioletRed);
    public static Color IndianRed => FromKnownColor(KnownColor.IndianRed);
    public static Color Peru => FromKnownColor(KnownColor.Peru);
    public static Color Chocolate => FromKnownColor(KnownColor.Chocolate);
    public static Color Tan => FromKnownColor(KnownColor.Tan);
    public static Color LightGray => FromKnownColor(KnownColor.LightGray);
    public static Color Thistle => FromKnownColor(KnownColor.Thistle);
    public static Color Orchid => FromKnownColor(KnownColor.Orchid);
    public static Color Goldenrod => FromKnownColor(KnownColor.Goldenrod);
    public static Color PaleVioletRed => FromKnownColor(KnownColor.PaleVioletRed);
    public static Color Crimson => FromKnownColor(KnownColor.Crimson);
    public static Color Gainsboro => FromKnownColor(KnownColor.Gainsboro);
    public static Color Plum => FromKnownColor(KnownColor.Plum);
    public static Color BurlyWood => FromKnownColor(KnownColor.BurlyWood);
    public static Color LightCyan => FromKnownColor(KnownColor.LightCyan);
    public static Color Lavender => FromKnownColor(KnownColor.Lavender);
    public static Color DarkSalmon => FromKnownColor(KnownColor.DarkSalmon);
    public static Color Violet => FromKnownColor(KnownColor.Violet);
    public static Color PaleGoldenrod => FromKnownColor(KnownColor.PaleGoldenrod);
    public static Color LightCoral => FromKnownColor(KnownColor.LightCoral);
    public static Color Khaki => FromKnownColor(KnownColor.Khaki);
    public static Color AliceBlue => FromKnownColor(KnownColor.AliceBlue);
    public static Color Honeydew => FromKnownColor(KnownColor.Honeydew);
    public static Color Azure => FromKnownColor(KnownColor.Azure);
    public static Color SandyBrown => FromKnownColor(KnownColor.SandyBrown);
    public static Color Wheat => FromKnownColor(KnownColor.Wheat);
    public static Color Beige => FromKnownColor(KnownColor.Beige);
    public static Color WhiteSmoke => FromKnownColor(KnownColor.WhiteSmoke);
    public static Color MintCream => FromKnownColor(KnownColor.MintCream);
    public static Color GhostWhite => FromKnownColor(KnownColor.GhostWhite);
    public static Color Salmon => FromKnownColor(KnownColor.Salmon);
    public static Color AntiqueWhite => FromKnownColor(KnownColor.AntiqueWhite);
    public static Color Linen => FromKnownColor(KnownColor.Linen);
    public static Color LightGoldenrodYellow => FromKnownColor(KnownColor.LightGoldenrodYellow);
    public static Color OldLace => FromKnownColor(KnownColor.OldLace);
    public static Color Red => FromKnownColor(KnownColor.Red);
    public static Color Magenta => FromKnownColor(KnownColor.Magenta);
    public static Color DeepPink => FromKnownColor(KnownColor.DeepPink);
    public static Color OrangeRed => FromKnownColor(KnownColor.OrangeRed);
    public static Color Tomato => FromKnownColor(KnownColor.Tomato);
    public static Color HotPink => FromKnownColor(KnownColor.HotPink);
    public static Color Coral => FromKnownColor(KnownColor.Coral);
    public static Color DarkOrange => FromKnownColor(KnownColor.DarkOrange);
    public static Color LightSalmon => FromKnownColor(KnownColor.LightSalmon);
    public static Color Orange => FromKnownColor(KnownColor.Orange);
    public static Color LightPink => FromKnownColor(KnownColor.LightPink);
    public static Color Pink => FromKnownColor(KnownColor.Pink);
    public static Color Gold => FromKnownColor(KnownColor.Gold);
    public static Color PeachPuff => FromKnownColor(KnownColor.PeachPuff);
    public static Color NavajoWhite => FromKnownColor(KnownColor.NavajoWhite);
    public static Color Moccasin => FromKnownColor(KnownColor.Moccasin);
    public static Color Bisque => FromKnownColor(KnownColor.Bisque);
    public static Color MistyRose => FromKnownColor(KnownColor.MistyRose);
    public static Color BlanchedAlmond => FromKnownColor(KnownColor.BlanchedAlmond);
    public static Color PapayaWhip => FromKnownColor(KnownColor.PapayaWhip);
    public static Color LavenderBlush => FromKnownColor(KnownColor.LavenderBlush);
    public static Color SeaShell => FromKnownColor(KnownColor.SeaShell);
    public static Color Cornsilk => FromKnownColor(KnownColor.Cornsilk);
    public static Color LemonChiffon => FromKnownColor(KnownColor.LemonChiffon);
    public static Color FloralWhite => FromKnownColor(KnownColor.FloralWhite);
    public static Color Snow => FromKnownColor(KnownColor.Snow);
    public static Color Yellow => FromKnownColor(KnownColor.Yellow);
    public static Color LightYellow => FromKnownColor(KnownColor.LightYellow);
    public static Color Ivory => FromKnownColor(KnownColor.Ivory);
    public static Color White => FromKnownColor(KnownColor.White);

    public static Color FromKnownColor(KnownColor knownColor) {
        // In ARGB format? Sheesh.... Should probably fix that...
        // Anywho......
        byte a = (byte)((int)knownColor >> 24 & 0xFF);
        byte r = (byte)((int)knownColor >> 16 & 0xFF);
        byte g = (byte)((int)knownColor >> 8 & 0xFF);
        byte b = (byte)((int)knownColor & 0xFF);
        return new Color { A = a, B = b, G = g, R = r };
    }
}