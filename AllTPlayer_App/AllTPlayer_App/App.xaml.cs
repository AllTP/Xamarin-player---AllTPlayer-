using MediaManager;
using AllTPlayer_App.Data;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace AllTPlayer_App
{
    public partial class App : Application
    {
        static UsersDB usersDB;

        public static UsersDB UsersDB
        {
            get
            {
                if (usersDB == null)
                {
                    usersDB = new UsersDB(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "UsersDatabase.db3"));
                }
                return usersDB;
            }
        }

        public App()
        {
            InitializeComponent();

            CrossMediaManager.Current.Init();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
