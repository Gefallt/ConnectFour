using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ConnectFourLogic.Helpers
{
    public class ColorHelper
    {
        private static readonly string[] Colors =
        {
            "#E53935", // Red
            "#1E88E5", // Blue
            "#FB8C00", // Orange
            "#43A047", // Green
            "#8E24AA", // Purple
            "#00ACC1"  // Cyan
        };

        public static string GetRandomColor()
        {
            var random = new Random();
            return Colors[random.Next(Colors.Length)];
        }


        public static string GetComplementaryColor(string hexColor)
        {
            Color color = ColorTranslator.FromHtml(hexColor);

            RgbToHsv(
                color.R,
                color.G,
                color.B,
                out double h,
                out double s,
                out double v);

            // 色相を180°回転
            h = (h + 180.0) % 360.0;

            Color result = HsvToRgb(h, s, v);

            return $"#{result.R:X2}{result.G:X2}{result.B:X2}";
        }

        private static void RgbToHsv(
            int r,
            int g,
            int b,
            out double h,
            out double s,
            out double v)
        {
            double rd = r / 255.0;
            double gd = g / 255.0;
            double bd = b / 255.0;

            double max = Math.Max(rd, Math.Max(gd, bd));
            double min = Math.Min(rd, Math.Min(gd, bd));
            double delta = max - min;

            h = 0;

            if (delta != 0)
            {
                if (max == rd)
                    h = 60 * (((gd - bd) / delta) % 6);
                else if (max == gd)
                    h = 60 * (((bd - rd) / delta) + 2);
                else
                    h = 60 * (((rd - gd) / delta) + 4);
            }

            if (h < 0)
                h += 360;

            s = max == 0 ? 0 : delta / max;
            v = max;
        }

        private static Color HsvToRgb(
            double h,
            double s,
            double v)
        {
            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
            double m = v - c;

            double r = 0;
            double g = 0;
            double b = 0;

            if (h < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (h < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (h < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (h < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (h < 300)
            {
                r = x; g = 0; b = c;
            }
            else
            {
                r = c; g = 0; b = x;
            }

            return Color.FromArgb(
                (int)((r + m) * 255),
                (int)((g + m) * 255),
                (int)((b + m) * 255));
        }

    }
}
