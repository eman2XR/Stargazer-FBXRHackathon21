
// =================================	
// Namespaces.
// =================================

using UnityEngine;
using System.Collections.Generic;

using System.Threading;

// =================================	
// Define namespace.
// =================================

namespace MirzaBeig
{

	namespace Scripting
	{

		namespace Effects
		{

			// =================================	
			// Classes.
			// =================================

			[RequireComponent(typeof(ParticleSystem))]
			[AddComponentMenu("Effects/Particle Plexus")]

			public class ParticlePlexus : MonoBehaviour
			{
				// =================================	
				// Nested classes and structures.
				// =================================

				// ...

				// =================================	
				// Variables.
				// =================================

				// ...

				public float maxDistance = 1.0f;

				public int maxConnections = 5;
				public int maxLineRenderers = 100;

				[Space]

				[Range(0.0f, 1.0f)]
				public float widthFromParticle = 0.125f;

				[Space]

				[Range(0.0f, 1.0f)]
				public float colourFromParticle = 1.0f;

				[Range(0.0f, 1.0f)]
				public float alphaFromParticle = 1.0f;

				[Space]

				public AnimationCurve alphaOverNormalizedDistance = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);

				//public float fadeInTime = 0.0f;

				new ParticleSystem particleSystem;

				ParticleSystem.Particle[] particles;

				Vector3[] particlePositions;

				Color[] particleColours;
				float[] particleSizes;

				ParticleSystem.MainModule particleSystemMainModule;

				[Space]

				public LineRenderer lineRendererTemplate;
				List<LineRenderer> lineRenderers = new List<LineRenderer>();

				//[System.Serializable]
				//public struct LineRendererData
				//{
				//    public float currentStartParticleID;
				//    public float currentEndParticleID;

				//    public float previousStartParticleID;
				//    public float previousEndParticleID;

				//    public float timer; 
				//}

				//List<LineRendererData> lineRendererData = new List<LineRendererData>();

				Transform _transform;

				[Header("Triangle Mesh Settings")]

				public MeshFilter trianglesMeshFilter;
				Mesh trianglesMesh;

				List<int[]> allConnectedParticles = new List<int[]>();

				[Space]

				// Triangles can be limited to those with particles who 
				// are a fraction of the distance limit from other particles.

				// Example:

				// 1.0 == use same distance as maxDistance.
				// 0.5 == half of maxDistance.

				[Range(0.0f, 1.0f)]
				public float maxDistanceTriangleBias = 1.0f;

				[Space]

				// Additional filtering for triangle generation so that it's more based on distance to adjacent particles.

				public bool trianglesDistanceCheck = false;

				[Space]

				[Range(0.0f, 1.0f)]
				public float triangleColourFromParticle = 1.0f;

				[Range(0.0f, 1.0f)]
				public float triangleAlphaFromParticle = 1.0f;

				[Header("General Performance Settings")]

				[Range(0.0f, 1.0f)]
				public float delay = 0.0f;

				float timer;

				public bool alwaysUpdate = false;
				bool visible;

				//List<Vector4> customParticleData = new List<Vector4>();
				//int uniqueParticleID;

				// =================================	
				// Functions.
				// =================================

				// ...

				void Start()
				{
					particleSystem = GetComponent<ParticleSystem>();
					particleSystemMainModule = particleSystem.main;

					_transform = transform;

					if (trianglesMeshFilter)
					{
						trianglesMesh = new Mesh();
						trianglesMeshFilter.mesh = trianglesMesh;

						// Unparent since I don't want it following me, but I may have
						// parented in the scene for organization.

						// EDIT: Nevermind, let the user deal with this as they see fit.

						//if (trianglesMeshFilter.transform != transform)
						//{
						//	trianglesMeshFilter.transform.parent = null;
						//}
					}
				}

				// ...

				void OnDisable()
				{
					for (int i = 0; i < lineRenderers.Count; i++)
					{
						lineRenderers[i].enabled = false;
					}
				}

				// ...

