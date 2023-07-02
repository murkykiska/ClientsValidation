using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ClientsValidation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ClientsCollection? Clients;
        public static ClientsCollection? ValidatedClients = new ClientsCollection();
        public static RegistratorsCollection? Registrators = new RegistratorsCollection();
        // 0 - не заполнено ФИО
        // 1 - не заполнен Рег. номер
        // 2 - не заполнен Регистратор
        public static List<Tuple<int, string>> Errors = new List<Tuple<int, string>>();
        // общее количесво ошибок
        public static int AllErrors;
        public void ReadClients(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ClientsCollection));

            using (StreamReader sr = new StreamReader(path))
            {
                try
                {
                    Clients = serializer.Deserialize(sr) as ClientsCollection;
                }
                catch
                {
                    throw new Exception("Wrong input file!");
                }

            }
        }
        public void AssemblyRegistrators()
        {
            foreach (Client c in Clients.Clients)
            {
                var r = new Registrator { Name = c.Registrator };

                if (!Registrators.Registrators.Contains(r) && c.Registrator != "")
                {
                    r.ID = Registrators.Registrators.Count;
                    Registrators.Registrators.Add(r);
                }

                c.SetRegistratorID(Registrators.Registrators.IndexOf(r));
            }
        }
        public void ValidateClients()
        {
            Errors.Add(new(0, "FIO"));
            Errors.Add(new(1, "DiasoftID"));
            Errors.Add(new(2, "Registrator"));

            foreach (var c in Clients.Clients)
            {
                var context = new ValidationContext(c);
                var results = new List<ValidationResult>();
                if (!Validator.TryValidateObject(c, context, results, true))
                {
                    foreach (var error in results)
                    {
                        switch (error.MemberNames.ToArray()[0])
                        {
                            case "FIO":
                                Errors[0] = new Tuple<int, string>(Errors[0].Item1 + 1, Errors[0].Item2);
                                break;
                            case "DiasoftID":
                                Errors[1] = new Tuple<int, string>(Errors[1].Item1 + 1, Errors[1].Item2);
                                break;
                            case "Registrator":
                                Errors[2] = new Tuple<int, string>(Errors[2].Item1 + 1, Errors[2].Item2);
                                break;
                        }
                    }
                    AllErrors++;
                }
                else
                {
                    ValidatedClients.Clients.Add(c);
                }
            }
        }
        public void PrintClients(string path)
        {
            path += "//ValidatedClients.xml";

            XmlSerializer serializer = new XmlSerializer(typeof(ClientsCollection));

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                serializer.Serialize(fs, ValidatedClients);
            }
        }
        public void PrintRegistrators(string path)
        {
            path += "//Registrators.xml";

            XmlSerializer serializer = new XmlSerializer(typeof(RegistratorsCollection));

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                serializer.Serialize(fs, Registrators);
            }
        }
        public void PrintErrors()
        {
            ErrorsTextBox.Clear();
            
            var errors = Errors.OrderByDescending(x => x.Item1).ToList();

            for (int i = 0; i < errors.Count; i++)
            {
                string s = "Не указан";

                switch (errors[i].Item2)
                {
                    case "FIO":
                        s += "о ФИО";
                        break;
                    case "DiasoftID":
                        s += " DiasoftID";
                        break;
                    case "Registrator":
                        s += " Регистратор";
                        break;
                }

                s += $": {errors[i].Item1} записей\n";

                ErrorsTextBox.AppendText(s);
            }
            ErrorsTextBox.AppendText($"Всего ошибочных записей: {AllErrors}");
        }
        public static void PrintErrorsInFile(string path)
        {
            path += "//Errors.txt";

            var errors = Errors.OrderByDescending(x => x.Item1).ToList();

            using (StreamWriter sw = new StreamWriter(path))
            {
                for (int i = 0; i < errors.Count; i++)
                {
                    string s = "Не указан";

                    switch (errors[i].Item2)
                    {
                        case "FIO":
                            s += "о ФИО";
                            break;
                        case "DiasoftID":
                            s += " DiasoftID";
                            break;
                        case "Registrator":
                            s += " Регистратор";
                            break;
                    }

                    s += $": {errors[i].Item1} записей";

                    sw.WriteLine(s);
                }
                sw.WriteLine($"Всего ошибочных записей: {AllErrors}");
            }
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            string path;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;

                // Десериализация клиентов из файла
                ReadClients(path);
            }
            // Генерация идентификаторов регистраторов и их списка
            AssemblyRegistrators();
            // Валидация данных клиентов
            ValidateClients();
            // Вывод описания ошибок на TextBox
            PrintErrors();
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            string path;

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            CommonFileDialogResult result = dialog.ShowDialog();

            path = dialog.FileName;

            // Вывод xml-файла со списком клиентов, записи которых не содержат ошибки
            PrintClients(path);
            // Вывод xml-файла со списком регистраторов
            PrintRegistrators(path);
            // Вывод текстового файла с описанием ошибок
            PrintErrorsInFile(path);
        }
    }
}
