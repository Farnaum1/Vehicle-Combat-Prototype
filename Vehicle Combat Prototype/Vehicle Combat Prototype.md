
**Combat Vehicle System README**

**Overview**

This project implements a modular and extensible Combat Vehicle System in Unity. The system supports multiple vehicle types (such as Tank and \
CombatCar), AI‑controlled combat behavior, event‑driven scoring, and a decoupled gameplay architecture.

The architecture focuses on clean object‑oriented design using inheritance, polymorphism, ScriptableObject data, and event‑based communication between systems.

**Key Concepts Used**

• Inheritance and Polymorphism\
• Event‑Driven Architecture\
• ScriptableObject Data‑Driven Design\
• Registry Pattern for performance\
• Decoupled Score System 

**System Architecture**

**Vehicle (Abstract Base Class)** \* Tank \* CombatCar

**Weapon System** \* RocketLauncher \* MachineGun \* Projectile

**Supporting Systems** \* AI System \* Spawn System \* Score System \* Event System


**Vehicle System**

**Vehicle Base Class**

The Vehicle class is the main gameplay entity and acts as an abstract base class.

|public abstract class Vehicle : MonoBehaviour, IDamageable|
| :-: |

**Responsibilities:** \* Health management \* Team assignment \* Weapon spawning \* Firing logic \* Damage handling \* Repair logic \* Destruction handling \* Raising gameplay events

Concrete vehicles such as Tank and CombatCar inherit from it, allowing systems like AI, SpawnManager, and ScoreManager to work with the base 

Vehicle type polymorphically.

**IDamageable Interface**

|public interface IDamageable { void TakeDamage(DamageInfo|
| :- |

|damageInfo); }|
| :- |

The IDamageable interface allows any object to receive damage without exposing its internal implementation. Objects implementing this include vehicles, destructible objects, and buildings. Projectiles communicate exclusively with this interface.

**Weapon and Projectile System**

Weapons are dynamically spawned by vehicles using data defined in ScriptableObjects. Each weapon maintains an owner reference and team identification.


**Projectile System**

When a projectile collides with a target, it performs the following: 1. Check for 

IDamageable. 2. Call TakeDamage(). 3. Vehicle processes damage logic.

**Rocket Projectile Improvements:** To prevent self-collision, the system utilizes 

Physics.IgnoreCollision and adds a spawn offset in the forward

direction. Rigidbody settings utilize IsKinematic = true and 

`	`Continuous Speculative collision detection for high-performance, accurate physics.

**AI System**

The VehicleAIController manages complex combat behavior, including: \* Enemy detection and targeting. \* Movement logic: Strafing, dodging, and \
approaching. \* Retreat and cover-seeking behavior based on health thresholds. \* Firing constraints: Only fires when the aiming angle is optimal.

**Cover System**

AI searches the CoverPointRegistry to identify valid positions. While in cover, the AI remains oriented toward the enemy while performing tactical movement and maintaining fire.

**Registry Pattern**

Objects register themselves during OnEnable() and unregister in 

`	`OnDisable(), ensuring high-performance AI queries and cleaner scene management.


**Spawn System**

The SpawnManager initializes vehicles at runtime using 

VehicleSpawnInfo. This data-driven approach allows for easy level

configuration, supporting any vehicle type that inherits from the Vehicle base class.

**Score System & Event System**

The ScoreManager utilizes an event‑driven design, subscribing to the 

GameEvents.VehicleDestroyed event. 

**Flow:** 1. Projectile hits vehicle. 2. Vehicle.TakeDamage() is invoked. 3. 

VehicleDamaged event is raised. 4. If health reaches zero, 

`	`VehicleDestroyed is raised. 5. ScoreManager updates the attacker's team score.

This decoupling ensures that the scoring logic remains independent of the vehicle's internal state.

**Data Driven Design**

Vehicle statistics (e.g., MaxHealth, Speed, Weapon type) are stored in 

ScriptableObject assets. This allows for rapid balancing and iteration without modifying the underlying source code.

**Suggested Project Structure**

• **Core/**: Vehicle.cs, IDamageable.cs, DamageInfo.cs\
• **Vehicles/**: Tank.cs, CombatCar.cs\
• **Weapons/**: Weapon.cs, RocketLauncher.cs, RocketProjectile.cs• **AI/**: VehicleAIController.cs\
• **Systems/**: SpawnManager.cs, ScoreManager.cs, GameEvents.cs, VehicleRegistry.cs, CoverPointRegistry.cs


• **Data/**: VehicleData assets, WeaponData assets

**Author:** Farnaum Ferdowsi

