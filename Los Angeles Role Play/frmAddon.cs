using System.Drawing;
using System.Windows.Forms;

namespace LARPLauncher {
    public partial class frmAddon : Form {
        #region  < Initialize > 
        public frmAddon() {
            InitializeComponent();
        }
        #endregion

        #region  < 폼 이동 > 
        Point MouseLocation;

        private void HeadCaption_MouseDown(object sender, MouseEventArgs e) {
            // HeadCaption 클릭 시 마우스 위치 기억
            MouseLocation = e.Location;
        }

        private void HeadCaption_MouseMove(object sender, MouseEventArgs e) {
            // 마우스 왼쪽 버튼이 눌려 있다면
            if (e.Button == MouseButtons.Left) {
                // 폼 위치 동기화
                int x = this.Location.X + e.Location.X - MouseLocation.X;
                int y = this.Location.Y + e.Location.Y - MouseLocation.Y;
                this.Location = new Point(x, y);
            }
        }
        #endregion
    }
}
