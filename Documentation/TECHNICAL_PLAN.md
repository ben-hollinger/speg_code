# Technical Plan

Defines core classes, relationships, and responsibilities. 

---

## Enums & Interfaces

```mermaid
classDiagram
    direction TB

    class IDamageable {
        <<interface>>
        +int CurrentHealth
        +int MaxHealth
        +bool IsDead
        +TakeDamage(int amount) void
        +Heal(int amount) void
    }

    class IInteractable {
        <<interface>>
        +string InteractionPrompt
        +Interact(PlayerController player) void
    }

    class ICombatant {
        <<interface>>
        +string DisplayName
        +GetAttackPower() int
        +TakeDamage(int amount) void
        +OnCombatStart() void
        +OnCombatEnd(bool won) void
    }

    IDamageable <|-- ICombatant : extends
```

**Enums:**
```
GameState: Exploration, Combat, Puzzle, Inventory, Paused, Transition
AbilityType: Dodge, DashStrike, Block, Grapple, MeleeAttack, RangedAttack
BulletPatternType: Straight, Spread, Circle, Wave, Spiral
AttackType: Melee, Ranged, Projectile
```

---

## ScriptableObject Data Layer

All game content is defined as ScriptableObject assets. Code never changes when adding new enemies, weapons, or levels.

```mermaid
classDiagram
    direction TB

    class CharacterData {
        <<abstract ScriptableObject>>
        +string characterName
        +int maxHealth
        +int baseAttack
        +GameObject modelPrefab
    }

    class PlayerCharacterData {
        <<ScriptableObject>>
        +float moveSpeed
        +float dodgeSpeed
        +float dodgeDuration
        +float dodgeCooldown
    }

    class EnemyData {
        <<ScriptableObject>>
        +int keysDropped
        +float attackCooldown
        +List~BulletPatternData~ bulletPatterns
        +AnimationClip deathAnimation
        +AudioClip deathSFX
    }

    class BulletPatternData {
        <<ScriptableObject>>
        +string patternName
        +BulletPatternType patternType
        +float shootSpeed
        +int bulletCount
        +float spreadAngle
        +float fireRate
        +AudioClip fireSFX
        +GameObject bulletPrefab
        +AnimationClip attackAnimation
    }

    class WeaponData {
        <<ScriptableObject>>
        +string weaponName
        +GameObject modelPrefab
        +int baseDamage
        +float fireRate
        +float bulletSpeed
        +GameObject bulletPrefab
        +AnimationClip attackAnimation
        +AudioClip fireSFX
    }

    class HatData {
        <<ScriptableObject>>
        +string hatName
        +GameObject modelPrefab
        +int bonusMaxHealth
    }

    class AbilityData {
        <<ScriptableObject>>
        +string abilityName
        +AbilityType abilityType
        +float abilityValue
        +float cooldownDuration
        +AnimationClip activationAnimation
    }

    class LevelData {
        <<ScriptableObject>>
        +string levelName
        +string sceneName
        +int keysRequired
        +AbilityData abilityUnlocked
        +AudioClip backgroundMusic
    }

    class LootTableData {
        <<ScriptableObject>>
        +List~LootEntry~ loot
    }

    class BulletData {
        <<ScriptableObject>>
        +int damage
        +float lifetime
        +float speed
        +GameObject vfxPrefab
    }

    CharacterData <|-- PlayerCharacterData
    CharacterData <|-- EnemyData
    EnemyData o-- BulletPatternData : bulletPatterns
    WeaponData o-- BulletData : uses
    LevelData o-- AbilityData : abilityUnlocked
```

---

## Player System

Four sibling components on the Player GameObject. `PlayerController` is the hub and the others are focused helpers.

```mermaid
classDiagram
    direction TB

    class PlayerController {
        <<MonoBehaviour>>
        -PlayerCharacterData characterData
        -Vector3 moveDirection
        +SetInputEnabled(bool enabled) void
        +OnMove(Vector2 input) void
        +OnDodge() void
        +OnMeleeAttack() void
        +OnRangedAttack() void
        +OnInventory() void
    }

    class PlayerStats {
        <<MonoBehaviour>>
        -int currentHealth
        +Action OnHealthChanged
        +GetTotalMaxHealth() int
        +TakeDamage(int amount) void
        +Heal(int amount) void
        +ResetStats() void
    }

    class PlayerInventory {
        <<MonoBehaviour>>
        -int keyCount
        -WeaponData equippedWeapon
        -HatData equippedHat
        -List~WeaponData~ ownedWeapons
        -List~HatData~ ownedHats
        +Action OnKeysChanged
        +Action OnWeaponChanged
        +Action OnHatChanged
        +AddKeys(int count) void
        +SpendKeys(int count) bool
        +EquipWeapon(WeaponData w) void
        +EquipHat(HatData h) void
        +AddLoot(LootEntry entry) void
    }

    class PlayerAbilityHandler {
        <<MonoBehaviour>>
        -HashSet~AbilityType~ unlockedAbilities
        -Dictionary~AbilityType, float~ abilityCooldowns
        +UnlockAbility(AbilityData ability) void
        +HasAbility(AbilityType type) bool
        +TryDodge() bool
        +TryBlock() void
        +TryGrapple() void
        +IsAbilityOnCooldown(AbilityType type) bool
    }

    PlayerController ..|> ICombatant
    PlayerStats ..|> IDamageable

    PlayerController --> PlayerStats : sibling
    PlayerController --> PlayerInventory : sibling
    PlayerController --> PlayerAbilityHandler : sibling

    PlayerStats --> WeaponData : reads bonuses
    PlayerStats --> HatData : reads bonuses
    PlayerInventory --> WeaponData : owns/equips
    PlayerInventory --> HatData : owns/equips
    PlayerAbilityHandler --> AbilityData : tracks
```

