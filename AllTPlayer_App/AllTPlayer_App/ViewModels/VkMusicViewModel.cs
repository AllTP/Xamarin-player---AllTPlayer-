using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Extensions.DependencyInjection;
using AllTPlayer_App.ViewModels;
using System.Windows.Input;
using System.Collections.ObjectModel;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model;
using VkNet.Model.RequestParams;
using MediaManager;
using AllTPlayer_App.Models;
using Xamarin.Essentials;
using Newtonsoft.Json;
using System;

namespace AllTPlayer_App.ViewModels
{
    public class VkMusicViewModel : BaseViewModel
    {
        public VkMusicViewModel()
        {
            var currentUser = App.UsersDB.GetUsersAsync().Result;

            if (currentUser.Count != 0)
            {
                GetVkAudio(currentUser[0].Login, currentUser[0].Password);
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
        private async void ToLoginPage()
        {
            await Shell.Current.Navigation.PushAsync(new VkLoginPage(),true);
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

        private bool isVisible=true;
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

        public ICommand LoginCommand => new Command(ToLoginPage);

        public VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> GetVkAudio(string login, string password)
        {
            var api = new VkApi(new ServiceCollection().AddAudioBypass());
            try
            {
                api.Authorize(new ApiAuthParams
                {
                    ApplicationId = 51608982,
                    Login = login,
                    Password = password,
                    Settings = VkNet.Enums.Filters.Settings.All,
                });
            }
            catch (System.Exception)
            {
                throw;
            }
            
            var vkAudios = api.Audio.Get(new AudioGetParams { Count = 100 });
            VkMusicToList(vkAudios);
            return vkAudios;
        }

        private void VkMusicToList(VkNet.Utils.VkCollection<VkNet.Model.Attachments.Audio> vkAudios)
        {
            foreach (var music in vkAudios)
            {
                try
                {
                    var findedMusic = music;
                    MusicList.Add(new Music
                    {
                        Title = findedMusic.Title,
                        Artist = findedMusic.Artist,
                        CoverImage = findedMusic.Album.Thumb.Photo300,
                        Url = findedMusic.Url.AbsoluteUri,
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