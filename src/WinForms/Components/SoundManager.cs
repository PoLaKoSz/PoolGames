using CSharpSnookerCore.Models;
using IrrKlang;
using System.Collections.Generic;

namespace CSharpSnooker.WinForms.Components
{
    class SoundManager
    {
        private readonly ISoundEngine _soundEngine;
        private List<Sound> _tracklist;



        public SoundManager()
        {
            _soundEngine = new ISoundEngine();
            _tracklist = new List<Sound>();
        }



        public void Add()
        {
            _tracklist.Add(null);
        }

        public void Add(int index, Sound music)
        {
            _tracklist[index] = music;
        }

        public void Play(int index)
        {
            Sound currentSound = _tracklist[index];

            if (currentSound != null)
            {
                Play(currentSound);
            }
        }

        public void StopAll()
        {
            _soundEngine.StopAllSounds();
        }

        /// <summary>
        /// Clears out every old and remaining sound.
        /// </summary>
        public void Empty()
        {
            for (int i = 0; i < _tracklist.Count; i++)
            {
                _tracklist[i] = null;
            }
        }


        private void Play(Sound music)
        {
            float posZ = (float)(music.Position.X - 300.0d) / 300.0f;

            _soundEngine.Play3D(music.Path, -1, 0, posZ);
        }
    }
}
