# BeatKana

BeatKana is an educational rhythm game built in Unity that teaches Japanese Hiragana through timing-based gameplay, progression systems, and language-aware mechanics.

Unlike many educational games that rely on simple memorization quizzes, BeatKana was designed around reinforcing actual reading fluency, kana recognition speed, sound recall, and typing accuracy under timing pressure. The gameplay loop combines rhythm-game mechanics with Japanese language structure to encourage deeper pattern recognition and rapid kana processing.

> Current project status: Prototype / active architectural development

---

## Overview

BeatKana combines:

- Rhythm game timing mechanics
- Japanese Hiragana recognition
- Kana composition systems
- Progression and mastery systems
- Dynamic beat timelines
- Custom kana input handling
- Pitch-accent informed sound generation

The project was developed as a solo game-development and software architecture exercise with a strong focus on scalable systems design and educational gameplay engineering.

---

## Gameplay Goals

The primary design objective was to create a system that encourages:

- Fast kana recognition
- Rapid sound recall
- Reading fluency
- Keyboard familiarity
- Pattern recognition under time pressure
- Mastery through repeated rhythm interaction

The game specifically attempts to avoid shallow “flash-card style” learning by requiring players to internalize kana patterns while reacting to rhythm-based gameplay.

---

## Core Features

### Rhythm-Based Kana Gameplay

Players input Hiragana characters on beat-based timelines while progressing through increasingly difficult combinations.

The system supports:

- Standard kana
- Dakuten / Handakuten combinations
- Palatalized kana combinations
- Combo-note timing structures
- Variable beat timing windows

---

### Custom Kana Keyboard

BeatKana includes a fully custom kana keyboard system designed specifically for gameplay.

This avoids:
- Dependence on third-party Japanese keyboards
- Mobile setup friction
- Input latency inconsistencies
- Poor UX for rhythm gameplay

The keyboard system also supports progression-based key unlocking tied directly to save-state data.

---

### Educational Progression System

The progression structure was designed around language acquisition concepts rather than generic level gating.

Features include:

- Gradual Hiragana introduction
- Structured kana grouping
- Unlockable progression paths
- Difficulty scaling
- Accuracy-based progression
- Energy / mastery systems
- Training modes
- Endless procedural practice modes

---

### Dynamic Beat Timeline Architecture

Gameplay is driven through timeline and queue-based systems that dynamically generate playable note structures.

This includes:
- Combo note generation
- Queue-driven endless gameplay
- Beat-state handling
- Failure contingencies
- Timing-window management
- State-machine driven input flow

---

### Pitch Accent Processing Pipeline

The game uses public Japanese pitch-accent information to improve pronunciation and sound accuracy.

A custom processing workflow was created that:
1. Parsed pitch-accent language data
2. Used AI-assisted transformation workflows
3. Processed string-formatted linguistic data
4. Exported structured data into Unity Scriptable Objects

This allowed the game to build sound behavior from structured linguistic information rather than manually authored audio logic.

---

## Technical Architecture

### Technologies Used

- Unity
- C#
- Unity Scriptable Objects
- Custom Input Systems
- Data-Driven Content Architecture

---

### System Design Focus

A major emphasis of the project was creating reusable gameplay systems instead of hardcoded level logic.

Architectural goals included:

- Data-driven progression
- Modular beat generation
- Scalable kana handling
- Reusable gameplay timelines
- Platform flexibility
- Localization support
- Dynamic content generation

---

### Timeline & State Systems

BeatKana uses custom gameplay state logic to manage:

- Input timing windows
- Combo note progression
- Beat synchronization
- Failure recovery behavior
- Queue advancement
- Training-mode flow control

The system was designed to support future mobile deployment where timing precision and input reliability become more constrained.

---

### Scriptable Object Content Pipeline

The project makes extensive use of Unity Scriptable Objects for:

- Kana metadata
- Pitch information
- Level progression data
- Sound behavior
- Educational grouping systems

This allows content scaling without rewriting gameplay systems.

---

## Technical Challenges

### Designing Educational Gameplay That Encourages Real Skill

One of the largest challenges was creating mechanics that reinforce genuine language understanding rather than simple memorization.

The project required balancing:
- Reaction gameplay
- Reading comprehension
- Audio association
- Recognition speed
- Input complexity

The goal was to ensure players gradually develop actual kana familiarity through repeated rhythm interaction.

---

### Kana Composition Handling

Japanese kana combinations introduced substantial gameplay and architectural complexity.

The system required custom handling for:
- Dakuten
- Handakuten
- Small kana combinations
- Combo-note timing
- Variable beat subdivision
- Romaji parsing behavior

These systems had to remain readable to players while still functioning within rhythm-game timing constraints.

---

### Mobile-Compatible Input Design

Traditional Japanese keyboard solutions were not suitable for rhythm gameplay.

To solve this, BeatKana implemented:
- A custom kana keyboard
- Progression-based key unlocking
- Controlled focus/input behavior
- Gameplay-specific input flows

This approach also improves portability for future Android/iOS deployment.

---

### Pitch Accent Data Processing

Pitch-accent information was not directly usable in gameplay format.

A custom pipeline was created to:
- Parse public linguistic data
- Process structured strings
- Convert data into gameplay-friendly formats
- Export directly into Unity assets

This significantly reduced manual content authoring overhead.

---

## Planned Features / Future Improvements

- Android deployment
- iOS deployment
- Expanded training systems
- Katakana support
- Kanji-assisted gameplay modes
- Improved procedural content generation
- Adaptive difficulty systems
- Expanded audio feedback systems
- Full tutorial/guide framework
- Additional progression content
- More advanced endless gameplay generation

---

## Author

Nicholas Ball Ulmer

- GitHub: https://github.com/nick-ulmer
- Portfolio: https://f1forhelp.dev
- LinkedIn: https://linkedin.com/in/nicholas-ball-ulmer/
