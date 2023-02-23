using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Path = System.IO.Path;

namespace Task1
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<DriveInfo> Drives { get; } = new();
        public ObservableCollection<FileInfo> Files { get; } = new();
        private const long Gigabyte = 1073741824;
        private string _directoryName = Path.Combine(@"C:\Users\noh4u\Desktop");

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Task.Run(() =>
            {
                Parallel.ForEach(System.IO.DriveInfo.GetDrives(), driveInfo =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (!driveInfo.IsReady) return;
                        Drives.Add(new DriveInfo(driveInfo.Name, driveInfo.DriveType, driveInfo.TotalSize / Gigabyte,
                            Math.Round((double)(driveInfo.TotalSize - driveInfo.AvailableFreeSpace) / driveInfo.TotalSize * 100), driveInfo.TotalFreeSpace / Gigabyte));
                    });
                });
            });
        }

        private void DirectoryNameTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            _directoryName = DirectoryNameTextBox.Text;
            if (!Path.Exists(_directoryName)) MessageBox.Show("Данной директории не существует");
            else GetTabContent();
        }

        private void GetTabContent()
        {
            switch (((TabItem)TabControl.SelectedItem).Header)
            {
                case "Общая информация":
                    GeneralInformation(new DirectoryInfo(_directoryName));
                    break;
                case "Топ 10 файлов":
                    PopularFiles(new DirectoryInfo(_directoryName));
                    break;
                case "Объём файлов":
                    break;
            }
        }

        private void GeneralInformation(DirectoryInfo directoryInfo)
        {
            try
            {
                var drive = new System.IO.DriveInfo(Path.GetPathRoot(directoryInfo.FullName));

                var filesCount = directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories).Count();
                var directoriesCount = directoryInfo.EnumerateDirectories("*", SearchOption.AllDirectories).Count();
                var size = directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories)
                    .Sum(fileInfo => fileInfo.Length);
                var occupancyPercent = Convert.ToDouble(size) / drive.TotalSize * 100.0;

                FilesCountTextBlock.Text = $"Количество файлов: {filesCount}";
                DirectoriesCountTextBlock.Text = $"Количество папок: {directoriesCount}";
                DirectorySizeTextBlock.Text = $"Размер папки: {size / 1024} КБ";
                OccupiedPlaceTextBlock.Text = $"Занятое место: {Math.Round(occupancyPercent, 2)}";
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show($"Access to the path '{directoryInfo.FullName}' is denied. Skipping subtree...");
            }
        }

        private void PopularFiles(DirectoryInfo directoryInfo)
        {
            try
            {
                Files.Clear();

                Task.Run(() =>
                {
                    Parallel.ForEach(
                        directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories)
                            .OrderByDescending(item => item.Length).Take(10),
                        fileInfo =>
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Files.Add(new FileInfo(fileInfo.Name, fileInfo.Length, fileInfo.FullName, fileInfo.LastWriteTime));
                            });
                        });
                });
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show($"Access to the path '{directoryInfo.FullName}' is denied. Skipping subtree...");
            }
        }

        private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetTabContent();
        }
    }
}