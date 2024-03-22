using AllTPlayer_App.Models;
using AllTPlayer_App.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using MediaManager;
using System.IO;
using AllTPlayer_App.Data;
using System;
using Xamarin.Essentials;
using static Android.Net.Http.SslCertificate;

namespace AllTPlayer_App.ViewModels
{
    public class MainViewModel : BaseViewModel
    {

        public MainViewModel()
        {
            GetLocalMusics();
            recentMusic = musicList.Where(x => x.IsRecent == true).FirstOrDefault();

            //App.UsersDB.DeleteUsersAsync();

            var currentUser = App.UsersDB.GetUsersAsync().Result;

            

            if (currentUser.Count == 0)
            {
                Shell.Current.Navigation.PushAsync(new VkLoginPage(), true);
            }
            else
            {
                Shell.Current.Navigation.PushAsync(new VkMusicPage(), true);
            }
            
        }

        private ObservableCollection<Music> musicList = new ObservableCollection<Music>();
        public ObservableCollection<Music> MusicList
        {
            get { return musicList; }
            set
            {
                musicList = value;
                OnPropertyChanged();
            }
        }

        private Music recentMusic;
        public Music RecentMusic
        {
            get { return recentMusic; }
            set
            {
                recentMusic = value;
                OnPropertyChanged();
            }
        }

        private Music selectedMusic;
        public Music SelectedMusic
        {
            get { return selectedMusic; }
            set
            {
                selectedMusic = value;
                OnPropertyChanged();
            }

        }

        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                OnPropertyChanged();
            }
        }

        public bool isPlaying;
        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                isPlaying = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PlayIcon));
            }
        }

        private bool changedMusic;
        public bool ChangedMusic
        {
            get { return changedMusic; }
            set
            {
                changedMusic = value;
                OnPropertyChanged();
            }
        }

        private async void PlayMusic(Music music)
        {
            var mediaInfo = CrossMediaManager.Current;
            await mediaInfo.Play(music?.Url);
            IsPlaying = true;

            mediaInfo.MediaItemFinished += (sender, args) =>
            {
                IsPlaying = false;
                ChangedMusic = true;
                NextMusic();
            };
        }

        public string PlayIcon { get => isPlaying ? "pause.png" : "playBlack.png"; }

        public ICommand PlayCommand => new Command(Play);
        public ICommand ChangeCommand => new Command(ChangeMusic);
        public ICommand SelectionCommand => new Command(SelectionChanged);
        public ICommand SearchCommand => new Command(SearchMusic);

        private void SearchMusic()
        {
            GetLocalMusics();
        }

        private async void SelectionChanged()
        {
            IsVisible = true;
            if (changedMusic != true)
            {
                if (isPlaying == true)
                {
                    await Task.Run(() => { CrossMediaManager.Current.Pause(); });
                    PlayMusic(selectedMusic);
                    IsPlaying = true;
                }
                if (isPlaying != true)
                {
                    PlayMusic(selectedMusic);
                    IsPlaying = true;
                }
            }
        }
        private async void Play()
        {
            if (isPlaying)
            {
                await CrossMediaManager.Current.Pause();
                IsPlaying = false;
            }
            else
            {
                await CrossMediaManager.Current.Play();
                IsPlaying = true;
            }
        }

        private void ChangeMusic(object obj)
        {
            ChangedMusic = true;
            if ((string)obj == "P")
            {
                PreviousMusic();
            }
            else if ((string)obj == "N")
            {
                NextMusic();
            }
        }

        private void NextMusic()
        {
            var currentIndex = musicList.IndexOf(selectedMusic);

            if (currentIndex < musicList.Count - 1)
            {
                SelectedMusic = musicList[currentIndex + 1];
                PlayMusic(selectedMusic);
                ChangedMusic = false;
            }

        }

        private void PreviousMusic()
        {
            var currentIndex = musicList.IndexOf(selectedMusic);

            if (currentIndex > 0)
            {
                SelectedMusic = musicList[currentIndex - 1];
                PlayMusic(selectedMusic);
                ChangedMusic = false;
            }
        }

        private async void GetLocalMusics()
        {
            var permissions = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

            if (permissions != PermissionStatus.Granted)
            {
                permissions = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            if (permissions != PermissionStatus.Granted)
            {
                return;
            }

            string musicFolderPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).AbsolutePath;
            string downloadFolderPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
            string dcimFolderPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).AbsolutePath;

            string[] musicFolders = { musicFolderPath, downloadFolderPath, dcimFolderPath };
            string fileName = "*.mp3";

            

            for (int i = 0; i < musicFolders.Length; i++)
            {

                
                var folder = musicFolders[i];

                string[] files = Directory.GetFiles(folder);

                foreach (string findedFile in Directory.EnumerateFiles(folder, fileName, SearchOption.AllDirectories))
                {
                    FileInfo fileInfo;
                    var findedMusic = await CrossMediaManager.Current.Extractor.CreateMediaItem(findedFile);
                    if (findedMusic.Title == null)
                    {

                    }
                    try
                    {
                        fileInfo = new FileInfo(findedFile);
                        MusicList.Add(new Music
                        {
                            Title = findedMusic.Title,
                            Artist = findedMusic.Artist,
                            CoverImage = findedMusic.AlbumImageUri,
                            Url = fileInfo.FullName,

                        });

                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }
    }
}
