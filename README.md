# HW4
## Devlog
Because this project is a simple arcade game with minimal persistent state, there is no dedicated Model class; gameplay state is lightweight and event-driven, so the MVC discussion focuses on Control and View.
### MVC Pattern: Control & View Implementation
#### 1. Control Layer: PlayerController (Core Gameplay Logic)
The PlayerController class acts as the Controller in the MVC pattern—it handles player input, core gameplay rules (jump physics, collision detection), and triggers game state changes without directly interacting with view elements, like score text, audio, and game over UI.
Key responsibilities of the Controller layer (PlayerController):
Input Handling: Listens for spacebar input to trigger jumps:

    if (Input.GetKeyDown(KeyCode.Space))
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
        GameEvents.RaisePlayerJumped(); 
    }
Collision/Trigger Logic: Detects pipe collisions (to trigger death) and ScoreZone triggers to increment score :

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        if (other.CompareTag("ScoreZone"))
        {
            GameEvents.RaiseScoreIncreased(); 
            Destroy(other.gameObject); 
        }
    }
Decoupling Choice: Instead of directly updating the score or playing audio, PlayerController raises events (via GameEvents) to signal state changes. This means the controller has no references to UI, audio, or score systems—it only emits "what happened," not "how to react."
#### 2. View Layer: ScoreManager (Presentation & Feedback)
The ScoreManager class acts as the View in the MVC pattern—it handles all visual and auditory feedback for the game (score display, game over text, sound effects) and responds to events triggered by the Controller (PlayerController) or other systems.
Key responsibilities of the View layer (ScoreManager):
UI Updates: Updates the score text when a OnScoreIncreased event is raised (lines 47-58):

            private void HandleScoreIncreased(int addValue)
            {
                if (isGameOver || scoreText == null) return;
                
                score += addValue;
                scoreText.text = "Score: " + score;
                
                if (audioSource != null && scoreClip != null)
                {
                    audioSource.PlayOneShot(scoreClip);
                }
            }
Game Over Presentation: Activates the game over text, stops pipe/player movement, and plays death audio when OnPlayerDied is raised (lines 59-87).
Audio Feedback: Plays jump/score/death sounds in response to OnPlayerJumped, OnScoreIncreased, and OnPlayerDied events (no direct calls from PlayerController).
Critical to the View layer: ScoreManager never modifies gameplay logic—it only reacts to events. It has no references to PlayerController or PipeMovement; it only subscribes to global events to update presentation.
### Decoupling Mechanisms: Events & Singleton
#### 1. GameEvents: Decoupling Control and View with Event-Driven Design
The GameEvents static class (a central event bus) is the primary tool for decoupling the Controller (PlayerController) and View (ScoreManager). It defines typed events for core game state changes:
OnScoreIncreased: Triggered by PlayerController (line 41) when the player passes a ScoreZone; handled by ScoreManager (line 47) to update the score.
OnPlayerDied: Triggered by PlayerController (line 35) on pipe collision; handled by ScoreManager (line 59) to show game over UI and stop gameplay.
OnPlayerJumped: Triggered by PlayerController (line 25) on spacebar press; handled by ScoreManager (line 89) to play jump audio.
This event-driven flow ensures:
PlayerController (Control) does not need to know about ScoreManager (View) — it only raises events when something happens.
ScoreManager (View) can be modified (e.g., changing score text font, swapping audio clips) without touching PlayerController.
New systems (e.g., a high-score tracker) can subscribe to OnScoreIncreased without modifying existing code (open/closed principle).
#### 2. Singleton Pattern in ScoreManager: Consistent View Access
The ScoreManager uses a Singleton pattern (lines 16-25) to ensure a single, globally accessible instance of the View layer:

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
This design choice:
Eliminates the need for scattered references to ScoreManager across the codebase (e.g., PlayerController never needs a [SerializeField] ScoreManager scoreManager field).
Ensures the View layer is persistent and consistent (via DontDestroyOnLoad) across scene loads (if applicable).
Maintains decoupling: The Singleton is only used for the View layer to self-manage its instance—PlayerController still uses events, not direct calls to ScoreManager.instance.
### Key Takeaways
Control (PlayerController): Owns input and gameplay logic, emits events for state changes (no view references).
View (ScoreManager): Owns presentation (UI/audio), subscribes to events (no control logic).
Events (GameEvents): Act as the "middleman" between Control and View, eliminating direct dependencies.
Singleton (ScoreManager): Ensures a single, reliable View instance without coupling it to other systems.
Without events, PlayerController would need direct references to UI and audio systems, increasing coupling and making future changes (such as adding new UI feedback) more error-prone.


## Open-Source Assets

- [Brackey's Platformer Bundle](https://brackeysgames.itch.io/brackeys-platformer-bundle) - sound effects
- [2D pixel art seagull sprites](https://elthen.itch.io/2d-pixel-art-seagull-sprites) - seagull sprites
