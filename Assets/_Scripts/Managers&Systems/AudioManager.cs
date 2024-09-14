using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets._Scripts.Managers_Systems
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AudioManager>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject("AudioManager");
                        instance = obj.AddComponent<AudioManager>();
                    }
                }
                return instance;
            }
        }

        private ObjectPool<AudioSource> audioSourcePool;
        private Transform audioSourceParent;
        private Dictionary<AudioSource, Coroutine> audioSourceCoroutines;

        private void Awake()
        {
            audioSourceParent = new GameObject("AudioSources").transform;
            audioSourcePool = new ObjectPool<AudioSource>(() =>
            {
                GameObject obj = new GameObject("AudioSource");
                obj.transform.SetParent(audioSourceParent);
                AudioSource audioSource = obj.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1f;
                audioSource.maxDistance = 5f;
                //audioSource.outputAudioMixerGroup = GameManager.Instance.audioMixer.outputAudioMixerGroup;
                return audioSource;
            });
            audioSourceCoroutines = new Dictionary<AudioSource, Coroutine>();
        }

        public AudioSource PlayAudioClip(AudioClip clip, Vector3 position, float volume, bool loop = false)
        {
            if (clip == null)
            {
                Debug.LogWarning("Audio clip is null");
                return null;
            }

            AudioSource audioSource = audioSourcePool.Get();
            audioSource.transform.position = position;
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.loop = loop;
            audioSource.Play();

            if (loop)
            {
                Coroutine coroutine = StartCoroutine(LoopAudioSource(audioSource));
                audioSourceCoroutines.Add(audioSource, coroutine);
            }
            else
            {
                StartCoroutine(ReturnAudioSource(audioSource));
            }

            return audioSource;
        }

        public void StopAudioClip(AudioSource audioSource)
        {
            audioSource.Stop();
            audioSource.loop = false;

            if (audioSourceCoroutines.ContainsKey(audioSource))
            {
                Coroutine coroutine = audioSourceCoroutines[audioSource];
                StopCoroutine(coroutine);
                audioSourceCoroutines.Remove(audioSource);
            }

            audioSourcePool.Return(audioSource);
        }

        private IEnumerator LoopAudioSource(AudioSource audioSource)
        {
            while (audioSource.isPlaying == true)
            {
                if (!audioSource.loop)
                {
                    StopAudioClip(audioSource);
                    yield break; // Exit the coroutine
                }
                yield return null;
            }
        }

        public void StopAllAudio()
        {
            foreach (Transform child in audioSourceParent)
            {
                AudioSource audioSource = child.GetComponent<AudioSource>();
                audioSource.Stop();
                audioSourcePool.Return(audioSource);

                if (audioSourceCoroutines.ContainsKey(audioSource))
                {
                    Coroutine coroutine = audioSourceCoroutines[audioSource];
                    StopCoroutine(coroutine);
                    audioSourceCoroutines.Remove(audioSource);
                }
            }
        }
        private IEnumerator ReturnAudioSource(AudioSource audioSource)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
            StopAudioClip(audioSource);
        }
    }

    public class ObjectPool<T> where T : Component
    {
        private Stack<T> stack = new Stack<T>();
        private Func<T> createFunc;

        public ObjectPool(Func<T> createFunc)
        {
            this.createFunc = createFunc;
        }

        public T Get()
        {
            if (stack.Count == 0)
            {
                return createFunc();
            }
            return stack.Pop();
        }

        public void Return(T obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("Returning null object to pool");
                return;
            }

            if (stack.Contains(obj))
            {
                Debug.LogWarning("Object already in pool");
                return;
            }

            stack.Push(obj);
        }
    }
}