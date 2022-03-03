# Ows
## Overview and Goals
Overworld Scripting Language, or **OWS**, is a line by line interpreted scripting language, made for use in game in [[../Overworld]]. 
It is designed with these focuses in mind:
- Easy to read and interperet.
- Quick to get in to and learn with redundant and forgiving syntax.
- Minimal complexity per line; only 3 [[Command]]s at most should be needed in a line of Ows code.
- Quick and simple Commands that directly tie into Overworld game events and actions.
- [[Variable]] scope is focused on what makes sense in the game world. Scripts know what [[Entity]] executed them, what Entity interacted with that Entity to start the executable, and recieve a modable [[World]] level [[Program Context]] with other info; such as nearby [[Character]]s and Entities.
- Object types available for use in code are controled by the Program Context.

## Documentation:
- [[Data Types]]
- General [[Syntax]]
- [[Operatons]] Syntax
- [[Variable Scope]]
- Global and [[Universal Variables]]