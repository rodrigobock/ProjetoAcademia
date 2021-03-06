﻿using System;
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
    public partial class F_Login : Form
    {
        Form1 form1;
        DataTable dt = new DataTable();

        public F_Login(Form1 f)
        {
            InitializeComponent();
            form1 = f;
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            string username = tb_usuario.Text;
            string senha = tb_senha.Text;

            if (username == "" || senha == "")
            {
                MessageBox.Show("Usuario ou senha invalidos");
                tb_usuario.Clear();
                tb_senha.Clear();
                tb_usuario.Focus();
                return;
            }

            string sql = "SELECT * FROM tb_usuarios WHERE T_USER_NAME = '" + username + "' AND T_SENHA_USER = '" + senha + "'";
            dt = banco.dql(sql);

            if(dt.Rows.Count == 1)
            {
                form1.lb_acesso.Text = dt.Rows[0].ItemArray[5].ToString();
                form1.lb_nomeUsuario.Text = dt.Rows[0].Field<string>("T_NOME_USUARIO");
                form1.pb_ledLogin.Image = Properties.Resources.led_verde;
                Globais.nivel = int.Parse(dt.Rows[0].Field<Int64>("N_NIVEL_ACESSO").ToString());
                Globais.logado = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Usuario não encontrado!");
                tb_usuario.Clear();
                tb_senha.Clear();
                tb_usuario.Focus();
                return;
            }


        }

        private void btn_cancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
