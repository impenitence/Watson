using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ui
{
    interface IVisualObject<T>
    {
        /// <summary>
        /// 可视化对象 path or label
        /// </summary>
        T VisualObj { get; }
        /// <summary>
        /// 将可视化对象加入父容器
        /// </summary>
        /// <param name="fatherDock"></param>
        void AddObj(object fatherDock);
        /// <summary>
        /// 刷新可视化显示
        /// </summary>
        void OnSetBackColor<B>(B brush) where B : Brush;
        /// <summary>
        /// 双击事件
        /// </summary>
        void _visualObj_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e);
    }
}
