
# Variable Scope
A [[Variable]]'s Scope in [[Overworld Scripting Language (OWS)/_index|OWS]] is the Context in which one can access a desired variable by it's name. Depending on the Scope, a Variable may only be accessable in certain situaions, to a limited set of [[Program]]s and [[Character]]s in the [[World]].

## Scopes
Different Scopes are assigned to and un-assigned to in OWS using different variations of the [[SET- Commands]] and [[UN-SET- Commands]].
### Program Character Specific
- These [[Variables]] are only accessable to a specific [[Character]] in the same [[Program]] they were declared in.
- <u>SET Command</u>: [[SET-LOCALLY]] (Defaults to the Executing Character)
- <u>SET Command</u>: [[SET-LOCALLY-FOR]]
- <u>UN-SET Command</u>: [[UN-SET-LOCALLY]] (Defaults to the Executing Character)
- <u>UN-SET Command</u>: [[UN-SET-LOCALLY-FOR]]
### World Character Specific
- These Variables are only accessable to a specific Character, but can be accessed via any Program in the same [[World]] they were declared in.
- <u>SET Command</u>: [[SET]] (Defaults to the Executing Character)
- <u>SET Command</u>: [[SET-FOR]]
- <u>UN-SET Command</u>: [[UN-SET]] (Defaults to the Executing Character)
- <u>UN-SET Command</u>: [[UN-SET-FOR]]
### Program Global
- These Variables are accessable to any Character, but can only be accessed via the Program they were declared in.
- <u>SET Command</u>: [[SET-FOR-PROGRAM]]
- <u>UN-SET Command</u>: [[UN-SET-FOR-PROGRAM]]
### World Global
- These Variables are accessable to any Character in any Program within the same World that they were declared in.
- <u>SET Command</u>: [[SET-FOR-WORLD]]
- <u>UN-SET Command</u>: [[UN-SET-FOR-WORLD]]
### Temporary Local
- These Variables are only accessable to the [[Commands]] they are passed to via the [[Command Context]] object.
- <u>Syntax</u>: [[WITH]] (See: [[Syntax#DO WITH]])