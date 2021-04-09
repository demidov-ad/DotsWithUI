using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotsWithUI
{
    public partial class Form1 : Form
    {
        private Field field = new Field();
        private CellState currentPlayer = CellState.Blue;
        private const int CELL_SIZE = 20;
        private Artificial_Intelligence AI = new Artificial_Intelligence();
        private List<PointWithPriority> points = new List<PointWithPriority>();

        public Form1()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        
        /// <summary>
        /// модификатор для OnMouseClick
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            var p = new Point((int)Math.Round(1f * e.X / CELL_SIZE), (int)Math.Round(1f * e.Y / CELL_SIZE));
            if (field[p] == CellState.Empty)
            {
                field.SetPoint(p, currentPlayer);
                var pToList = new PointWithPriority(xSet: p.X, ySet: p.Y);
                points.Add(pToList);
                //currentPlayer = Field.Inverse(currentPlayer);
                Invalidate();
            }
            AI.SetPoint(field, points);
            Invalidate();
        }
        

        /// <summary>
        /// рисует)) 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.ScaleTransform(CELL_SIZE, CELL_SIZE);
            //рисуем сеточку
            using (var pen = new Pen(Color.Gainsboro, 0.1f))
            {
                for (int x = 0; x < Field.SIZE; x++)
                    e.Graphics.DrawLine(pen, x, 0, x, Field.SIZE - 1);
                for (int y = 0; y < Field.SIZE; y++)
                    e.Graphics.DrawLine(pen, 0, y, Field.SIZE - 1, y);
            }

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //рисуем занятые области
            foreach(var item in field.TakenAreas)
            {
                var state = item.Item1;
                var area = item.Item2;
                var contour = field.GetContour(area);

                //рисуем контур
                using (var pen = new Pen(Color.White, 0.1f))
                using (var brush = new SolidBrush(Color.White))
                {
                    pen.Color = StateToColor(state);
                    brush.Color = StateToColor(state, 100);
                    e.Graphics.FillPolygon(brush, contour.ToArray());
                    e.Graphics.DrawPolygon(pen, contour.ToArray());
                }
            }

            //рисуем выставленные точки
            using(var brush = new SolidBrush(Color.White))
                for (int x = 0; x < Field.SIZE; x++)
                for (int y = 0; y < Field.SIZE; y++)
                {
                    var p = new Point(x, y);
                    var cell = field[p];
                    if (cell != CellState.Empty)
                    {
                        brush.Color = StateToColor(cell);
                        e.Graphics.FillEllipse(brush, x - 0.2f, y - 0.2f, 0.4f, 0.4f);
                    }
                }
        }

        /// <summary>
        /// цвета точек
        /// </summary>
        /// <param name="state"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        Color StateToColor(CellState state, byte alpha = 255)
        {
            var res = state == CellState.Blue ? Color.Blue : Color.Red;
            return Color.FromArgb(alpha, res);
        }
    }
}