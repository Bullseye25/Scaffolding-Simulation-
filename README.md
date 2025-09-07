# Unity Scaffolding Simulation

# This project was created using **Unity 6.0.55f**.

# The Latest work will be found in the **Development**.

# It demonstrates a modular simulation system with grid management, camera controls, UI handling, and drag-and-drop functionality.  

---

## ✨ Core Features

### 1. Grid Manager
Handles the grid-based layout system for placing and managing objects.  
📹 [Part 1](https://drive.google.com/file/d/1iMSbpwC6AXkR3egZqPXd7B-S-ypv-3Qu/view?usp=sharing)

---

### 2. Camera Control
Includes full 3D camera movement:
- **Orbit** around the scene  
- **Pan** across the view  
- **Zoom** in/out for better perspective  
📹 [Part 2](https://drive.google.com/file/d/1LsVTcmwEbm-Nb2mT-Ak4UGcbF6Gniixd/view?usp=sharing)

---

### 3. UI Manager
- Built with **ScriptableObjects** to keep UI elements organized and reusable.  
📹 [Part 3](https://drive.google.com/file/d/1KU6t1w5ui498D9voVD0UWTAymNqKdFOJ/view?usp=sharing)

---

### 4. Drag and Drop
- Interactive drag-and-drop placement system for 3D objects.  
📹 [Part 4](https://drive.google.com/file/d/1vHp5_r4K0QPZjD2o8unbLkZxEaa2gmky/view?usp=sharing)

---

### 5. Full Simulation in Action
- All systems combined into a single interactive simulation.  
📹 [Final Simulation Video](https://drive.google.com/file/d/1wHN2SbsUkai9ME6kxyKLZxDWcvNwI8FB/view?usp=sharing)

---

## 🎮 User Controls

### 🖥️ Camera
- **Move Camera** → `W, A, S, D` or **Arrow Keys**  
- **Pan** → `Alt + Ctrl + Left Mouse Button`  
- **Orbit** → `Alt + Left Mouse Button`  
- **Zoom** → `Mouse Scroll Wheel` **or** `Alt + Right Mouse Button`

### 📦 Object Interaction
- **Snapping to Grid** → Hold `Left Shift` while dragging  
- **Rotate Element** → Press `R` while dragging  

---

## 🔍 What Could Be Improved

1. **Object Pooling**  
   - Instead of instantiating/destroying objects repeatedly, I could have implemented an **object pool**.  
   - This would improve performance by reusing inactive objects rather than creating new ones.

2. **3D Elements & Interfaces**  
   - I modeled the 3D elements myself, which was fun.  
   - With more time, I would have designed additional assets and applied the **Interface Segregation Principle**.  
   - Example: creating interfaces like `IStackable`, `IResizable`, `IPlaceable` to make the system more modular and extendable.

---

## 🛠️ Tech & Tools
- **Unity 6.0.55f**
- **C#**
- **ScriptableObjects**
- **Physics-based interactions**

---

## 🚀 How to Run
1. Clone this repository  
2. Open the project in **Unity 6.0.55f**  
3. Load the main scene and press ▶ Play  

---

## 📌 Notes
This project was built as a demonstration of modular Unity systems. It can be extended for simulation games, educational tools, or prototyping 3D interactions.  