				void OnBecameVisible()
				{
					visible = true;
				}
				void OnBecameInvisible()
				{
					visible = false;
				}

				// ...

				void LateUpdate()
				{
					if (trianglesMeshFilter)
					{
						switch (particleSystemMainModule.simulationSpace)
						{
							case ParticleSystemSimulationSpace.World:
								{
									// Make sure this is always at origin, or triangle mesh moves out of sync since
									// the mesh triangle vertices are already set to the world space particle positions.

									trianglesMeshFilter.transform.position = Vector3.zero;

									break;
								}
							case ParticleSystemSimulationSpace.Local:
								{
									// In local space it should follow me.

									trianglesMeshFilter.transform.position = transform.position;
									trianglesMeshFilter.transform.rotation = transform.rotation;

									break;
								}
							case ParticleSystemSimulationSpace.Custom:
								{
									// In custom space it should follow the custom transform.

									trianglesMeshFilter.transform.position = particleSystemMainModule.customSimulationSpace.position;
									trianglesMeshFilter.transform.rotation = particleSystemMainModule.customSimulationSpace.rotation;

									break;
								}
						}
					}

					// Filter doesn't exist but mesh is there? That means the filter reference was lost. Clear the mesh.
					// AKA... If mesh filter is gone (deleted and/or reference nulled), clear the mesh.

					else if (trianglesMesh)
					{
						trianglesMesh.Clear();
					}

					int lineRenderersCount = lineRenderers.Count;

					// In case max line renderers value is changed at runtime -> destroy extra.

					if (lineRenderersCount > maxLineRenderers)
					{
						for (int i = maxLineRenderers; i < lineRenderersCount; i++)
						{
							Destroy(lineRenderers[i].gameObject);
						}

						lineRenderers.RemoveRange(maxLineRenderers, lineRenderersCount - maxLineRenderers);
						//lineRendererData.RemoveRange(maxLineRenderers, lineRenderersCount - maxLineRenderers);

						lineRenderersCount -= lineRenderersCount - maxLineRenderers;
					}

					if (alwaysUpdate || visible)
					{
						// Prevent constant allocations so long as max particle count doesn't change.

						int maxParticles = particleSystemMainModule.maxParticles;

						if (particles == null || particles.Length < maxParticles)
						{
							particles = new ParticleSystem.Particle[maxParticles];

							particlePositions = new Vector3[maxParticles];

							particleColours = new Color[maxParticles];
							particleSizes = new float[maxParticles];
						}

						float deltaTime = Time.deltaTime;

						timer += deltaTime;

						if (timer >= delay)
						{
							timer = 0.0f;

							int lrIndex = 0;

							allConnectedParticles.Clear();

							// Only update if drawing/making connections.

							if (maxConnections > 0 && maxLineRenderers > 0)
							{
								particleSystem.GetParticles(particles);
								//particleSystem.GetCustomParticleData(customParticleData, ParticleSystemCustomData.Custom1);

								int particleCount = particleSystem.particleCount;

								float maxDistanceSqr = maxDistance * maxDistance;

								ParticleSystemSimulationSpace simulationSpace = particleSystemMainModule.simulationSpace;
								ParticleSystemScalingMode scalingMode = particleSystemMainModule.scalingMode;

								Transform customSimulationSpaceTransform = particleSystemMainModule.customSimulationSpace;

								Color lineRendererStartColour = lineRendererTemplate.startColor;
								Color lineRendererEndColour = lineRendererTemplate.endColor;

								float lineRendererStartWidth = lineRendererTemplate.startWidth * lineRendererTemplate.widthMultiplier;
								float lineRendererEndWidth = lineRendererTemplate.endWidth * lineRendererTemplate.widthMultiplier;

								// Save particle properties in a quick loop (accessing these is expensive and loops significantly more later, so it's better to save them once now).

								for (int i = 0; i < particleCount; i++)
								{
									particlePositions[i] = particles[i].position;

									particleColours[i] = particles[i].GetCurrentColor(particleSystem);
									particleSizes[i] = particles[i].GetCurrentSize(particleSystem);

									// Default is 0.0f, so if default, this is a new particle and I need to assign a custom ID.

									//if (customParticleData[i].x == 0.0f)
									//{
									//    // ++value -> increment first, then return that value.
									//    // That way it won't be zero (default) the first time I use it.

									//    customParticleData[i] = new Vector4(++uniqueParticleID, 0, 0, 0);
									//}
								}

								//particleSystem.SetCustomParticleData(customParticleData, ParticleSystemCustomData.Custom1);

								Vector3 p1p2_difference;

								// If in world space, there's no need to do any of the extra calculations... simplify the loop!

								if (simulationSpace == ParticleSystemSimulationSpace.World)
								{
									for (int i = 0; i < particleCount; i++)
									{
										if (lrIndex == maxLineRenderers)
										{
											break;
										}

										Color particleColour = particleColours[i];

										Color lineStartColour = Color.LerpUnclamped(lineRendererStartColour, particleColour, colourFromParticle);
										float lineStartColourOriginalAlpha = Mathf.LerpUnclamped(lineRendererStartColour.a, particleColour.a, alphaFromParticle);

										lineStartColour.a = lineStartColourOriginalAlpha;

										float lineStartWidth = Mathf.LerpUnclamped(lineRendererStartWidth, particleSizes[i], widthFromParticle);

										int connections = 0;
										int[] connectedParticles = new int[maxConnections + 1];

										for (int j = i + 1; j < particleCount; j++)
										{
											p1p2_difference.x = particlePositions[i].x - particlePositions[j].x;
											p1p2_difference.y = particlePositions[i].y - particlePositions[j].y;
											p1p2_difference.z = particlePositions[i].z - particlePositions[j].z;

											//float distanceSqr = Vector3.SqrMagnitude(p1p2_difference);

											float distanceSqr =

												p1p2_difference.x * p1p2_difference.x +
												p1p2_difference.y * p1p2_difference.y +
												p1p2_difference.z * p1p2_difference.z;

											if (distanceSqr <= maxDistanceSqr)
											{
												LineRenderer lr;

												if (lrIndex == lineRenderersCount)
												{
													lr = Instantiate(lineRendererTemplate, _transform, false);

													lineRenderers.Add(lr);
													lineRenderersCount++;
												}

												lr = lineRenderers[lrIndex]; lr.enabled = true;

												lr.SetPosition(0, particlePositions[i]);
												lr.SetPosition(1, particlePositions[j]);

												float alphaAttenuation = alphaOverNormalizedDistance.Evaluate(distanceSqr / maxDistanceSqr);
												lineStartColour.a = lineStartColourOriginalAlpha * alphaAttenuation;

												lr.startColor = lineStartColour;

												particleColour = particleColours[j];

												Color lineEndColour = Color.LerpUnclamped(lineRendererEndColour, particleColour, colourFromParticle);
												lineEndColour.a = Mathf.LerpUnclamped(lineRendererEndColour.a, particleColour.a, alphaFromParticle);

												lr.endColor = lineEndColour;

												lr.startWidth = lineStartWidth;
												lr.endWidth = Mathf.LerpUnclamped(lineRendererEndWidth, particleSizes[j], widthFromParticle);

												lrIndex++;
												connections++;

												// Intentionally taken AFTER connections++ (because index = 0 is the i'th / line origin particle).

												connectedParticles[connections] = j;

												if (connections == maxConnections || lrIndex == maxLineRenderers)
												{
													break;
												}
											}
										}

										if (connections >= 2)
										{
											connectedParticles[0] = i;
											allConnectedParticles.Add(connectedParticles);
										}
									}
								}
								else
								{
									Vector3 position = Vector3.zero;
									Quaternion rotation = Quaternion.identity;
									Vector3 localScale = Vector3.one;

									Transform simulationSpaceTransform = _transform;

									switch (simulationSpace)
									{
										case ParticleSystemSimulationSpace.Local:
											{
												position = simulationSpaceTransform.position;
												rotation = simulationSpaceTransform.rotation;
												localScale = simulationSpaceTransform.localScale;

												break;
											}
										case ParticleSystemSimulationSpace.Custom:
											{
												simulationSpaceTransform = customSimulationSpaceTransform;

												position = simulationSpaceTransform.position;
												rotation = simulationSpaceTransform.rotation;
												localScale = simulationSpaceTransform.localScale;

												break;
											}
										default:
											{
												throw new System.NotSupportedException(

													string.Format("Unsupported scaling mode '{0}'.", simulationSpace));
											}
									}

									// I put these here so I can take out the default exception case.
									// Else I'd have a compiler error for potentially unassigned variables.

									Vector3 p1_position = Vector3.zero;
									Vector3 p2_position = Vector3.zero;

									for (int i = 0; i < particleCount; i++)
									{
										if (lrIndex == maxLineRenderers)
										{
											break;
										}

										switch (simulationSpace)
										{
											case ParticleSystemSimulationSpace.Local:
											case ParticleSystemSimulationSpace.Custom:
												{
													switch (scalingMode)
													{
														case ParticleSystemScalingMode.Hierarchy:
															{
																p1_position = simulationSpaceTransform.TransformPoint(particlePositions[i]);

																break;
															}
														case ParticleSystemScalingMode.Local:
															{
																// Order is important.

																//p1_position = Vector3.Scale(particlePositions[i], localScale);

																p1_position.x = particlePositions[i].x * localScale.x;
																p1_position.y = particlePositions[i].y * localScale.y;
																p1_position.z = particlePositions[i].z * localScale.z;

																p1_position = rotation * p1_position;
																//p1_position += position;

																p1_position.x += position.x;
																p1_position.y += position.y;
																p1_position.z += position.z;

																break;
															}
														case ParticleSystemScalingMode.Shape:
															{
																// Order is important.

																p1_position = rotation * particlePositions[i];
																//p1_position += position;

																p1_position.x += position.x;
																p1_position.y += position.y;
																p1_position.z += position.z;

																break;
															}
														default:
															{
																throw new System.NotSupportedException(

																	string.Format("Unsupported scaling mode '{0}'.", scalingMode));
															}
													}

													break;
												}
										}

										Color particleColour = particleColours[i];

										Color lineStartColour = Color.LerpUnclamped(lineRendererStartColour, particleColour, colourFromParticle);
										float lineStartColourOriginalAlpha = Mathf.LerpUnclamped(lineRendererStartColour.a, particleColour.a, alphaFromParticle);

										lineStartColour.a = lineStartColourOriginalAlpha;

										float lineStartWidth = Mathf.LerpUnclamped(lineRendererStartWidth, particleSizes[i], widthFromParticle);

										int connections = 0;
										int[] connectedParticles = new int[maxConnections + 1];

										for (int j = i + 1; j < particleCount; j++)
										{
											// Note that because particles array is not sorted by distance,
											// but rather by spawn time (I think), the connections made are 
											// not necessarily the closest.

											switch (simulationSpace)
											{
												case ParticleSystemSimulationSpace.Local:
												case ParticleSystemSimulationSpace.Custom:
													{
														switch (scalingMode)
														{
															case ParticleSystemScalingMode.Hierarchy:
																{
																	p2_position = simulationSpaceTransform.TransformPoint(particlePositions[j]);

																	break;
																}
															case ParticleSystemScalingMode.Local:
																{
																	// Order is important.

																	//p2_position = Vector3.Scale(particlePositions[j], localScale);

																	p2_position.x = particlePositions[j].x * localScale.x;
																	p2_position.y = particlePositions[j].y * localScale.y;
																	p2_position.z = particlePositions[j].z * localScale.z;

																	p2_position = rotation * p2_position;
																	//p2_position += position;

																	p2_position.x += position.x;
																	p2_position.y += position.y;
																	p2_position.z += position.z;

																	break;
																}
															case ParticleSystemScalingMode.Shape:
																{
																	// Order is important.

																	p2_position = rotation * particlePositions[j];
																	//p2_position += position;

																	p2_position.x += position.x;
																	p2_position.y += position.y;
																	p2_position.z += position.z;

																	break;
																}
															default:
																{
																	throw new System.NotSupportedException(

																		string.Format("Unsupported scaling mode '{0}'.", scalingMode));
																}
														}

														break;
													}
											}

											p1p2_difference.x = particlePositions[i].x - particlePositions[j].x;
											p1p2_difference.y = particlePositions[i].y - particlePositions[j].y;
											p1p2_difference.z = particlePositions[i].z - particlePositions[j].z;

											// Note that distance is always calculated in WORLD SPACE.
											// Scaling the particle system will stretch the distances
											// and may require adjusting the maxDistance value.

											// I could also do it in local space (which may actually make more
											// sense) by just getting the difference of the positions without
											// all the transformations. This also provides opportunity for 
											// optimization as I can limit the world space transform calculations
											// to only happen if a particle is within range. 

											// Think about: Putting in a bool to switch between the two?

											//float distanceSqr = Vector3.SqrMagnitude(p1p2_difference);

											float distanceSqr =

												p1p2_difference.x * p1p2_difference.x +
												p1p2_difference.y * p1p2_difference.y +
												p1p2_difference.z * p1p2_difference.z;

											// If distance to particle within range, add new vertex position.

											// The larger the max distance, the quicker connections will
											// reach its max, terminating the loop earlier. So even though more lines have
											// to be drawn, it's still faster to have a larger maxDistance value because
											// the call to Vector3.Distance() is expensive.

											if (distanceSqr <= maxDistanceSqr)
											{
												LineRenderer lr;

												if (lrIndex == lineRenderersCount)
												{
													lr = Instantiate(lineRendererTemplate, _transform, false);

													lineRenderers.Add(lr);
													lineRenderersCount++;

													//lineRendererData.Add(new LineRendererData());
												}

												lr = lineRenderers[lrIndex];
												//LineRendererData lrd = lineRendererData[lrIndex];

												lr.enabled = true;

												//lrd.previousStartParticleID = lrd.currentStartParticleID;
												//lrd.previousEndParticleID = lrd.currentEndParticleID;

												//lrd.currentStartParticleID = customParticleData[i].x;
												//lrd.currentEndParticleID = customParticleData[j].x;

												lr.SetPosition(0, p1_position);
												lr.SetPosition(1, p2_position);

												//if (lrd.currentStartParticleID != lrd.previousStartParticleID || lrd.currentEndParticleID != lrd.previousEndParticleID)
												//{
												//    lrd.timer = 0.0f;
												//}

												//if (lrd.timer < 1.0f)
												//{
												//    lrd.timer += deltaTime / fadeInTime;
												//}

												//if (lrd.timer > 1.0f)
												//{
												//    lrd.timer = 1.0f;
												//}

												float alphaAttenuation = alphaOverNormalizedDistance.Evaluate(distanceSqr / maxDistanceSqr);
												//float alphaAttenuation = lrd.timer * alphaOverNormalizedDistance.Evaluate(distanceSqr / maxDistanceSqr);

												lineStartColour.a = lineStartColourOriginalAlpha * alphaAttenuation;

												lr.startColor = lineStartColour;

												particleColour = particleColours[j];

												Color lineEndColour = Color.LerpUnclamped(lineRendererEndColour, particleColour, colourFromParticle);
												lineEndColour.a = Mathf.LerpUnclamped(lineRendererEndColour.a, particleColour.a, alphaFromParticle) * alphaAttenuation;

												lr.endColor = lineEndColour;

												lr.startWidth = lineStartWidth;
												lr.endWidth = Mathf.LerpUnclamped(lineRendererEndWidth, particleSizes[j], widthFromParticle);

												//lineRendererData[lrIndex] = lrd;

												lrIndex++;
												connections++;

												// Intentionally taken AFTER connections++ (because index = 0 is the i'th / line origin particle).

												connectedParticles[connections] = j;

												if (connections == maxConnections || lrIndex == maxLineRenderers)
												{
													break;
												}
											}
										}

										if (connections >= 2)
										{
											connectedParticles[0] = i;
											allConnectedParticles.Add(connectedParticles);
										}
									}
								}
							}

							// Disable remaining line renderers from the pool that weren't used.

							for (int i = lrIndex; i < lineRenderersCount; i++)
							{
								if (lineRenderers[i].enabled)
								{
									lineRenderers[i].enabled = false;
								}
							}

							// I check against the filter rather than the mesh because the mesh should always exist as long as the filter reference is there.
							// This way I can stop drawing/updating should the filter reference be lost.

							if (trianglesMeshFilter)
							{
								// Triangles mesh. 

								// For efficiency (and my own general laziness), I only bother taking the first triangle formed.
								// It doesn't matter all that much since this is an abstract effect anyway.

								int vertexCount = allConnectedParticles.Count * 3;

								Vector3[] vertices = new Vector3[vertexCount];
								int[] triangles = new int[vertexCount];

								Vector2[] uv = new Vector2[vertexCount];

								Color[] colours = new Color[vertexCount];

								float maxDistanceSqr = (maxDistance * maxDistance) * maxDistanceTriangleBias;

								for (int i = 0; i < allConnectedParticles.Count; i++)
								{
									int[] connectedParticles = allConnectedParticles[i];

									float distanceSqr = 0.0f;

									if (trianglesDistanceCheck)
									{
										Vector3 particlePositionA = particlePositions[connectedParticles[1]];
										Vector3 particlePositionB = particlePositions[connectedParticles[2]];

										//distance = Vector3.Distance(particlePositionA, particlePositionB);

										Vector3 difference;

										difference.x = particlePositionA.x - particlePositionB.x;
										difference.y = particlePositionA.y - particlePositionB.y;
										difference.z = particlePositionA.z - particlePositionB.z;

										distanceSqr =

											difference.x * difference.x +
											difference.y * difference.y +
											difference.z * difference.z;
									}

									if (distanceSqr < maxDistanceSqr)
									{
										int i3 = i * 3;
										
										vertices[i3 + 0] = particlePositions[connectedParticles[0]];
										vertices[i3 + 1] = particlePositions[connectedParticles[1]];
										vertices[i3 + 2] = particlePositions[connectedParticles[2]];

										uv[i3 + 0] = new Vector2(0.0f, 0.0f);
										uv[i3 + 1] = new Vector2(0.0f, 1.0f);
										uv[i3 + 2] = new Vector2(1.0f, 1.0f);

										triangles[i3 + 0] = i3 + 0;
										triangles[i3 + 1] = i3 + 1;
										triangles[i3 + 2] = i3 + 2;

										colours[i3 + 0] = particleColours[connectedParticles[0]];
										colours[i3 + 1] = particleColours[connectedParticles[1]];
										colours[i3 + 2] = particleColours[connectedParticles[2]];

										colours[i3 + 0] = Color.LerpUnclamped(Color.white, particleColours[connectedParticles[0]], triangleColourFromParticle);
										colours[i3 + 1] = Color.LerpUnclamped(Color.white, particleColours[connectedParticles[1]], triangleColourFromParticle);
										colours[i3 + 2] = Color.LerpUnclamped(Color.white, particleColours[connectedParticles[2]], triangleColourFromParticle);

										colours[i3 + 0].a = Mathf.LerpUnclamped(1.0f, particleColours[connectedParticles[0]].a, triangleAlphaFromParticle);
										colours[i3 + 1].a = Mathf.LerpUnclamped(1.0f, particleColours[connectedParticles[1]].a, triangleAlphaFromParticle);
										colours[i3 + 2].a = Mathf.LerpUnclamped(1.0f, particleColours[connectedParticles[2]].a, triangleAlphaFromParticle);
									}
								}

								trianglesMesh.Clear();

								trianglesMesh.vertices = vertices;

								trianglesMesh.uv = uv;

								trianglesMesh.triangles = triangles;
								trianglesMesh.colors = colours;
							}
						}
					}
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

}

// =================================	
// --END-- //
// =================================
