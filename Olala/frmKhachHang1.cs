using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Olala
{
    public partial class frmKhachHang1 : DevExpress.XtraEditors.XtraForm
    {
        int vitri = 0;
        Boolean themKH = false;
        String maCN;
        DateTime now = DateTime.Now;
        public frmKhachHang1()
        {
            InitializeComponent();
        }

        private void frmKhachHang1_Load(object sender, EventArgs e)
        {
            DS.EnforceConstraints = false; //không cần ktra các ràng buộc
            this.khachHangTableAdapter.Connection.ConnectionString = Program.connstr;
            this.khachHangTableAdapter.Fill(this.DS.KhachHang);

            this.taiKhoanTableAdapter.Connection.ConnectionString = Program.connstr;
            this.taiKhoanTableAdapter.Fill(this.DS.TaiKhoan);

            //đoạn code này vẫn tiềm ẩn lỗi, tự fix thầy sẽ thả vào lổi lúc chạy demo
            maCN = ((DataRowView)bdsKH[0])["MACN"].ToString().Trim();
            cmbChiNhanh.DataSource = Program.bdsDSPM;
            cmbChiNhanh.DisplayMember = "TENCN";
            cmbChiNhanh.ValueMember = "TENSERVER";
            cmbChiNhanh.SelectedIndex = Program.mChinhanh;

            if (Program.mGroup.Trim() == "NGANHANG")
            {
                //Program.bdsDSPM.Filter = "TENCN <> 'TRA CUU'";
                cmbChiNhanh.Enabled = true;

                //nhóm ngan hang chỉ đc xem tra cứu
                btnThem.Enabled = btnGhi.Enabled = btnSua.Enabled = btnXoa.Enabled = btnUndo.Enabled = false;
            }
            else
            {
                cmbChiNhanh.Enabled = false;
                btnThem.Enabled = btnGhi.Enabled = btnSua.Enabled = btnXoa.Enabled = btnUndo.Enabled = true;
            }

            txtCMND.ReadOnly = true;
            txtMACN.ReadOnly = true; //khóa mã CN lại
            panelControl2.Enabled = false; // ?????
        }

        private void khachHangBindingNavigatorSaveItem_Click_1(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsKH.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            themKH = true; //??? để khi thêm ktra trùng CMND
            vitri = bdsKH.Position;
            panelControl2.Enabled = true;
            bdsKH.AddNew();
            gcKH.Enabled = false; //cho cái lưới enable = false
            txtMACN.Text = maCN; // test đỡ 1 TH khi chưa có form đăng nhập
            cmbPhai.SelectedIndex = -1;
            txtCMND.Text = "";
            txtCMND.ReadOnly = false;
            txtCMND.Focus(); // kiểu đưa con trỏ chuột về đây đầu tiên 
            ngayCap.EditValue = now;
            //khi them thì chỉ có 2 nút hoạt động là ghi, undo
            btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnUndo.Enabled = true;
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsKH.Position;
            panelControl2.Enabled = true;
            gcKH.Enabled = false;
            // khi sửa chỉ có thể ghi hoặc undo
            btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnUndo.Enabled = true;
        }

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txtCMND.Text.Trim() == "")
            {
                MessageBox.Show("Chứng minh nhân dân không được bỏ trống ! ", "", MessageBoxButtons.OK);
                txtCMND.Focus();
                return;
            }
            if (!Program.isValidInputCMNDSTK.IsMatch(txtCMND.Text.Trim()))
            {
                MessageBox.Show("Chứng minh nhân dân không đúng định dạng !", "", MessageBoxButtons.OK);
                txtCMND.Focus();
                return;
            }
            if (txtHO.Text.Trim() == "")
            {
                MessageBox.Show("Họ khách hàng không được bỏ trống ! ", "", MessageBoxButtons.OK);
                txtHO.Focus();
                return;
            }
            if (txtTEN.Text.Trim() == "")
            {
                MessageBox.Show("Tên khách hàng không được bỏ trống ! ", "", MessageBoxButtons.OK);
                txtTEN.Focus();
                return;
            }
            if (txtSDT.Text.Trim() == "")
            {
                MessageBox.Show("Số điện thoại không được bỏ trống ! ", "", MessageBoxButtons.OK);
                txtSDT.Focus();
                return;
            }
            if (!Program.isValidInputSDT.IsMatch(txtSDT.Text.Trim()))
            {
                MessageBox.Show("Số điện thoại không đúng định dạng !", "", MessageBoxButtons.OK);
                txtSDT.Focus();
                return;
            }
            if (txtDiaChi.Text.Trim() == "")
            {
                MessageBox.Show("Địa chỉ không được bỏ trống ! ", "", MessageBoxButtons.OK);
                txtDiaChi.Focus();
                return;
            }
            if (ngayCap.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng chọn ngày cấp ! ", "", MessageBoxButtons.OK);
                ngayCap.Focus();
                return;
            }
            if (ngayCap.DateTime.CompareTo(now) == 1)
            {
                MessageBox.Show("Không được chọn ngày cấp quá ngày hiện tại ! ", "", MessageBoxButtons.OK);
                ngayCap.Focus();
                return;
            }
            if (cmbPhai.Text.Trim() == "")
            {
                MessageBox.Show("Phái không được bỏ trống! ", "", MessageBoxButtons.OK);
                cmbPhai.Focus();
                return;
            }
            //check cmnd tồn tại
            if (themKH == true && kiemtraCMND(txtCMND.Text.Trim()) == 1)
            {
                MessageBox.Show("Chứng minh dân nhân đã tồn tại ! ", "", MessageBoxButtons.OK);
                txtCMND.Focus();
                return;
            }
            try
            {
                bdsKH.EndEdit();
                bdsKH.ResetCurrentItem();
                this.khachHangTableAdapter.Connection.ConnectionString = Program.connstr; // cái này đúng
                this.khachHangTableAdapter.Update(this.DS.KhachHang);
                MessageBox.Show("Ghi khách hàng thành công!", "", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi khách hàng.\n" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
            gcKH.Enabled = true;
            panelControl2.Enabled = false;
            //đã ghi r kh thể ghi và undo
            btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = btnUndo.Enabled = false;
            themKH = false;
            //      txtCMND.ReadOnly = true; //đóng ô khóa chính, chỉ mở khi thêm mới
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            String CMND = "";
            if (bdsTK.Count > 0) // tài khoản là nhân bản nên tìm ở site nào cũng v
            {
                MessageBox.Show("Không thể xóa khách hàng này vì đã tạo tài khoản ngân hàng", "",
                    MessageBoxButtons.OK);
                return;
            }
            if (MessageBox.Show("Bạn có thật sự muốn xóa khách hàng này ?", "Xác nhận",
                MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    CMND = ((DataRowView)bdsKH[bdsKH.Position])["CMND"].ToString().Trim();
                    bdsKH.RemoveCurrent(); // xóa trên máy hiện tại trước

                    this.khachHangTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.khachHangTableAdapter.Update(this.DS.KhachHang); // xóa trên csdl*//*
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa khách hàng. Bạn hãy xóa lại\n" + ex.Message, "",
                        MessageBoxButtons.OK);
                    this.khachHangTableAdapter.Fill(this.DS.KhachHang);
                    bdsKH.Position = bdsKH.Find("CMND", CMND); //khi xóa bị lổi phải trả lại nhân viên tại vị trí đang xóa
                    return;
                }
            }
            if (bdsKH.Count == 0) btnXoa.Enabled = false; // kh có khách hàng thì kh thể xóa
        }

        private void btnUndo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            themKH = false; //khi bấm thêm sau đó chọn undo thì cho cái nút thêm trở lại trạng thái false
            bdsKH.CancelEdit();
            //thêm thì bỏ thêm, sửa thì bỏ sửa
            if (btnThem.Enabled == false) bdsKH.Position = vitri;
            gcKH.Enabled = true;
            panelControl2.Enabled = false;
            //bấm undo thì kh bấm ghi và undo đc nữa
            btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = btnUndo.Enabled = false;
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.khachHangTableAdapter.Fill(this.DS.KhachHang);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Reload: " + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
        }

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close(); //đúng kh ?
        }

        private void cmbChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbChiNhanh.SelectedValue.ToString() == "System.Data.DataRowView")
                return;
            Program.servername = cmbChiNhanh.SelectedValue.ToString();

            if (cmbChiNhanh.SelectedIndex != Program.mChinhanh)
            {
                Program.mlogin = Program.remotelogin;
                Program.password = Program.remotepassword;
            }
            else
            {
                Program.mlogin = Program.mloginDN;
                Program.password = Program.passwordDN;
            }
            if (Program.KetNoi() == 0)
            {
                MessageBox.Show("Lỗi kết nối về chi nhánh mới", "", MessageBoxButtons.OK);
            }
            else
            {
                this.khachHangTableAdapter.Connection.ConnectionString = Program.connstr;
                this.khachHangTableAdapter.Fill(this.DS.KhachHang);
                this.taiKhoanTableAdapter.Connection.ConnectionString = Program.connstr;
                this.taiKhoanTableAdapter.Fill(this.DS.TaiKhoan);
                //lệnh này thừa. Vì phân quyền chỉ có nhóm ngân hàng đc rẻ nhánh
                // và nhóm ngân hàng chỉ đc xem, tra cứu 
                // maCN = ((DataRowView)bdsKH[0])["MACN"].ToString().Trim();
            }
        }

        public static int kiemtraCMND(String check_str)
        {
            if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
            String str_sp = "SP_CHECKCMND";
            Program.Sqlcmd = Program.conn.CreateCommand();
            Program.Sqlcmd.CommandType = CommandType.StoredProcedure;
            Program.Sqlcmd.CommandText = str_sp;
            Program.Sqlcmd.Parameters.Add("@CMND", SqlDbType.NChar).Value = check_str;
            Program.Sqlcmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            Program.Sqlcmd.ExecuteNonQuery();
            String ret = Program.Sqlcmd.Parameters["@RET"].Value.ToString();
            if (ret == "1")
            {
                return 1; //đã tồn tại
            }
            return 0;
        }

        private void gcKH_Click(object sender, EventArgs e)
        {

        }

        private void txtCMND_TextChanged(object sender, EventArgs e)
        {

        }
    }
}