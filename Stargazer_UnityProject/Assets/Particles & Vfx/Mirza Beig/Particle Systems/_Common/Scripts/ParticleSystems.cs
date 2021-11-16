
// =================================	
// Namespaces.
// =================================

using UnityEngine;

// =================================	
// Define namespace.
// =================================

namespace MirzaBeig
{

    namespace ParticleSystems
    {

        // =================================	
        // Classes.
        // =================================
        
        public class ParticleSystems : MonoBehaviour
        {
            // =================================	
            // Nested classes and structures.
            // =================================



            // =================================	
            // Variables.
            // =================================

            public ParticleSystem[] particleSystems { get; set; }

            // Event delegates.

            //public delegate void onParticleSystemsDeadEventHandler();
            //public event onParticleSystemsDeadEventHandler onParticleSystemsDeadEvent;

            // =================================	
            // Functions.
            // =================================

            // ...

            protected virtual void Awake()
            {
                particleSystems = GetComponentsInChildren<ParticleSystem>();
            }

            // ...

            protected virtual void Start()
            {

            }

            // ...

            protected virtual void Update()
            {

            }

            // ...

            protected virtual void LateUpdate()
            {
                //if (onParticleSystemsDeadEvent != null)
                //{
                //    if (!IsAlive())
                //    {
                //        onParticleSystemsDeadEvent();
                //    }
                //}
            }

            // ...

            public void Reset()
            {
                //simulate(0.0f, true);

                for (int i = 0; i < particleSystems.Length; i++)
                {
                    particleSystems[i].time = 0.0f;
                }
            }

            // ...

            public void Play()
            {
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    particleSystems[i].Play(false);
                }
            }

            // ...

            public void Pause()
            {
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    particleSystems[i].Pause(false);
                }
            }

            // ...

            public void Stop()
            {
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    particleSystems[i].Stop(false);
                }
            }

            // ...

            public void Clear()
            {
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    particleSystems[i].Clear(false);
                }
            }

            // ...

            public void SetLoop(bool loop)
            {
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    ParticleSystem.MainModule m = particleSystems[i].main;
                    m.loop = loop;
                }
            }

            // ...

            public void SetPlaybackSpeed(float speed)
            {
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    ParticleSystem.MainModule m = particleSystems[i].main;
                    m.simulationSpeed = speed;
                }
            }

            // ...

            public void Simulate(float time, bool reset = false)
            {
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    particleSystems[i].Simulate(time, false, reset);
                }
            }

            // ...

            public bool IsAlive()
            {
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    if (particleSystems[i])
                    {
                        if (particleSystems[i].IsAlive())
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            // ...

            public bool IsPlaying(bool checkAll = false)
            {
                if (particleSystems.Length == 0)
                {
                    return false;
                }
                else if (!checkAll)
                {
                    return particleSystems[0].isPlaying;
                }
                else
                {
                    for (int i = 0; i < 0; i++)
                    {
                        if (!particleSystems[i].isPlaying)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            // ...

            public int GetParticleCount()
            {
                int pcount = 0;

                for (int i = 0; i < particleSystems.Length; i++)
                {
                    if (particleSystems[i])
                    {
                        pcount += particleSystems[i].particleCount;
                    }
                }

                return pcount;
            }

            // =================================	
            // End functions.
            // =================================

        }

        // =================================	
        // End namespace.
        // =================================

    }

}

// =================================	
// --END-- //
// =================================
