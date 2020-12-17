using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academia
{
    public partial class F_Horarios : Form
    {
        public F_Horarios()
        {
            InitializeComponent();
        }

        private void F_Horarios_Load(object sender, EventArgs e)
        {
            string vquery = @"
                SELECT N_IDHORARIO as 'ID', T_DCSHORARIO as 'Horário' from tb_horarios ORDER BY T_DCSHORARIO
            ";
            dgv_horario.DataSource = banco.dql(vquery);

            dgv_horario.Columns[0].Width = 160;
            dgv_horario.Columns[1].Width = 290;
        }

        private void dgv_horario_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            int conLinhas = dgv.SelectedRows.Count;
            if (conLinhas > 0)
            {
                DataTable dt = new DataTable();
                string vid = dgv.SelectedRows[0].Cells[0].Value.ToString();
                string vquery = @"SELECT * FROM tb_horarios WHERE N_IDHORARIO = " + vid;

                dt = banco.dql(vquery);
                tb_idHorario.Text = dt.Rows[0].Field<Int64>("N_IDHORARIO").ToString();
                tb_horario.Text = dt.Rows[0].Field<string>("T_DCSHORARIO").ToString();
            }
        }

        private void btn_novo_Click(object sender, EventArgs e)
        {
            tb_idHorario.Clear();
            tb_horario.Clear();
            tb_horario.Focus();
        }

        private void btn_salvar_Click(object sender, EventArgs e)
        {
            string vquery = "";

            if (tb_idHorario.Text == "")
            {
                vquery = "INSERT INTO tb_horarios (T_DCSHORARIO) VALUES ('" + tb_horario.Text + "')";
            }
            else
            {
                vquery = "UPDATE tb_horarios SET T_DCSHORARIO = '" + tb_horario.Text + "' WHERE N_IDHORARIO = " + tb_idHorario.Text;
            }

            banco.dml(vquery);
            vquery = @"
                SELECT N_IDHORARIO as 'ID', T_DCSHORARIO as 'Horário' from tb_horarios ORDER BY T_DCSHORARIO
            ";
            dgv_horario.DataSource = banco.dql(vquery);

        }

        private void btn_fechar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_excluir_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Confirmar exclusão?", "Excluir", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                string vquery = "DELETE FROM tb_horarios WHERE N_IDHORARIO = " + tb_idHorario.Text;
                banco.dml(vquery);
                dgv_horario.Rows.Remove(dgv_horario.CurrentRow);
            }
        }
    }
}
