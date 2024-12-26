using WinFormsApp1.Helpers;

namespace TransformJsonToXml
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string rootPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\.."));
            string xmlTemplate = Path.Combine(rootPath, "Xml", "Template.xml");
            string jsonData = Path.Combine(rootPath, "Json", "Data.json");

            var result = SOAPHelper.TransformJsonToXml(xmlTemplate, jsonData);

            textBox1.Text = result;
        }
    }
}
