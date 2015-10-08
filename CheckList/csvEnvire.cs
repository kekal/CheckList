using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
namespace CheckList
{
    static class CSVEnvire
    {
        public static void ReadCSV()
        {
            ObservableCollection<Item> db = null;
            var body = "";
            var path = MainWindow.Wm.PathTextBox.Text;
            if (!Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
            {
                throw new FileNotFoundException("Source path / URL is invalid.");
            }
            Uri uriResult;
            if (Uri.TryCreate(path, UriKind.Absolute, out uriResult))
            {
                TaskManager.Add(
                    delegate
                    {
                        var webRequest = WebRequest.Create(path);
                        using (var response = webRequest.GetResponse())
                        using (var content = response.GetResponseStream())
                            if (content != null)
                            {
                                using (var reader = new StreamReader(content))
                                {
                                    try
                                    {
                                        body = reader.ReadToEnd();
                                    }
                                    catch (Exception e)
                                    {
                                        TaskManager.Add(null, () => MessageBox.Show(e.Message));
                                        throw new FileNotFoundException();
                                    }
                                }
                            }
                        TaskManager.Add(delegate
                        {
                            db = ParceFile(body);
                        });
                        TaskManager.Add(null,
                            delegate
                            {
                                MainWindow.Wm.Custdata = db;
                                MainWindow.Wm.MainDataGrid.ItemsSource = MainWindow.Wm.Custdata;
                            });
                    });
            }
            else
            {
                TaskManager.Add(
                    delegate
                    {
                        try
                        {
                            body = File.ReadAllText(path);
                        }
                        catch (Exception e)
                        {
                            TaskManager.Add(null, () => MessageBox.Show(e.Message));
                            throw new FileNotFoundException();
                        }
                        TaskManager.Add(delegate
                        {
                            db = ParceFile(body);
                        });
                        TaskManager.Add(null,
                            delegate
                            {
                                MainWindow.Wm.Custdata = db;
                                MainWindow.Wm.MainDataGrid.ItemsSource = MainWindow.Wm.Custdata;
                            });
                    });
            }
        }
        private static ObservableCollection<Item> ParceFile(string body)
        {
            var db = new ObservableCollection<Item>();
            var lines = body.Split('\n');

            // ID, description, State, IsCompleted
            foreach (var lineElements in lines.Select(line => line.Split('	').Select(e => e.Trim()).ToArray()))
            {
                ulong tempId;
                if (!ulong.TryParse(lineElements[0], out tempId)) { continue; }
                if (lineElements[1].Length < 1) { continue; } // description is not empty
                Status tempState;
                if (!Enum.TryParse(lineElements[2], out tempState)) { continue; }
                if (lineElements[3].Trim().Length > 0) // if completness is described...
                {
                    bool tempCompletness;
                    if (!bool.TryParse(lineElements[3].Trim(), out tempCompletness)) { continue; } // ...trying to parse it
                    db.Add(new Item
                    {
                        Id = tempId,
                        Description = lineElements[1],
                        State = tempState,
                        IsCompleted = tempCompletness
                    });
                }
                else
                {
                    db.Add(new Item
                    {
                        Id = tempId,
                        Description = lineElements[1],
                        State = tempState
                    });
                }
            }
            return db;
        }
    }
}
