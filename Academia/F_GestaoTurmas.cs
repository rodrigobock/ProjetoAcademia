using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Academia
{
    public partial class F_GestaoTurmas : Form
    {
        string idSelecionado;
        int modo = 0; // 0 = padrao, 1 = edição, 2= insercao
        string vquery = "";
        public F_GestaoTurmas()
        {
            InitializeComponent();
        }

        private void F_GestaoTurmas_Load(object sender, EventArgs e)
        {


            vquery = @"
            SELECT 
                tbt.N_IDTURMA as 'ID',
                tbt.T_DSCTURMA as 'TURMA',
                tbh.T_DCSHORARIO as 'HORARIO'
            FROM
                tb_turmas as tbt
            INNER JOIN
                tb_horarios as tbh on tbh.N_IDHORARIO = tbt.N_IDHORARIOS
            ";

            dgv_turmas.DataSource = banco.dql(vquery);

            dgv_turmas.Columns[0].Width = 100;
            dgv_turmas.Columns[1].Width = 150;
            dgv_turmas.Columns[2].Width = 170;

            // popular combobox professores

            string vqueryProfessores = @"SELECT N_IDPROFESSOR, T_NOMEPROFESSOR FROM tb_professores ORDER BY N_IDPROFESSOR";
            cb_NomeProfessor.Items.Clear();
            cb_NomeProfessor.DataSource = banco.dql(vqueryProfessores);
            cb_NomeProfessor.DisplayMember = "T_NOMEPROFESSOR";
            cb_NomeProfessor.ValueMember = "N_IDPROFESSOR";

            // Popular combobox status 

            Dictionary<string, string> st = new Dictionary<string, string>();
            st.Add("A", "Ativa");
            st.Add("P", "Paralisada");
            st.Add("C", "Cancelada");
            cb_status.Items.Clear();
            cb_status.DataSource = new BindingSource(st, null);
            cb_status.DisplayMember = "Value";
            cb_status.ValueMember = "Key";

            // Popular ComboBox Horários

            string vqueryHorarios = @"SELECT * FROM tb_horarios ORDER BY T_DCSHORARIO";
            cb_horario.Items.Clear();
            cb_horario.DataSource = banco.dql(vqueryHorarios);
            cb_horario.DisplayMember = "T_DCSHORARIO";
            cb_horario.ValueMember = "N_IDHORARIO";


        }

        private void dgv_turmas_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            int contlinhas = dgv.SelectedRows.Count;
            if (contlinhas > 0)
            {
                modo = 0;
                idSelecionado = dgv_turmas.Rows[dgv_turmas.SelectedRows[0].Index].Cells[0].Value.ToString();

                string vqueryCampos = @"SELECT T_DSCTURMA, N_IDPROFESSOR, N_IDHORARIOS, N_MAXALUNOS, T_STATUS FROM tb_turmas
                WHERE N_IDTURMA = " + idSelecionado;

                DataTable dt = banco.dql(vqueryCampos);
                tb_dscturma.Text = dt.Rows[0].Field<string>("T_DSCTURMA").ToString();
                cb_NomeProfessor.SelectedValue = dt.Rows[0].Field<Int64>("N_IDPROFESSOR").ToString();
                tb_MaxAlunos.Value = dt.Rows[0].Field<Int64>("N_MAXALUNOS");
                cb_status.SelectedValue = dt.Rows[0].Field<string>("T_STATUS").ToString();
                cb_horario.SelectedValue = dt.Rows[0].Field<Int64>("N_IDHORARIOS");

                // Calculo de vagas

                tb_vagas.Text = calcVagas();
            }
        }

        private string calcVagas()
        {
            string queryVagas = String.Format(@"SELECT N_IDALUNO as 'ContVagas' FROM tb_alunos WHERE T_STATUS = 'A' and N_IDTURMA = {0}", idSelecionado);

            DataTable dt = banco.dql(queryVagas);
            int vagas = int.Parse(Math.Round(tb_MaxAlunos.Value, 0).ToString());
            vagas -= Int32.Parse(dt.Rows[0].Field<Int64>("ContVagas").ToString());

            return vagas.ToString();
        }

        private void btn_novo_Click(object sender, EventArgs e)
        {
            tb_dscturma.Clear();
            cb_NomeProfessor.SelectedIndex = -1;
            tb_MaxAlunos.Value = 0;
            cb_status.SelectedIndex = -1;
            cb_horario.SelectedIndex = -1;
            tb_dscturma.Focus();
            modo = 2;
        }

        private void btn_salvar_Click(object sender, EventArgs e)
        {
            if (modo != 0)
            {
                string queryTurma = "";
                string msg = "";
                if (modo == 1)
                {
                    msg = "Dados alterados";
                    queryTurma = String.Format(@"
                UPDATE 
                    tb_turmas
                SET
                    T_DSCTURMA='{0}',
                    N_IDPROFESSOR = '{1}',
                    N_IDHORARIOS = '{2}',
                    N_MAXALUNOS = '{3}',
                    T_STATUS = '{4}',
                WHERE
                    N_IDTURMA = '{5}'", tb_dscturma.Text, cb_NomeProfessor.SelectedValue, cb_horario.SelectedValue,
                    Int32.Parse(Math.Round(tb_MaxAlunos.Value).ToString()), cb_status.SelectedValue, idSelecionado
                );
                }
                else
                {
                    msg = "Nova turma inserida";
                    queryTurma = String.Format(@"INSERT INTO tb_turmas
                        (T_DSCTURMA,N_IDPROFESSOR,N_IDHORARIOS,N_MAXALUNOS,T_STATUS)
                        VALUES ('{0}',{1},{2},{3},'{4}')", tb_dscturma.Text, cb_NomeProfessor.SelectedValue, cb_horario.SelectedValue, Int32.Parse(Math.Round(tb_MaxAlunos.Value).ToString()), cb_status.SelectedValue);
                }

                int linha = dgv_turmas.SelectedRows[0].Index;
                banco.dml(queryTurma);
                if (modo == 1)
                {
                    dgv_turmas[1, linha].Value = tb_dscturma.Text;
                    dgv_turmas[2, linha].Value = cb_horario.Text;
                    tb_vagas.Text = calcVagas();
                }
                else
                {
                    dgv_turmas.DataSource = banco.dql(vquery);
                }
                MessageBox.Show(msg);

            }
        }
        private void btn_excluir_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Confirmar exclusão?", "Excluir?", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                string QueryExcluirTurma = String.Format(@"
                DELETE
                FROM
                    tb_turmas
                WHERE
                    N_IDTURMA = {0}", idSelecionado
                );
                banco.dml(QueryExcluirTurma);
                dgv_turmas.Rows.Remove(dgv_turmas.CurrentRow);
            }
        }

        private void btn_imprimir_Click(object sender, EventArgs e)
        {
            string nomeArquivo = Globais.caminho + @"\turmas.pdf";
            FileStream arquivoPDF = new FileStream(nomeArquivo, FileMode.Create);
            Document doc = new Document(PageSize.A4);
            PdfWriter escritorPDF = PdfWriter.GetInstance(doc, arquivoPDF);

            string dados = "";

            Paragraph paragrafo = new Paragraph(dados, new iTextSharp.text.Font(iTextSharp.text.Font.NORMAL, 14, (int)System.Drawing.FontStyle.Bold));
            paragrafo.Alignment = Element.ALIGN_CENTER;
            paragrafo.Add("Relatório de turmas\n\n");

            PdfPTable tabela = new PdfPTable(3);
            tabela.DefaultCell.FixedHeight = 25;

            tabela.AddCell("ID Turma");
            tabela.AddCell("Turma");
            tabela.AddCell("Horário");

            DataTable dtTurmas = banco.dql(vquery);
            for (int i = 0; i < dtTurmas.Rows.Count; i++)
            {
                tabela.AddCell(dtTurmas.Rows[i].Field<Int64>("ID").ToString());
                tabela.AddCell(dtTurmas.Rows[i].Field<string>("TURMA"));
                tabela.AddCell(dtTurmas.Rows[i].Field<string>("HORARIO"));
            }

            doc.Open();
            doc.Add(paragrafo);
            doc.Add(tabela);
            doc.Close();

            DialogResult res = MessageBox.Show("Deseja abrir o relatório?", "Relatório", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(Globais.caminho + @"\turmas.pdf");
            }
        }

        private void btn_fechar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tb_dscturma_TextChanged(object sender, EventArgs e)
        {
            if (modo == 0)
            {
                modo = 1;
            }

        }

        private void cb_NomeProfessor_TabIndexChanged(object sender, EventArgs e)
        {
            if (modo == 0)
            {
                modo = 1;
            }
        }

        private void tb_MaxAlunos_ValueChanged(object sender, EventArgs e)
        {
            if (modo == 0)
            {
                modo = 1;
            }
        }

        private void cb_status_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (modo == 0)
            {
                modo = 1;
            }
        }

        private void cb_horario_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (modo == 0)
            {
                modo = 1;
            }
        }
    }
}