---

## Enemy System

```mermaid
classDiagram
    direction TB

    class EnemyController {
        <<MonoBehaviour>>
        -EnemyData enemyData
        -int currentHealth
        -bool isDefeated
        -BulletPatternData currentPattern
        -float nextAttackTime
        +GetAttackPower() int
        +TakeDamage(int amount) void
        +ExecuteAttackPattern() void
        +PlayDeathAnimation() void
        +DropLoot() void
        +OnDefeated() void
    }

    class BulletEmitter {
        <<MonoBehaviour>>
        +FireBullets(BulletPatternData pattern, Transform firePoint) void
    }

    class Bullet {
        <<MonoBehaviour>>
        -int damage
        -float lifetime
        -Vector3 direction
        +OnTriggerEnter(Collider other) void
    }

    EnemyController ..|> ICombatant
    EnemyController --> EnemyData : reads
    EnemyController --> BulletEmitter
    BulletEmitter --> Bullet : spawns
    BulletEmitter --> BulletPatternData : reads
```

- Enemies are placed manually in level scenes and are stationary (do not chase player)
- Each enemy has a list of `BulletPatternData` assets defining their attack patterns
- Enemies fire at intervals based on `attackCooldown` and selected pattern
- On defeat: plays death animation, drops keys, plays death sound effect, disables GameObject
- Different bullet patterns (straight, spread, circle, wave, spiral) defined via SO data

---

## Combat System

Real time action combat. Player must dodge enemy bullet patterns and attacks, then strike back with melee or ranged attacks.

```mermaid
classDiagram
    direction TB

    class CombatManager {
        <<MonoBehaviour>>
        -PlayerController player
        -List~EnemyController~ activeEnemies
        +AddEnemy(EnemyController enemy) void
        +RemoveEnemy(EnemyController enemy) void
        +OnEnemyDefeated(EnemyController enemy) void
        +AreAllEnemiesDefeated() bool
    }

    class DamageCalculator {
        <<static>>
        +CalculateDamage(int atk, int baseBulletDamage, HatData hat)$ int
    }

    class DodgeSystem {
        <<MonoBehaviour>>
        -float dodgeDuration
        -float dodgeCooldown
        -bool isDodging
        +TryDodge() bool
        +IsDodging() bool
        +Update() void
    }

    class WeaponController {
        <<MonoBehaviour>>
        -WeaponData currentWeapon
        +OnMeleeAttack(Vector3 direction) void
        +OnRangedAttack(Vector3 direction) void
        +IsOnCooldown() bool
    }

    class ProjectilePool {
        <<MonoBehaviour, Singleton>>
        +GetBullet(GameObject prefab) GameObject
        +ReturnBullet(GameObject bullet) void
    }

    CombatManager --> EnemyController
    CombatManager ..> DamageCalculator : static calls
    WeaponController --> WeaponData
    WeaponController --> ProjectilePool : spawns
    DodgeSystem --> PlayerAbilityHandler
```

---

## Level & Progression System

```mermaid
classDiagram
    direction TB

    class LevelManager {
        <<MonoBehaviour, Singleton>>
        -LevelData currentLevel
        -List~LevelData~ allLevels
        +LoadLevel(LevelData level) void
        +ReloadCurrentLevel() void
        +CanAccessLevel(LevelData level, int keys) bool
    }

    class LevelGate {
        <<MonoBehaviour>>
        -LevelData targetLevel
        +Interact(PlayerController player) void
    }

    LevelGate ..|> IInteractable
    LevelGate --> LevelData : references
    LevelGate ..> LevelManager : calls LoadLevel
    LevelManager --> LevelData : manages
```

- Gates are placed manually in each level scene
- On level load: keys spent, scene loads async, ability unlocked, BGM changes
- On player death: `LevelManager.ReloadCurrentLevel()` — full scene reset

---

## Interactable & Puzzle System

```mermaid
classDiagram
    direction TB

    class TreasureChest {
        <<MonoBehaviour>>
        -LootTableData lootTable
        -LockpickPuzzleData puzzleData
        -bool isOpened
        +Interact(PlayerController player) void
        -OnPuzzleSolved() void
    }

    class Hazard {
        <<MonoBehaviour>>
        -int damage
        -bool isInstantKill
    }

    class PuzzleManager {
        <<MonoBehaviour>>
        +StartLockpick(LockpickPuzzleData data, Action onSuccess) void
    }

    class LockpickPuzzleUI {
        <<MonoBehaviour>>
        -int currentPin
        -float indicatorAngle
        +Action OnSolved
        +Action OnFailed
        +Initialize(LockpickPuzzleData data) void
        +OnPickAttempt() void
        +ResetPuzzle() void
    }

    TreasureChest ..|> IInteractable
    TreasureChest --> LootTableData
    TreasureChest --> LockpickPuzzleData
    TreasureChest ..> PuzzleManager : triggers
    PuzzleManager --> LockpickPuzzleUI
    LockpickPuzzleUI --> LockpickPuzzleData : reads
```

