using CSharpSnookerCore.Models;
using System.Drawing;
using System.Windows.Forms;

namespace CSharpSnooker.WinForms.Components
{
    class UiDebugger
    {
        private readonly PocketManager _pocketManager;
        private readonly BorderManager _borderManager;
        private readonly PictureBox _debugLayer;



        public UiDebugger(MainForm mainForm, PocketManager pocketManager, BorderManager borderManager)
        {
            _pocketManager = pocketManager;
            _borderManager = borderManager;

            _debugLayer = new PictureBox
            {
                Size = mainForm.picTable.Size,
                Location = new Point(0, 0),
                Image = new Bitmap(mainForm.picTable.Width, mainForm.picTable.Height),
                BackColor = Color.Transparent
            };

            _debugLayer.MouseUp   += mainForm.PoolTable_MouseUp;
            _debugLayer.MouseMove += mainForm.PoolTable_MouseMove;

            mainForm.picTable.Controls.Add(_debugLayer);
        }



        /// <summary>
        /// Shows where are the <see cref="Pocket"/>s.
        /// </summary>
        public void AddPockets()
        {
            using (Graphics layer = Graphics.FromImage(_debugLayer.Image))
            {
                foreach (Pocket pocket in _pocketManager.Pockets)
                {
                    layer.DrawEllipse(new Pen(Brushes.White),
                        pocket.X - (int)Ball.Radius,
                        pocket.Y - (int)Ball.Radius,
                        (int)Ball.Radius * 2,
                        (int)Ball.Radius * 2);
                }
            }

            _debugLayer.Refresh();
        }

        /// <summary>
        /// Shows where are the borders.
        /// </summary>
        public void AddBorders()
        {
            using (Graphics layer = Graphics.FromImage(_debugLayer.Image))
            {
                foreach (DiagonalBorder diagonalBorder in _borderManager.DiagonalBorders)
                {
                    layer.DrawLine(
                        new Pen(Brushes.White),
                        diagonalBorder.X1,
                        diagonalBorder.Y1,
                        diagonalBorder.X2,
                        diagonalBorder.Y2);
                }

                foreach (TableBorder tableBorder in _borderManager.TableBorders)
                {
                    layer.DrawRectangle(
                        new Pen(Brushes.White),
                        tableBorder.X,
                        tableBorder.Y,
                        tableBorder.Width,
                        tableBorder.Height);
                }
            }

            _debugLayer.Refresh();
        }
    }
}
