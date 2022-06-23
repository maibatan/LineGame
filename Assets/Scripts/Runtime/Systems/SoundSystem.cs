using LineGame.Runtime.Utilities;
using UnityEngine;

namespace LineGame.Runtime.Systems
{

    public class SoundSystem : StaticInstance<SoundSystem>
    {
        [SerializeField]
        protected AudioSource _audioSource;

        [SerializeField]
        protected AudioClip _moveSound;
        [SerializeField]
        protected AudioClip _explodeSound;
        [SerializeField]
        protected AudioClip _gameOverSound;

        protected void PlaySound(AudioClip audio)
        {
            _audioSource.PlayOneShot(audio);
        }
        public void PlayMoveSound()
        {
            PlaySound(_moveSound);
        }
        public void PlayExplodeSound()
        {
            PlaySound(_explodeSound);
        }
        public void PlayGameOverSound()
        {
            PlaySound(_gameOverSound);
        }
    }
}
