﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YetAnotherRelogger.Helpers.Bot;
using YetAnotherRelogger.Helpers.Enums;
using YetAnotherRelogger.Helpers.Tools;

namespace YetAnotherRelogger.Forms.Wizard
{
    public partial class ProfileSchedule : UserControl
    {
        private readonly WizardMain WM;

        public ProfileSchedule(WizardMain parent)
        {
            WM = parent;
            InitializeComponent();

            var col = new DataGridViewComboBoxColumn
            {
                Name = "Monster Power",
                DataSource = Enum.GetValues(typeof (MonsterPower)),
                ValueType = typeof (MonsterPower),
            };
            dataGridView1.Columns.Add(col);

            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
        }

        public BindingList<Profile> Profiles { get; set; }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dataGridView1.Columns[e.ColumnIndex].Name.Equals("Monster Power"))
            {
                Profiles[e.RowIndex].MonsterPowerLevel =
                    (MonsterPower) dataGridView1.Rows[e.RowIndex].Cells["Monster Power"].Value;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].IsNewRow)
            {
                var ofd = new OpenFileDialog {Filter = "Profile file|*.xml", Title = "Browse to profile"};
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    //dataGridView1.Rows.Add(Path.GetFileName(ofd.FileName), ofd.FileName, 0, 0);
                    var p = new Profile
                    {
                        Location = ofd.FileName,
                        Name = Path.GetFileName(ofd.FileName),
                        Runs = 0,
                        Minutes = 0,
                        MonsterPowerLevel = MonsterPower.Disabled
                    };
                    dataGridView1.DataSource = null;
                    Profiles.Add(p);
                    dataGridView1.DataSource = Profiles;
                    dataGridView1.Columns["isDone"].Visible = false;
                    UpdateGridview();
                }
            }
        }

        private void ProfileSchedule_Load(object sender, EventArgs e)
        {
            VisibleChanged += ProfileSchedule_VisibleChanged;
            textBox1.KeyPress += NumericCheck;
            textBox2.KeyPress += NumericCheck;
            comboBox1.SelectedItem = "** Global **";
        }

        private void NumericCheck(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.NumericOnly(e.KeyChar);
        }

        private void ProfileSchedule_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                WM.NextStep("Profile Settings");
                UpdateGridview();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Delete record
            if (dataGridView1.CurrentRow.IsNewRow)
            {
                return;
            }
            if (
                MessageBox.Show("Are you sure you want to delete this profile from your schedule?",
                    "Delete profile from schedule", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.Yes)
            {
                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
            }
        }

        private void dataGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hitTestInfo = dataGridView1.HitTest(e.X, e.Y);
            if (!dataGridView1.CurrentRow.IsNewRow && hitTestInfo.Type == DataGridViewHitTestType.Cell)
            {
                if (e.Button == MouseButtons.Right)
                    contextMenuStrip1.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }

        public bool ValidateInput()
        {
            return true;
        }

        public void UpdateGridview()
        {
            if (Profiles == null)
                Profiles = new BindingList<Profile>();

            dataGridView1.DataSource = Profiles;
            dataGridView1.Refresh();
            dataGridView1.ReadOnly = false;
            dataGridView1.Columns["isDone"].Visible = false;
            dataGridView1.Columns["MonsterPowerLevel"].Visible = false;

            // MonsterPowerLevel
            for (int i = 0; i < Profiles.Count; i++)
            {
                MonsterPower pl = Profiles[i].MonsterPowerLevel;
                dataGridView1.Rows[i].Cells["Monster Power"].Value = pl;
            }
        }
    }
}