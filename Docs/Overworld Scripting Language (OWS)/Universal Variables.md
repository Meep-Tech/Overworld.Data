# Universal Variables and Constants
These are OWS Variables that are Globally acessable to all OWS programs, are usually readonly, and usually always serve the same function in each program.
## Tiles
`TILE-THIS-SCRIPT-EXECUTED-FROM`: The [[Tiles]] the currently running [[Overworld Scripting Language (OWS)/_index|OWS]] executed from. It may be the Tile the Script is attached to, or the tile the Entity attached to the script is on.
## Entities
`ENTITY-THIS-SCRIPT-IS-ATTACHED-TO`: The [[Entity]] who the currently running [[Overworld Scripting Language (OWS)/_index|OWS]] is attached to.
## Characters
`CHARACTER-WHO-EXECUTED-THIS-SCRIPT`: The [[Character]] who executed the currently running [[Overworld Scripting Language (OWS)/_index|OWS]] script.
## Cardinal Directions
The four cardinal directions exist as read only [[Variable]]s in OWS:
- `NORTH`
- `SOUTH`
- `EAST`
- `WEST`
- `NORTH-WEST`
- `NORTH-EAST`
- `SOUTH-EAST`
- `SOUTH-WEST`