
using System.Drawing;
using System.Windows.Forms;

namespace YMH219_FlappyBird
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;

            ClientSize = new Size(800, 550);

            Name = "Form1";
            Text = "Flappy Bird - Night Pixel Edition";

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            MaximizeBox = false;
            MinimizeBox = true;

            ResumeLayout(false);
        }
    }
}
