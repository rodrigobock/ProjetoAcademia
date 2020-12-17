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
    public partial class F_GestaoUsuarios : Form
    {
        public F_GestaoUsuarios()
        {
            InitializeComponent();
        }

        private void btn_novo_Click(object sender, EventArgs e)
        {
            F_NovoUsuario f_NovoUsuario = new F_NovoUsuario();
            f_NovoUsuario.ShowDialog();
            dgv_Usuarios.DataSource = banco.ObterUsuariosIDNome();
        }

        private void btn_fechar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void F_GestaoUsuarios_Load(object sender, EventArgs e)
        {
            dgv_Usuarios.DataSource = banco.ObterUsuariosIDNome();
            dgv_Usuarios.Columns[0].Width = 120;
            dgv_Usuarios.Columns[1].Width = 260;
        }

        private void dgv_Usuarios_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            int conLinhas = dgv.SelectedRows.Count;
            if (conLinhas > 0)
            {
                DataTable dt = new DataTable();
                string vid = dgv.SelectedRows[0].Cells[0].Value.ToString();
                dt = banco.ObterDadosUsuario(vid);
                tb_id.Text = dt.Rows[0].Field<Int64>("N_ID_USUARIOS").ToString();
                tb_nome.Text = dt.Rows[0].Field<string>("T_NOME_USUARIO").ToString();
                tb_username.Text = dt.Rows[0].Field<string>("T_USER_NAME").ToString();
                tb_senha.Text = dt.Rows[0].Field<string>("T_SENHA_USER").ToString();
                cb_status.Text = dt.Rows[0].Field<string>("T_STATUS_USER").ToString();
                n_nivel.Value = dt.Rows[0].Field<Int64>("N_NIVEL_ACESSO");
            }   
        }

        private void btn_salva_Click(object sender, EventArgs e)
        {
            int linha = dgv_Usuarios.SelectedRows[0].Index;
            Usuario u = new Usuario();
            u.id = Convert.ToInt32(tb_id.Text);
            u.usuario = tb_nome.Text;
            u.username = tb_username.Text;
            u.senha = tb_senha.Text;
            u.status = cb_status.Text;
            u.acesso = Convert.ToInt32(Math.Round(n_nivel.Value));
            banco.AtualizarUsuario(u);

            dgv_Usuarios[1, linha].Value = tb_nome.Text;
        }

        private void btn_excluir_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Confirmar exclusão?", "Excluir?", MessageBoxButtons.YesNo);
            if(res == DialogResult.Yes)
            {
                banco.DeleteUser(tb_id.Text);
                dgv_Usuarios.Rows.Remove(dgv_Usuarios.CurrentRow);
            }
        }
    }
}
