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
        +GetDefensePower() int
        +GetAvailableMoves() List~CombatMoveData~
        +OnCombatStart() void
        +OnCombatEnd(bool won) void
    }

    IDamageable <|-- ICombatant : extends
```

**Enums:**
```
GameState: Exploration, Combat, Puzzle, Inventory, Paused, Transition
MoveType: Attack, Heal, Buff, Debuff
MoveTarget: Self, Enemy
AbilityType: DoubleJump, DashStrike, Shield, Grapple, CombatMove
CombatPhase: PlayerSelectMove, PlayerExecute, EnemyWindup, PlayerParry: EnemyExecute, Victory, Defeat
ParryResult: Perfect, Partial, Miss
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
        +int baseDefense
        +GameObject modelPrefab
        +List~CombatMoveData~ baseMoves
    }

    class PlayerCharacterData {
        <<ScriptableObject>>
        +float moveSpeed
        +float jumpForce
        +Vector3 cameraOffset
        +float cameraAngle
    }

    class EnemyData {
        <<ScriptableObject>>
        +float aggroRange
        +int keysDropped
        +float healThreshold
        +float specialMoveChance
    }

    class CombatMoveData {
        <<ScriptableObject>>
        +string moveName
        +MoveType moveType
        +MoveTarget target
        +int power
        +float critChance
        +float accuracy
        +AudioClip soundEffect
        +GameObject vfxPrefab
    }

    class WeaponData {
        <<ScriptableObject>>
        +string weaponName
        +GameObject modelPrefab
        +int bonusDamage
        +float bonusCritChance
        +List~CombatMoveData~ weaponMoves
    }

    class HatData {
        <<ScriptableObject>>
        +string hatName
        +GameObject modelPrefab
        +int bonusDefense
        +int bonusMaxHealth
    }

    class AbilityData {
        <<ScriptableObject>>
        +string abilityName
        +AbilityType abilityType
        +float abilityValue
        +CombatMoveData grantedCombatMove
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

    class LockpickPuzzleData {
        <<ScriptableObject>>
        +int pinCount
        +float sweetSpotSize
        +float rotationSpeed
    }

    CharacterData <|-- PlayerCharacterData
    CharacterData <|-- EnemyData
    CharacterData o-- CombatMoveData : baseMoves
    WeaponData o-- CombatMoveData : weaponMoves
    AbilityData o-- CombatMoveData : grantedCombatMove
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
        +SetInputEnabled(bool enabled) void
        +OnMove() void
        +OnJump() void
        +OnInteract() void
        +OnInventory() void
    }

    class PlayerStats {
        <<MonoBehaviour>>
        -int currentHealth
        +Action OnHealthChanged
        +GetTotalAttack() int
        +GetTotalDefense() int
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
        +UnlockAbility(AbilityData ability) void
        +HasAbility(AbilityType type) bool
        +TryDoubleJump() bool
        +TryDash() void
        +TryShield() void
        +GetAbilityCombatMoves() List~CombatMoveData~
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
        +GetAttackPower() int
        +GetDefensePower() int
        +GetAvailableMoves() List~CombatMoveData~
        +TakeDamage(int amount) void
        +OnCombatEnd(bool won) void
    }

    EnemyController ..|> ICombatant
    EnemyController --> EnemyData : reads
```

- One `EnemyController` prefab variant per enemy type
- Aggro detection (triggers combat) handled in `EnemyController` 
- On defeat: awards keys, plays death animation, disables self, etc.

---

## Combat System

Turn based 1v1. Operates on `ICombatant`.

```mermaid
classDiagram
    direction TB

    class CombatManager {
        <<MonoBehaviour>>
        -ICombatant player
        -ICombatant enemy
        -CombatPhase currentPhase
        +StartCombat(ICombatant p, ICombatant e) void
        +OnMoveSelected(CombatMoveData move) void
        +OnParryResult(ParryResult result) void
        +EndCombat(bool playerWon) void
    }

    class DamageCalculator {
        <<static>>
        +CalculateDamage(int atk, CombatMoveData move, int def, float crit, ParryResult parry)$ int
        +CalculateHeal(CombatMoveData move)$ int
    }

    class ParrySystem {
        <<MonoBehaviour>>
        -float parryWindowDuration
        -float perfectWindowDuration
        +Action~ParryResult~ OnParryComplete
        +StartParryWindow() void
        +OnParryInput() void
    }

    class EnemyCombatAI {
        <<MonoBehaviour>>
        +SelectMove(ICombatant enemy, ICombatant player) CombatMoveData
    }

    CombatManager --> ICombatant : player
    CombatManager --> ICombatant : enemy
    CombatManager --> ParrySystem
    CombatManager --> EnemyCombatAI
    CombatManager ..> DamageCalculator : static calls
    EnemyCombatAI --> EnemyData : reads AI params
```

**Turn order:**

1. Player picks a move -> player executes
2. Enemy AI picks a move -> parry window opens → enemy executes
3. Repeat until one side is dead

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
    }

    class CombatUI {
        <<MonoBehaviour>>
        +ShowMoveSelection(List~CombatMoveData~ moves, Action~CombatMoveData~ onSelect) void
        +ShowParryPrompt() void
        +HideParryPrompt() void
        +UpdateHealthBars(int pHP, int pMax, int eHP, int eMax) void
        +ShowDamageNumber(int amount, bool isCrit) void
        +ShowResult(string message) void
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
    UIManager *-- CombatUI
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
    class DamageCalculator {
        <<static>>
    }
    class ParrySystem
    class EnemyCombatAI
    class LevelGate
    class TreasureChest
    class Hazard
    class PuzzleManager
    class LockpickPuzzleUI
    class HUDController
    class CombatUI
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

    %% Combat
    CombatManager --> ParrySystem
    CombatManager --> EnemyCombatAI
    CombatManager --> CombatUI
    CombatManager ..> DamageCalculator

    %% UI composition
    UIManager *-- HUDController
    UIManager *-- CombatUI
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
    CombatManager --> ICombatant : enemy
```
