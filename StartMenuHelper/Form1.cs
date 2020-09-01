using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace StartMenuHelper
{
    public partial class Form1 : Form
    {
        [DllImport("shell32.dll")]
        static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner,
           [Out] StringBuilder lpszPath, int nFolder, bool fCreate);
        const int CSIDL_COMMON_STARTMENU = 0x16;  // All Users\Start Menu
        //private const int CP_NOCLOSE_BUTTON = 0x200;
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams myCp = base.CreateParams;
        //        myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
        //        return myCp;
        //    }
        //}
        public Form1()
        {
            InitializeComponent();
            //this.WindowState = FormWindowState.Normal;

            this.dgResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


            List<MenuItem> menuItems = new List<MenuItem>()
            {
                new MenuItem("Exit", (object sender, EventArgs e)=>{
                    this.notifyIcon1.Visible=false;
                    Environment.Exit(0);
                }),
            };
            this.notifyIcon1.ContextMenu = new ContextMenu(menuItems.ToArray());
        }
        private string AllUsersStartMenu()
        {
            StringBuilder path = new StringBuilder(260);
            SHGetSpecialFolderPath(IntPtr.Zero, path, CSIDL_COMMON_STARTMENU, false);
            string s = path.ToString();
            return s;
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.BringToFront();
            this.Activate();
            this.WindowState = FormWindowState.Normal;
        }
        /// <summary>
        /// Searches the start menu item when entering data in the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string startMenuDir = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            List<string> files = Directory.GetFiles(startMenuDir, "*.*", SearchOption.AllDirectories)
                                .Where(f => Path.GetFileName(f.ToLower()).Contains(this.txtSearch.Text.ToLower().Trim()) && !f.EndsWith(".ini")).ToList();

            List<Result> results = new List<Result>();
            foreach (string f in files)
            {
                results.Add(new Result(f));
            }
            files = Directory.GetFiles(this.AllUsersStartMenu(), "*.*", SearchOption.AllDirectories)
                                .Where(f => Path.GetFileName(f.ToLower()).Contains(this.txtSearch.Text.ToLower().Trim()) && !f.EndsWith(".ini")).ToList();
            foreach (string f in files)
            {
                results.Add(new Result(f));
            }
            this.dgResults.DataSource = results;
        }
        /// <summary>
        /// When the form is shown, focus on the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            this.txtSearch.Focus();
        }

        private void dgResults_CellDoubleClick(object sender=null, DataGridViewCellEventArgs e = null)
        {
            if (this.dgResults.Rows.Count == 0) return;
            if (this.dgResults.SelectedRows.Count == 0) return;
            string fullLocation = this.dgResults.SelectedRows[0].Cells["fullLocation"].Value.ToString();
            if (File.Exists(fullLocation))
            {
                try
                {
                    Process proc = new Process();
                    proc.StartInfo.FileName = fullLocation;
                    proc.Start();
                }
                catch (Exception)
                {

                }
                this.WindowState = FormWindowState.Minimized;
                this.txtSearch.Clear();
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.dgResults.Rows.Count > 0)
                {
                    this.dgResults_CellDoubleClick();
                }
            }
            if (e.KeyCode == Keys.Down)
            {
                if (this.dgResults.Rows.Count > 0)
                {
                    int selectedRowIndex = this.dgResults.SelectedRows[0].Index;
                    selectedRowIndex++;
                    if (selectedRowIndex < this.dgResults.Rows.Count)
                    {
                        this.dgResults.Rows[selectedRowIndex].Selected = true;
                    }
                }
            }
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }

            if (e.KeyCode == Keys.Up)
            {
                int selectedRowIndex = this.dgResults.SelectedRows[0].Index;
                selectedRowIndex--;
                if (selectedRowIndex >= 0)
                {
                    this.dgResults.Rows[selectedRowIndex].Selected = true;
                }
            }
        }
        /// <summary>
        /// Hide the form to the start menu when the application closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.txtSearch.Clear();
            this.WindowState = FormWindowState.Minimized;
            e.Cancel = true;
        }
    }
}
