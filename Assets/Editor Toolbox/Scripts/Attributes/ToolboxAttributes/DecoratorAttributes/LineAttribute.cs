﻿using System;

namespace UnityEngine
{
    /// <summary>
    /// Creates thin, horizontal line.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class LineAttribute : ToolboxDecoratorAttribute
    {
        public LineAttribute(float thickness = 0.75f, float padding = 6.0f)
        {
            Thickness = Math.Max(thickness, 0);
            Padding = padding;
        }

        public float Thickness { get; private set; }

        public float Padding { get; private set; }

        /// <summary>
        /// Indicates if drawer should apply additional indent to the line's width.
        /// </summary>
        public bool ApplyIndent { get; set; }

        /// <summary>
        /// Allows to override the color of the horizontal line.
        /// </summary>
        public string HexColor { get; set; }

        /// <summary>
        /// Returns the expected color of the horizontal line.
        /// </summary>
        public Color GuiColor
        {
            get
            {
                if (ColorUtility.TryParseHtmlString(HexColor, out var color))
                {
                    return color;
                }
                else
                {
                    return new Color(0.3f, 0.3f, 0.3f);
                }
            }
        }
    }
}