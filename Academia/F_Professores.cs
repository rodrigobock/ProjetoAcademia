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
    public partial class F_Professores : Form
    {
        public F_Professores()
        {
            InitializeComponent();
        }

        private void F_Professores_Load(object sender, EventArgs e)
        {
            string vquery = @"
                SELECT N_IDPROFESSOR as 'ID', T_NOMEPROFESSOR as 'Professor', T_TELEFONE as 'Telefone' from tb_professores ORDER BY N_IDPROFESSOR
            ";
            dgv_professores.DataSource = banco.dql(vquery);

            dgv_professores.Columns[0].Width = 100;
            dgv_professores.Columns[1].Width = 200;
            dgv_professores.Columns[2].Width = 200;
        }

        private void dgv_professores_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            int conLinhas = dgv.SelectedRows.Count;
            if (conLinhas > 0)
            {
                DataTable dt = new DataTable();
                string vid = dgv.SelectedRows[0].Cells[0].Value.ToString();
                string vquery = @"SELECT * FROM tb_professores WHERE N_IDPROFESSOR = " + vid;

                dt = banco.dql(vquery);
                tb_idProfessor.Text = dt.Rows[0].Field<Int64>("N_IDPROFESSOR").ToString();
                tb_nomeProfessor.Text = dt.Rows[0].Field<string>("T_NOMEPROFESSOR");
                tb_telefone.Text = dt.Rows[0].Field<string>("T_TELEFONE");
            }
        }

        private void btn_novo_Click(object sender, EventArgs e)
        {
            tb_idProfessor.Clear();
            tb_nomeProfessor.Clear();
            tb_telefone.Clear();
            tb_nomeProfessor.Focus();
        }

        private void btn_salvar_Click(object sender, EventArgs e)
        {
            string vquery = "";

            if (tb_idProfessor.Text == "")
            {
                vquery = "INSERT INTO tb_professores (T_NOMEPROFESSOR, T_TELEFONE) VALUES ('" + tb_nomeProfessor.Text + "','" + tb_telefone.Text + "')";
            }
            else
            {
                vquery = "UPDATE tb_professores SET T_NOMEPROFESSOR = '" + tb_nomeProfessor.Text + "', T_TELEFONE = '"+tb_telefone+ "'  WHERE N_IDPROFESSOR = " + tb_idProfessor.Text;
            }

            banco.dml(vquery);
            vquery = @"
                SELECT N_IDPROFESSOR as 'ID', T_NOMEPROFESSOR as 'Professor', T_TELEFONE as 'Telefone'  from tb_professores ORDER BY N_IDPROFESSOR
            ";
            dgv_professores.DataSource = banco.dql(vquery);
        }

        private void btn_excluir_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Confirmar exclusão?", "Excluir", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                string vquery = "DELETE FROM tb_professores WHERE N_IDPROFESSOR = " + tb_idProfessor.Text;
                banco.dml(vquery);
                dgv_professores.Rows.Remove(dgv_professores.CurrentRow);
            }
        }

        private void btn_fechar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
