using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

public static class Explorer
{
    public static DriveInfo[] GetDrives()
    {
        return DriveInfo.GetDrives();
    }

    public static void ShowDrives(DriveInfo[] drives, int selectedIndex)
    {
        Console.WriteLine("Выберите диск:");

        for (int i = 0; i < drives.Length; i++)
        {
            if (i == selectedIndex)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            Console.WriteLine($"{i + 1}. {drives[i].Name} - {drives[i].DriveType} {drives[i].TotalSize / (1024L * 1024 * 1024)} GB всего, {drives[i].AvailableFreeSpace / (1024L * 1024 * 1024)} GB свободно");

            Console.ResetColor();
        }
    }

    public static DriveInfo GetSelectedDrive(DriveInfo[] drives)
    {
        int selectedIndex = 0;

        while (true)
        {
            Console.Clear();
            ShowDrives(drives, selectedIndex);

            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex == 0) ? drives.Length - 1 : selectedIndex - 1;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex == drives.Length - 1) ? 0 : selectedIndex + 1;
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                return drives[selectedIndex];
            }
        }
    }

    public static string GetSelectedDirectoryOrFile(string path)
    {
        int selectedIndex = 0;
        string[] contentList = Directory.GetFileSystemEntries(path)
            .Select(entry => $"{(Directory.Exists(entry) ? "[D]" : "[F]")} {Path.GetRelativePath(path, entry)}")
            .ToArray();

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Содержимое {path}:");

            for (int i = 0; i < contentList.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(contentList[i]);

                Console.ResetColor();
            }

            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex == 0) ? contentList.Length - 1 : selectedIndex - 1;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex == contentList.Length - 1) ? 0 : selectedIndex + 1;
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                return contentList[selectedIndex].Substring(4);
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                return "..";
            }
        }
    }

    public static void ShowFolderContents(string path)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Содержимое {path}:");

            string[] contentList = Directory.GetFileSystemEntries(path)
                .Select(entry => $"{(Directory.Exists(entry) ? "[D]" : "[F]")} {Path.GetRelativePath(path, entry)}")
                .ToArray();

            for (int i = 0; i < contentList.Length; i++)
            {
                Console.WriteLine(contentList[i]);
            }

            Console.WriteLine("Нажмите Escape, чтобы вернуться в основное меню.");

            string selectedDirectoryOrFile = GetSelectedDirectoryOrFile(path);

            if (selectedDirectoryOrFile == "..")
            {
                if (IsDriveRoot(path))
                {
                    return;
                }
                else
                {
                    path = Path.GetDirectoryName(path);
                }
            }
            else
            {
                string newPath = Path.Combine(path, selectedDirectoryOrFile);
                if (Directory.Exists(newPath))
                {
                    path = newPath;
                }
                else if (File.Exists(newPath))
                {
                    ExecuteFile(newPath);
                }
            }
        }
    }

    private static bool IsDriveRoot(string path)
    {
        DriveInfo drive = DriveInfo.GetDrives().FirstOrDefault(d => d.RootDirectory.FullName == path);
        return drive != null && drive.RootDirectory.FullName == path;
    }

    public static void ShowDriveInfo(DriveInfo drive)
    {
        Console.WriteLine($"Диск {drive.Name} - {drive.TotalSize / (1024L * 1024 * 1024)} GB всего, {drive.AvailableFreeSpace / (1024L * 1024 * 1024)} GB свободно");
    }

    public static void ShowFolderInfo(string folderPath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
        Console.WriteLine($"Папка {directoryInfo.FullName}");
        Console.WriteLine($"Создана: {directoryInfo.CreationTime}");
    }

    public static void ExecuteFile(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();

        switch (extension)
        {
            case ".txt":
                Process.Start("notepad.exe", filePath);
                break;
            case ".docx":
                Process.Start("C:\\Program Files\\Microsoft Office\\root\\Office16\\WINWORD.EXE", filePath);
                break;
            case ".pdf":
                Process.Start("AcroRd32.exe", filePath);
                break;
            default:
                Console.WriteLine($"Не удалось определить, как открыть файл {filePath}");
                break;
        }
    }

}

public static class ArrowHelper
{
    public static int ShowArrowMenu(string[] options, int selectedIndex)
    {
        while (true)
        {
            Console.Clear();
            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(options[i]);

                Console.ResetColor();
            }

            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex == options.Length - 1) ? 0 : selectedIndex + 1;
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                return selectedIndex;
            }
        }
    }
}

class Program
{
    static void Main()
    {
        string[] menuOptions = { "Показать диски", "Выход" };
        int selectedIndex = 0;

        while (true)
        {
            int menuResult = ArrowHelper.ShowArrowMenu(menuOptions, selectedIndex);

            if (menuResult == 0)
            {
                Console.Clear();
                DriveInfo[] drives = Explorer.GetDrives();
                DriveInfo selectedDrive = Explorer.GetSelectedDrive(drives);

                if (selectedDrive != null)
                {
                    Explorer.ShowDriveInfo(selectedDrive);
                    Explorer.ShowFolderContents(selectedDrive.RootDirectory.FullName);
                }
            }
            else if (menuResult == 1)
            {
                break;
            }
        }
    }
}
