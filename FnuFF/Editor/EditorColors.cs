using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Editor
{
    public static class EditorColors
    {
		// colors
        public static Color BACKGROUND_HIGH = Color.FromArgb( 45, 45, 48 );
        public static Color BACKGROUND_LOW = Color.FromArgb( 28, 28, 28 );
        public static Color BACKGROUND_LOW_BLUEPRINT = Color.FromArgb( 43, 113, 183 );
        public static Color SELECTED = Color.FromArgb( 174, 45, 48 );
        public static Color HOVERED = Color.FromArgb( 208, 208, 216 );
        public static Color PRESSED = Color.FromArgb( 71, 71, 76 );
		public static Color PRESSED_DARK = Color.FromArgb( 21, 21, 21 );
		public static Color SELECTED_HOVERED = Color.FromArgb( 255, 66, 72 );
        public static Color SELECTED_PRESSED = Color.FromArgb( 96, 25, 27 );
        public static Color GRID = Color.FromArgb( 34, 34, 34 );
		public static Color GRID_HIGHLIGHT = Color.FromArgb( 60, 60, 60 );
		public static Color GRID_BLUEPRINT = Color.FromArgb( 95, 137, 197 );

		// brushes
		public static Brush BRUSH_WHITE = Brushes.White;
		public static Brush BRUSH_HANDLE = Brushes.White;

		// pens
		public static Pen PEN_WHITE = new Pen( Color.White );
		public static Pen PEN_BLACK = new Pen( Color.Black );
		public static Pen PEN_RED = new Pen( Color.Red );
		public static Pen PEN_BLUE = new Pen( Color.Blue );
		public static Pen PEN_GREEN = new Pen( Color.Green );
		public static Pen PEN_YELLOW = new Pen( Color.Yellow );

		public static Pen PEN_FADED_RED = new Pen( Color.FromArgb( 128, Color.Red ) );
		public static Pen PEN_FADED_GREEN = new Pen( Color.FromArgb( 128, Color.Green ) );
		public static Pen PEN_FADED_BLUE = new Pen( Color.FromArgb( 128, Color.Blue ) );

		public static Pen PEN_DASH_RED = new Pen( Color.Red ) { DashPattern = new[] { 2.0f, 2.0f } };
		public static Pen PEN_DASH_GREEN = new Pen( Color.Green ) { DashPattern = new[] { 2.0f, 2.0f } };
		public static Pen PEN_DASH_BLUE = new Pen( Color.Blue ) { DashPattern = new[] { 2.0f, 2.0f } };

		public static Pen PEN_DASH_FADED_RED = new Pen( Color.FromArgb( 128, Color.Red ) ) { DashPattern = new[] { 2.0f, 2.0f } };
		public static Pen PEN_DASH_FADED_GREEN = new Pen( Color.FromArgb( 128, Color.Green ) ) { DashPattern = new[] { 2.0f, 2.0f } };
		public static Pen PEN_DASH_FADED_BLUE = new Pen( Color.FromArgb( 128, Color.Blue ) ) { DashPattern = new[] { 2.0f, 2.0f } };

		public static Pen PEN_HANDLE_OUTLINE = Pens.Black;
		public static Pen PEN_DASH_FADED_HANDLE_OUTLINE = new Pen( Color.FromArgb( 128, Color.DarkGray ) ) { DashPattern = new[] { 2.0f, 2.0f } };

		public static Pen PEN_DASH_FADED_CLIP_LINE = new Pen( Color.FromArgb( 128, Color.DarkGray ) ) { DashPattern = new[] { 4.0f, 4.0f } };

		// fonts
		public static Font SOLID_DIMENSIONS_FONT = new Font( FontFamily.GenericMonospace, 8.0f );

		// values
		public static int FADE = 64;
		public static float[] DASH_PATTERN = { 4.0f, 4.0f };
	}
}