**Lockpick mechanic:** rotating indicator must land in a sweet spot for each pin. Miss any pin -> full reset. All pins cleared -> chest opens.

---

## UI System

```mermaid
classDiagram
    direction TB

    class UIManager {
        <<MonoBehaviour>>
        +ShowPanel(UIPanel panel) void
        +HidePanel(UIPanel panel) void
        +ShowMessage(string msg, float duration) void
    }

    class HUDController {
        <<MonoBehaviour>>
        +UpdateHealth(int current, int max) void
        +UpdateKeys(int count) void
        +UpdateEquipment(WeaponData w, HatData h) void
        +UpdateAbilities(List~AbilityData~ a) void
        +ShowWeaponCooldown(float remaining) void
    }

    class CombatHUD {
        <<MonoBehaviour>>
        +UpdateHealthBars(int pHP, int pMax) void
        +ShowDamageNumber(int amount) void
        +ShowEnemyList(List~EnemyController~ enemies) void
    }

    class InventoryUI {
        <<MonoBehaviour>>
        -PlayerInventory playerInventory
        +Open() void
        +Close() void
        +SelectItem(ScriptableObject item) void
        +OnEquipPressed() void
    }

    class PauseMenuUI {
        <<MonoBehaviour>>
        +Show() void
        +Hide() void
    }

    UIManager *-- HUDController
    UIManager *-- CombatHUD
    UIManager *-- InventoryUI
    UIManager *-- PauseMenuUI
    InventoryUI --> PlayerInventory
```

---

## Audio System

```mermaid
classDiagram
    direction TB

    class AudioManager {
        <<MonoBehaviour, Singleton>>
        +PlayMusic(AudioClip clip) void
        +StopMusic() void
        +PlaySFX(AudioClip clip) void
        +SetMusicVolume(float vol) void
        +SetSFXVolume(float vol) void
    }
```

---

## GameManager

```mermaid
classDiagram
    direction TB

    class GameManager {
        <<MonoBehaviour, Singleton>>
        -GameState currentState
        +Action~GameState~ OnGameStateChanged
        +ChangeState(GameState state) void
        +PauseGame() void
        +ResumeGame() void
        +OnPlayerDefeated() void
        +OpenInventory() void
        +CloseInventory() void
    }
```

`GameManager.ChangeState()` is the single place that enables/disables player input and shows/hides the right UI panels.

---

## Full System Overview

```mermaid
classDiagram
    direction TB

    class GameManager {
        <<Singleton>>
    }
    class CombatManager
    class LevelManager {
        <<Singleton>>
    }
    class AudioManager {
        <<Singleton>>
    }
    class UIManager
    class PlayerController
    class PlayerStats
    class PlayerInventory
    class PlayerAbilityHandler
    class EnemyController
    class BulletEmitter
    class Bullet
    class DamageCalculator {
        <<static>>
    }
    class DodgeSystem
    class WeaponController
    class ProjectilePool {
        <<Singleton>>
    }
    class LevelGate
    class TreasureChest
    class Hazard
    class PuzzleManager
    class LockpickPuzzleUI
    class HUDController
    class CombatHUD
    class InventoryUI
    class PauseMenuUI

    %% Top-level management
    GameManager --> CombatManager
    GameManager --> LevelManager
    GameManager --> UIManager
    GameManager --> AudioManager
    GameManager --> PlayerController

    %% Player composition (same GameObject)
    PlayerController --> PlayerStats : sibling
    PlayerController --> PlayerInventory : sibling
    PlayerController --> PlayerAbilityHandler : sibling
    PlayerController --> DodgeSystem : sibling
    PlayerController --> WeaponController : sibling

    %% Combat
    CombatManager --> EnemyController
    CombatManager ..> DamageCalculator
    WeaponController --> ProjectilePool
    BulletEmitter --> Bullet
    EnemyController --> BulletEmitter

    %% UI composition
    UIManager *-- HUDController
    UIManager *-- CombatHUD
    UIManager *-- InventoryUI
    UIManager *-- PauseMenuUI
    InventoryUI --> PlayerInventory

    %% Level
    LevelGate ..> LevelManager

    %% Puzzle
    TreasureChest ..> PuzzleManager
    PuzzleManager --> LockpickPuzzleUI

    %% Interface implementations
    PlayerController ..|> ICombatant
    PlayerStats ..|> IDamageable
    EnemyController ..|> ICombatant
    LevelGate ..|> IInteractable
    TreasureChest ..|> IInteractable

    %% Combat uses interfaces
    CombatManager --> ICombatant : player
```
