using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WCS
{
    public class PathInfo
    {
        string id;
        Geometry path_info;
        double left;
        double top;
        double right;
        double bottom;
        double width;
        double height;
        double origin_x;
        double origin_y;
        double scale_x;
        double scale_y;
        double rotate_angle;
        string fill_color;
        string border_color;
        double border_thickness;
        string stretch;
        string verticalalignment;
        string horizontalaignment;
        string text;

        public string Id { get => id; set => id = value; }
        public Geometry Path_info { get => path_info; set => path_info = value; }
        public double Left { get => left; set => left = value; }
        public double Top { get => top; set => top = value; }
        public double Right { get => right; set => right = value; }
        public double Bottom { get => bottom; set => bottom = value; }
        public double Width { get => width; set => width = value; }
        public double Height { get => height; set => height = value; }
        public double Origin_x { get => origin_x; set => origin_x = value; }
        public double Origin_y { get => origin_y; set => origin_y = value; }
        public double Scale_x { get => scale_x; set => scale_x = value; }
        public double Scale_y { get => scale_y; set => scale_y = value; }
        public double Rotate_angle { get => rotate_angle; set => rotate_angle = value; }
        public string Fill_color { get => fill_color; set => fill_color = value; }
        public string Border_color { get => border_color; set => border_color = value; }
        public double Border_thickness { get => border_thickness; set => border_thickness = value; }
        public string Stretch { get => stretch; set => stretch = value; }
        public string Verticalalignment { get => verticalalignment; set => verticalalignment = value; }
        public string Horizontalaignment { get => horizontalaignment; set => horizontalaignment = value; }
        public string Text { get => text; set => text = value; }
    }
}
