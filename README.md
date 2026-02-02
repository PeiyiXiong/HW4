# HW4
## Devlog
There is no dedicated Model class for this project because it is a simple arcade game with little persistent state. The gameplay state is lightweight and event-driven, so this devlog is mostly about Control and View.
### MVC Pattern: Control & View Implementation
#### 1. Control Layer: PlayerController (Core Gameplay Logic)
The PlayerController class is the Controller in the MVC pattern. It handles player input, core gameplay rules, like jump physics and collision detection, and changes the state of the game without directly interacting with view elements like score text, audio, and the game over UI.


Key responsibilities of the Controller layer (PlayerController):


Input Handling: Listens for spacebar input to trigger jumps:

    if (Input.GetKeyDown(KeyCode.Space))
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
        GameEvents.RaisePlayerJumped(); 
    }
Collision/Trigger Logic: Detects pipe collisions to trigger death and ScoreZone triggers to increment score :

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        if (other.CompareTag("ScoreZone"))
        {
            GameEvents.RaiseScoreIncreased(); 
            Destroy(other.gameObject); 
        }
    }
Decoupling Choice: PlayerController doesn't directly change the score or play audio; instead, it raises events through GameEvents to let other parts of the game know when the state changes. This means that the controller doesn't know anything about the UI, audio, or score systems.
#### 2. View Layer: ScoreManager (Presentation & Feedback)
The ScoreManager class is the View in the MVC pattern. It takes care of all the visual and auditory feedback for the game, like showing the score, the game over text, and sound effects. It also responds to events that the Controller (PlayerController) or other systems trigger.


Key responsibilities of the View layer (ScoreManager):


UI Updates: Updates the score text when a OnScoreIncreased event is raised:

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
Game Over Presentation: Activates the game over text, stops pipe/player movement, and plays death audio when OnPlayerDied is raised.


Audio Feedback: Plays jump/score/death sounds in response to OnPlayerJumped, OnScoreIncreased, and OnPlayerDied events (no direct calls from PlayerController).


Important for the View layer: ScoreManager never changes the rules of the game; it only reacts to events. It doesn't mention PlayerController or PipeMovement; it just listens for global events to change the presentation.
### Decoupling Mechanisms: Events & Singleton
#### 1. GameEvents: Decoupling Control and View with Event-Driven Design
The GameEvents static class, a central event bus, is the primary tool for decoupling the Controller (PlayerController) and View (ScoreManager). It defines typed events for core game state changes:


OnScoreIncreased: Triggered by PlayerController when the player passes a ScoreZone, handled by ScoreManager to update the score.


OnPlayerDied: Triggered by PlayerController on pipe collision, handled by ScoreManager to show game over UI and stop gameplay.


OnPlayerJumped: Triggered by PlayerController on spacebar press, handled by ScoreManager to play jump audio.


This event-driven flow ensures:

PlayerController (Control) doesn't need to know about ScoreManager (View); it just sends events when something happens.


You can change ScoreManager (View) without changing PlayerController, for example, by changing the font of the score text or swapping audio clips.



#### 2. Singleton Pattern in ScoreManager: Consistent View Access
The ScoreManager uses a Singleton pattern to ensure a single, globally accessible instance of the View layer:

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

There is no longer a need for many different places in the codebase to refer to ScoreManager (for example, PlayerController never needs a [SerializeField] ScoreManager scoreManager field).


Ensures that the View layer stays the same and doesn't change when scenes are loaded (via DontDestroyOnLoad).


Keeps things separate: The Singleton is only used for the View layer to manage its own instance; PlayerController still uses events instead of directly calling ScoreManager.instance.


### Key Takeaways
Control (PlayerController): Owns input and gameplay logic, emits events for state changes (no view references).


View (ScoreManager): Owns presentation (UI/audio), subscribes to events (no control logic).


Events (GameEvents): Act as the "middleman" between Control and View, eliminating direct dependencies.


Singleton (ScoreManager): Ensures a single, reliable View instance without coupling it to other systems.


Without events, PlayerController would need direct references to UI and audio systems, increasing coupling and making future changes, such as adding new UI feedback, more error-prone.


## Open-Source Assets

- [Brackey's Platformer Bundle](https://brackeysgames.itch.io/brackeys-platformer-bundle) - sound effects
- [2D pixel art seagull sprites](https://elthen.itch.io/2d-pixel-art-seagull-sprites) - seagull sprites
- [1](https://freesound.org) - sound effects
- [Bird Sprites with Animations](https://carysaurus.itch.io/bird-sprites) - bird sprites
- [Industrial Pipe Platformer Tileset](https://wwolf-w.itch.io/industrial-pipe-platformer-tileset) - pipe sprites
