using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangesFinder
{

    class Logic
    {
        static string DateFormat = "-dd-MM-yyyy__HH-mm-ss";
        public static void GetChanges(string SourcePath, string SavePath, string AppName, int? Days, bool ShowInExplorer, bool GetAll, bool MakeRar,string Ext)
        {
            DateTime ComparisonDate = DateTime.Now;
            var CurrentTime = DateTime.Now.ToString(DateFormat);
            if (Days.HasValue && !GetAll)
                ComparisonDate = DateTime.Now.AddDays(-1 * Days.Value);
            var AllowedExt = Ext.Trim().Split(',');
            var mostRecentlyModified = Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories)
                          .Select(f => new FileInfo(f)).Where(x => AllowedExt.Contains(Path.GetExtension(x.FullName).ToLower()));
            if (!GetAll)
                mostRecentlyModified = mostRecentlyModified.Where(x => x.LastWriteTime >= ComparisonDate);

            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
            string currentPath = SavePath;

            SavePath = Path.Combine(SavePath, AppName + CurrentTime);
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
            foreach (var file in mostRecentlyModified)
            {
                MoveFile(file.FullName, SavePath, SourcePath);
            }
            if (MakeRar)
            {
                CreateRar(SavePath + ".rar", SavePath,currentPath);
                string argument = "/select, \"" + SavePath + ".rar" + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
            else if (ShowInExplorer)
            {
                string argument = "/select, \"" + SavePath + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
        }
        static void MoveFile(string file, string NewLocation, string SourcePath)
        {
            var NewPath = file.Replace(SourcePath, NewLocation);

            string LastDir = "";
            for (int i = 0; i < NewPath.Split('\\').Length - 1; i++)
            {
                string dir = NewPath.Split('\\')[i];
                //Add Exception For Obj Folder
                if (dir.ToLower() == "obj")
                    return;
                if (!File.Exists(Path.Combine(SourcePath, LastDir, dir)))
                {
                    string newDirPath = Path.Combine(LastDir, dir);
                    if (!Directory.Exists(newDirPath))
                    {
                        Directory.CreateDirectory(newDirPath);
                    }
                    if (dir.EndsWith(":"))
                        dir += "\\";
                    LastDir = Path.Combine(LastDir, dir);
                }
            }

            NewPath = Path.Combine(file.Replace(SourcePath, NewLocation));
            if (File.Exists(NewPath))
            {
                File.Delete(NewPath);
            }
            File.Copy(file, NewPath);
        }
        static void CreateRar(string rarName, string FolderPath, string WorkingDir)
        {
            string rarPath = @"C:\Program Files (x86)\WinRAR\";

            if (!Directory.Exists(rarPath))
                rarPath = @"C:\Program Files\WinRAR\";//Try for 64 bit version
            if (Directory.Exists(rarPath))
            {
                string WorkerPath = Path.Combine(WorkingDir, "Rar.cmd");
                System.Diagnostics.ProcessStartInfo sdp = new System.Diagnostics.ProcessStartInfo();
                string cmdArgs = string.Format("cd \"{0}\"\nset path=\"{1}\"; \nrar a -r \"{2}\"", FolderPath, rarPath, rarName);
                File.WriteAllText(WorkerPath, cmdArgs);
                sdp.ErrorDialog = true;
                sdp.UseShellExecute = true;
                sdp.FileName = WorkerPath;
                sdp.CreateNoWindow = false;
                System.Diagnostics.Process process = System.Diagnostics.Process.Start(sdp);
                process.WaitForExit();
                File.Delete(WorkerPath);
            }
            else
            {
                System.Windows.MessageBox.Show("Please Install Winrar To Be Able To Use Add To Rar Feature");
            }
        }
    }
}
