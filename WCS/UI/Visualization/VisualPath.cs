using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ui
{
    public class VisualPath : IVisualObject<Path>
    {
        private Path _visualObj;
        public Path VisualObj { get => _visualObj; }

        public VisualPath(Path path)
        {
            _visualObj = path;
            _visualObj.MouseLeftButtonDown += _visualObj_MouseLeftButtonDown;
        }

        public void _visualObj_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void AddObj(object fatherDock)
        {
            ((Grid)fatherDock).Children.Add(_visualObj);
        }

        public void OnSetBackColor<T>(T brush) where T :Brush
        {
            throw new NotImplementedException();
        }
    }
}
