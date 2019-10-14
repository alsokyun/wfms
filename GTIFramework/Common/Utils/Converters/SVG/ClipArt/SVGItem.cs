using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ClipArtViewer
{
    public class SVGItem
    {
        DrawingGroup m_image = null;

        public SVG m_svg = null;

        public string FullPath { get; private set; }
        public string Filename { get; private set; }
        public SVGRender SVGRender { get; private set; }
        public DrawingGroup SVGImage
        {
            get
            {
                EnsureLoaded();
                return m_image;
            }
        }
        public SVGItem(string fullpath)
        {
            FullPath = fullpath;
            Filename = System.IO.Path.GetFileNameWithoutExtension(fullpath);
        }
        public void Reload()
        {
            //m_image = SVGRender.LoadDrawing(FullPath);
            m_image = SVGRender.LoadDrawing(m_svg);
        }
        void EnsureLoaded()
        {
            if (m_image != null)
                return;
            //Console.WriteLine("{0} - loading {1}", DateTime.Now.ToLongDateString(), Filename);
            SVGRender = new SVGRender();
            m_image = SVGRender.LoadDrawing(FullPath);
        }

        public void RenderToImage()
        {
            m_image = null;
            SVGRender = new SVGRender();
            m_image = SVGRender.LoadDrawing(m_svg);
        }
    }
}
