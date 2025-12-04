using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//Новый текст

namespace Test1ISP
{
    public partial class Form1 : Form
    {
        int a;
        int quection_count;
        int correct_answers;
        int wrong_answers;
        string Ocenka;
        int correct_answers_number;
        int selected_response;

        System.IO.StreamReader Read;

        public Form1()
        {
            InitializeComponent();
        }

        public struct QuestionInfo
        {
            public string QuestionText;
            public string Answer1;
            public string Answer2;
            public string Answer3;
            public int CorrectAnswer;
            public int QuestionIndex;
        }

        List<QuestionInfo> ihuchuumiret = new List<QuestionInfo>();

        Dictionary<int, bool> answersStatus = new Dictionary<int, bool>();

        void start()
        {
            var Encoding = System.Text.Encoding.GetEncoding(65001);
            try
            {
                Read = new System.IO.StreamReader(System.IO.Directory.GetCurrentDirectory() + @"\t.txt", Encoding);

                this.Text = Read.ReadLine();

                a = 0;
                quection_count = 0;
                correct_answers = 0;
                wrong_answers = 0;

                answersStatus.Clear();
                ihuchuumiret.Clear();

                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;

                string line;
                while ((line = Read.ReadLine()) != null)
                {
                    QuestionInfo question = new QuestionInfo();
                    question.QuestionText = line;
                    question.Answer1 = Read.ReadLine();
                    question.Answer2 = Read.ReadLine();
                    question.Answer3 = Read.ReadLine();
                    question.CorrectAnswer = int.Parse(Read.ReadLine());
                    ihuchuumiret.Add(question);
                    a++;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Файл не найден");
                this.Close();
            }
            quection();
        }

        string ochenka()
        {
            double percentage = (double)correct_answers / quection_count * 100;

            if (percentage >= 91)
            {
                Ocenka = "5";
            }
            else if (percentage >= 71)
            {
                Ocenka = "4";
            }
            else if (percentage >= 50)
            {
                Ocenka = "3";
            }
            else
            {
                Ocenka = "2";
            }
            return Ocenka;
        }

        void predquestion()
        {
            if (ihuchuumiret.Count > 0)
            {
                QuestionInfo lastQuestion = ihuchuumiret[ihuchuumiret.Count - 1];

                label1.Text = lastQuestion.QuestionText;
                radioButton1.Text = lastQuestion.Answer1;
                radioButton2.Text = lastQuestion.Answer2;
                radioButton3.Text = lastQuestion.Answer3;
                correct_answers_number = lastQuestion.CorrectAnswer;
                quection_count = lastQuestion.QuestionIndex;

                if (answersStatus.ContainsKey(quection_count - 1) && answersStatus[quection_count - 1])
                {
                    correct_answers--;
                }

                if (button1.Text.Equals("Завершить"))
                {
                    button1.Text = "Следующий вопрос";
                }

                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;

                button1.Enabled = false;
            }
        }

        void quection()
        {
            if (quection_count < ihuchuumiret.Count)
            {
                QuestionInfo question = ihuchuumiret[quection_count];
                label1.Text = question.QuestionText;
                radioButton1.Text = question.Answer1;
                radioButton2.Text = question.Answer2;
                radioButton3.Text = question.Answer3;
                correct_answers_number = question.CorrectAnswer;

                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;

                button1.Enabled = false;
                quection_count++;

                if (quection_count == a)
                    button1.Text = "Завершить";
            }
        }

        void perecl(object sender, EventArgs e)
        {
            button1.Enabled = true; button1.Focus();
            RadioButton perc = (RadioButton)sender;
            var tmp = perc.Name;

            selected_response = int.Parse(tmp.Substring(11));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.AutoSize = false;
            this.AutoSize = true;
            label1.Size = new Size(1115, 623);

            if (ihuchuumiret.Count <= 0)
            {
                button2.Visible = false;
            }
            button1.Text = "Следующий вопрос";
            button2.Text = "Предыдуший вопрос";

            radioButton1.CheckedChanged += new EventHandler(perecl);
            radioButton2.CheckedChanged += new EventHandler(perecl);
            radioButton3.CheckedChanged += new EventHandler(perecl);

            start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            QuestionInfo questionInfo = new QuestionInfo();
            questionInfo.QuestionText = label1.Text;
            questionInfo.Answer1 = radioButton1.Text;
            questionInfo.Answer2 = radioButton2.Text;
            questionInfo.Answer3 = radioButton3.Text;
            questionInfo.CorrectAnswer = correct_answers_number;
            questionInfo.QuestionIndex = quection_count;
            ihuchuumiret.Add(questionInfo);

            if (ihuchuumiret.Count > 0)
            {
                button2.Visible = true;
            }

            if (selected_response == correct_answers_number) correct_answers++;
            if (selected_response != correct_answers_number)
            {
                wrong_answers++;
            }

            bool isCorrect = (selected_response == correct_answers_number);
            answersStatus[quection_count - 1] = isCorrect;

            if (button1.Text == "Начать тестирование сначала")
            {
                button2.Visible = false;

                answersStatus.Clear();
                ihuchuumiret.Clear();

                button1.Text = "Следующий вопрос";

                radioButton1.Visible = true;
                radioButton2.Visible = true;
                radioButton3.Visible = true;

                start(); return;
            }
            if (button1.Text.Equals("Завершить"))
            {
                button2.Text = "Закрыть тест";
                Read.Close();

                radioButton1.Visible = false;
                radioButton2.Visible = false;
                radioButton3.Visible = false;

                string Ocenka = ochenka();

                label1.Text = $"Тестирование завершено.\n" +
                $"Правильных ответов: {correct_answers} из {quection_count}.\n" +
                $"Оценка: {Ocenka}";

                button1.Text = $"Начать тестирование сначала";

                string resultsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "results.txt");

                using (StreamWriter writer = new StreamWriter(resultsFilePath, false, Encoding.UTF8))
                {
                    writer.WriteLine($"Результаты для попытки теста {DateTime.Now}");
                    writer.WriteLine($"Правильных ответов: {correct_answers}, Неправильных ответов: {wrong_answers}");
                    writer.WriteLine($"Ваша оценка: {Ocenka}");
                    writer.WriteLine();
                }

                return;
            }

            quection();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Начать тестирование сначала"))
            {
                this.Close();
            }
            else
            {
                if (ihuchuumiret.Count > 0 && quection_count > 0)
                {
                    quection_count--;
                    predquestion();
                    ihuchuumiret.RemoveAt(ihuchuumiret.Count - 1);

                    if (quection_count == 1)
                    {
                        button2.Visible = false;
                    }
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

