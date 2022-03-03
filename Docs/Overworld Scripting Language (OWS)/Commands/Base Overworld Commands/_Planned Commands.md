## Movement Commands
These commands are used to Move entities a desired number of tiles using the default Move/Walk animation.
All of these also have a version with `-FOR` that takes a collection of entities as the first variable before the others described below.
- [[QUEUE-MOVEMENT]] Takes a [[Collection]] of [[../../Universal Variables#Cardinal Directions|Directions]] and [[Number]]s alternating and adds each alternating set to the movement Queue for the Attached Entity.
- [[MOVEMENT-IS-COMPLETE]] Used to check if the movement queue is done for the current entity. Also has a For version like Queue movement and Move. Can be used like `until:movement-is-complete-for:"Meep":wait`
- [[MOVE]] Takes a[[../../Universal Variables#Cardinal Directions|Direction]] and a [[Number]] and moves the Attached entity that many spaces using the general Move/Walk animation. This also calls [[WAIT-UNTIL-MOVEMENT-COMPLETE]] immediately, unlike [[QUEUE-MOVEMENT]].  You can leave the directional parameter empty to move forward in the direction the entity is currently facing. `move::4`
- [[MOVE-IN-PATTERN]]: Takes a [[Collection]] of [[../../Universal Variables#Cardinal Directions|Directions]] and [[Number]]s alternating. It moves the Attached [[Entity]] in the given pattern. This functions like [[MOVE]] and calls the Wait function immediately afterwards.
- [[FACE]] Takes an [[Entity]] and [[../../Universal Variables#Cardinal Directions|Direction]] and turns the entity to face the desired direction in game. Also has a Queue version like [[Move]]
- [[TURN-CLOCKWISE]] Takes an entity, direction, or corner and turns it. If it's an entity, it turns it in the game and returns the direction they are now facing. Also has a Queue version like [[Move]]
- [[TURN-COUNTER-CLOCKWISE]] Takes an entity, direction, or corner and turns it. If it's an entity, it turns it in the game and returns the direction they are now facing. Also has a Queue version like [[Move]]

## Communication
- Think (makes a thought bubble)
- Act (does stuff with asterix, some special effects, like emotuing)
- play-animation
- loop-animation
- Say
- Whipser
- Shout
- Give

## Simple Ux View
Simple UX will eventually have an OWS based builder:
``` 
set: newView = Make-Simple-Ux-View: "Main Title" : [
...    Make-Pannel-Tab : "Tab Title" : [
...       DO:Make-Simple-Ux-Field WITH [
...         Display-Type AS "Text",  
...         Label AS "Field Name"
...      ]
...    ] 
...    AND Make-Pannel-Tab : "Tab Title":[]
...    AND Make-Pannel-Tab : "Tab Title":[]
... ]

display-simple-ux-for : CHARACTER-WHO-EXECUTED-THIS-SCRIPT : newView

until:not-view-is-open-for:CHARACTER-WHO-EXECUTED-THIS-SCRIPT:newView:wait;

set:FIELD-VAlue = GET-SIMPLE-UX-FIELD-VALUE:"Field Name":newView
```