# Final Project: Boat Game!

![ShipGameGif](https://github.com/user-attachments/assets/cb8ed2c0-471c-4ea6-bb82-ae26115a51bd)

This is the repository for my Procedural Ship Game, a top-down combat game where you pilot a toy boat against waves of enemy ships within a foggy pool! Interesting features include:

- Volumetric Fog, can be parted by objects which attenuate the fog density
- Procedural Water with Ripples using render textures and a wave shader
- Render texture scrolling to follow the player across a huge map
- Boid NPC behaviors to create ship formations
- Procedural behavior generation for enemies, with possible states depending on assigned tasks and enemy type
- Procedural enemy attributes, including coloring, decoration, and armaments based on enemy types
- UI & Radar Minimap to enhance immersion

Final Presentation / Writeup: https://docs.google.com/presentation/d/1UKBCCSkjmP8u_keOVX7MhYTuFbagRYfBnC0QpznhGNI/edit?usp=sharing

Demo Video: https://youtu.be/7vNvFi7JsXs

Thanks for checking this out!

## Controls

- WASD: control the ship's engine power and turning
- Left Click: Fire guns
- Right Click: Launch torpedoes (firing angles limited to the sides of the ship)
- Q, E, Scroll Wheel: Controls camera angle and camera zoom, respectively
- Tab: toggle between the camera's 2 different modes


## Project Planning: Design Doc

https://docs.google.com/document/d/1HD5-_L-pCF_eC4lvgOmJ6otgNM5OX93LYyv_ONMpe5w/edit?usp=sharing


## Milestone #1

- Current Progress:
  - Procedural water with ripples using render textures and shaders:
    - Top-down orthographic camera renders water-interacting objects to a collision render texture for creation of new waves
    - Additive blend shader takes the current frame's wave RT and blends it with this collision RT
    - Wave frag shader reads current frame and previous frame wave RT, computing next frame's wave RT with propogation and damping
  - Render texture scrolling to support the orthographic camera and water texture's movement with the player
  - Designed and implemented player controller and shooting
  - Added top-down camera setup
  - Implemented friendly NPC procedural group behaviors with boids
 
- Screenshots:

| <img alt="Screenshot 2025-11-11 182142" src="https://github.com/user-attachments/assets/9369d0a9-54e4-48be-82ed-2edacc6d744a" /> | <img alt="Screenshot 2025-11-11 234142" src="https://github.com/user-attachments/assets/8de07eba-2713-410a-bb29-5302c839705a" /> | <img alt="Screenshot 2025-11-12 011452" src="https://github.com/user-attachments/assets/291096b2-650d-4310-82fd-ccc854afd07a" /> |
|:--:|:--:|:--:|
| *Render Texture* | *Simple Water Texture* | *Water w/ Distortion* |

## Milestone 2: Implementation part 2 (due 11/24)

- Progress:
  - Procedural NPC enemy types, with the type affecting appearances, weapons, and behaviors
  - Randomized spawning system
  - Volumetric fog with dissipation
    - Top-down orthographic camera renders fog-interacting objects to a collision render texture
    - Additive blend shader blends it with the current frame's fog RT
    - Render texture is dampened every frame to simulate the fog filling the space back up, read by the fog raymarcher for density attenuation
  - Gameplay tweaks

- Screenshots:

| <img alt="Screenshot 2025-11-13 203016" src="https://github.com/user-attachments/assets/d52c9d67-0fd7-4c75-8d3c-6d6efbdf40f8" /> | <img alt="Screenshot 2025-11-13 024010" src="https://github.com/user-attachments/assets/658847b9-5e8d-4577-be75-916e002df009" /> | <img alt="Screenshot 2025-11-14 010401" src="https://github.com/user-attachments/assets/524ce757-d371-469c-9282-8cc5f5957128" /> | <img alt="Screenshot 2025-11-19 014734" src="https://github.com/user-attachments/assets/956fe620-cc92-4ad8-be45-44ec71d36068" /> |
|:--:|:--:|:--:|:--:|
| *Render Texture* | *Volumetric Fog* | *Fog Trails* | *In-Game* |


## Final submission (due 12/1)

- Progress:
  - Camera Rework
  - Simple VFX
  - UI added
  - Increased enemy variety
  - Audio
