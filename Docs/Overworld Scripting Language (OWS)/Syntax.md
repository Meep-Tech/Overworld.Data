# OWS Syntax
General [[Overworld Scripting Language (OWS)|OWS]] Syntax Overview
## General
General Rules for OWS Syntax include:
- Newlines matter for 'Line of Code' interpretation.
- OWS is Weakly, and Duck typed in all situations except where some [[Command]]s can be configured to only take specific [[Data Types|Data Types]] as [[Parameter]]s.
- Everything other than string literals are considered to be case insensitive.
- Whitespace is generally ignored. For the exception see the below bullet point.
- When invoking special keyworlds and [[Operatons]] though their "Phrase"/word versions, you must include spaces between the phrases and any variables. You do not need to do the same for symbol based Operators and 'Key-symbols'; For example:
```
# Does not require spaces, because it uses symbols:
5+5
# Requires spaces because it uses phrases/word based operator:
5 PLUS 5
# Does not require spaces, because it uses symbols:
SET:X=10
# Requires spaces because it uses phrases/word based operator:
SET:X TO 10
```
## Comments
Comments are indicated with the `#` symbol.
Both full and inline comments use the same symbol:
```
#This whole line is a comment
#Just the beginning of this line is a comment# Return:4
Return:4#Just the end of this line is a comment
```
## Starting a Line of Code
Every line of code in [[Overworld Scripting Language (OWS)|OWS]] starts with either a [[Line Label]] or a [[Command]] Invocation.
### Line Label Declaration
[[Line Label]]s are declared with a Label name surrounded by square brackets, followed by a collon. The Label's name should not have quotations around it:
```
[Label_Name]: ...code
```
A [[Command]] does not need to immediately follow a Line Label declaration; the line can also be left blank.

Label names should not have spaces, and should contain only Alphanumeric and the following characters: `_-`. Other characters may work, but are not officially supported. 
Line label names are case insensitive.
### Command Invocation
[[Command]]s are Invoked using the Command's name:
```
PRINT-HELLO-WORLD
```

If a Command has any [[Parameter]]s, you must pass them all in in the exact number, sperated by the `:` symbol:
```
SAY: "HELLO WORLD"
# OR:
MOVE:LEFT:4
```

Commands can also be invoked as [[Parameter]]s for other Commands or [[Operations]]. Their return value will be passed as the Parameter when the code is run:
```
SAY: GET-HELLO-WORLD-TEXT
# OR:
MOVE:GET-DRECTION:"LEFT":4
```

Label names should not have spaces, and should contain only Alphanumeric and the following characters: `_-`. Other characters may work, but are not officially supported. 
Command names are case insensitive.
#### Commands With Unique Syntax
Here is a list of commands with unique Syntax. There should be more info about them thought this and other articles:
- [[SET- Commands]] (See: [[Syntax#Declaring a New Variable or Setting an Existing Variable's value]] )
- [[IF]]::[[ELSE]]: (See: [[Syntax#IF and IF ELSE]])
- [[DO]]::[[WITH]] []: (See [[Syntax#DO WITH]])
- All Other [[Jump Commands]] also have slightly unique syntax. See below
#### Jump Commands
[[Overworld Scripting Language (OWS)|OWS]] does not have true Functions, or the ability to declare [[Command]]s in the Script itself.
The functionality of Functions can be replicated though using [[Jump Commands]].
Some of these Commands, such as [[GO-TO]] and [[DO]], are popularly used with [[Syntax#Loops|Loops]] and [[Syntax#IF and IF ELSE|IF Statements]] to execute more than one line of code in a single [[Parameter]]ized Command call. 
### Forwards 
Forwards [[Jump Commands]] move forwards to a new line of code using a [[Line Label]] name.
#### GO-TO:
GO-TO takes the name of a [[Line Label]] within the same [[Program]], and jumps to that line of code.
```
GO-TO: LABEL_NAME
```
The Line Label name should not be in quotations.
#### DO::WITH[]
Like GO-TO, [[DO]]::[[WITH]][] takes the name of a [[Line Label]] within the same [[Program]], and jumps to that line of code.
The DO Command can also use WITH Syntax to create [[Variable Scope#Temp-Local|Scope]]d [[Variable]]s (See: [[Syntax#Variable Scope]]).

WITH Syntax is currently unique to the DO Command, and uses the world `WITH`(case insensitive) followed by square brackets containing a list of entries in the format: `Variable_Name AS Value`(or `=` instead of `AS`, `AS` is also case insensitive), with each entry seperated by either a `,` or an `AND`(case insensitive):
```
DO:LABEL_NAME WITH [X = 1]
# OR:
DO: LABEL_NAME with [X AS 1 AND Y = 2]
# OR:
DO: LABEL_NAME with [X AS 1 & Y = 2]
# OR:
DO: LABEL_NAME with [X AS 1, Y = 2]
# OR:
DO: LABEL_NAME WITH 
... [
...    X AS 1
...    AND Y AS 2
... ]
```
The Line Label name should not be in quotations.
### Backwards
Backwards [[Jump Commands]] move back to the line of code that the last [[Syntax#Forwards|Forwards]] Jump Command executed from.
#### GO-BACK:
[[GO-BACK]] is a [[Command]] that stops the code execution at the current line, and goes back to the line that last executed a [[#Forwards|Forwards Jump Command]].
#### RETURN:
[[RETURN]] is a [[Command]] that stops the code execution at the current line, and goes back to the line that last executed a [[#Forwards|Forwards Jump Command]], and returns the provided [[Variable]] in place of the Forward Jump Command.
### Outwards
Outwards [[Jump Commands]] end the program's execution on the spot.
#### END
[[END]] is a [[Command]] that ends the running [[Overworld Scripting Language (OWS)|OWS]] [[Program]] immediately and returns null to the Program's caller.
#### END-AND-RETURN:
[[END-AND-RETURN]] is a [[Command]] that ends the running [[Overworld Scripting Language (OWS)|OWS]] [[Program]] immediately and returns the provided [[Variable]] to the Program's caller.
## Ending a Line of Code
A line of code is ended with a new-line, or with an optional `;` symbol. You can designate the end of a 'Line' using the semicolon without actually using a new-line character, allowing a whole program to potentailly be written in a one-line string:
```
SAY: "HELLO"
# OR:
SAY: "HELLO";
# OR:
SAY:"HELLO";MOVE:LEFT:5
```
### Continuing a Line of Code Passed a Newline
It is possible to continue a Line of Code(as interpeted) beyond a newline. To concatinate two broken lines of code into one, make sure the sub-lines(lines to be concatenated) all begin with `...` and follow the initial line:
```
SAY: "HELLO, I AM GOING TO SAY A LOT HERE SO"
... + "I NEED TO CONTINUE HERE"
# OR:
SAY: 
... "HELLO"
# OR:
Move 
... :Left:3
# OR:
Move 
... :Left
... :3
```
[[Variable]] names, [[Command]] names, [[Line Label]] names, and [[String]] literals cannot be patched together across a newline like this. Use the first above example;with the String Concatination (See: [[Operations#String Operations]]), on how to properly split a string between lines.
## Variables
### Declaring and Setting Variables
#### Declaring a New Vairable, or Setting a New Value to an Exising Variable
The [[SET- Commands]] are used to declare new [[Variable]]s, and to set the value of Variables. SET has special Syntax unlike other Commands:
Set takes a Variable name as it's first [[Parameter]], and then instead of a `:` uses the symbol `=`, or the text `TO`(case insensitive) as the seperator, followed by a value to assign to the provided Variable Name:
```
SET: X TO 11
# OR:
SET: Y = 1
# OR:
SeT: CharActEr = GET-CLOSEST-CHARACTER
```

The Variable name should not contain spaces, and should not be surrounded by quotations of any kind. Variable names should also contain only Alphanumeric and the following characters: `_-`. Other characters may work, but are not officially supported. 
Variable names are case insensitive.
#### Variable Scope
Variables in [[Overworld Scripting Language (OWS)|OWS]] can have one of five unique [[Variable Scope|Scope]]s. These Scopes are designed to be more specific in use for In-Game scripting than traditional scopes may be.
Different Scopes are assigned to using different variations of the [[SET- Commands]].
- **Program-Character-Specific**:
	- These [[Variables]] are only accessable to a specific [[Character]] in the same [[Program]] they were declared in.
	- <u>Command</u>: [[SET-LOCALLY]]
	- <u>Command</u>: [[SET-LOCALLY-FOR]]
- **World-Character-Specific**:
	- These Variables are only accessable to a specific Character, but can be accessed via any Program in the same [[World]] they were declared in.
	- <u>Command</u>: [[SET]] (The default syntax for the default SET Command)
	- <u>Command</u>: [[SET-FOR]]
- **Program-Global**:
	- These Variables are accessable to any Character, but can only be accessed via the Program they were declared in.
	- <u>Command</u>: [[SET-FOR-PROGRAM]]
- **World-Global**:
	- These Variables are accessable to any Character in any Program within the same World that they were declared in.
	- <u>Command</u>: [[SET-FOR-WORLD]]
- **Temporary-Local**:
	- These Variables are only accessable to the commands they are passed to via the [[Command Context]] object.
	- <u>Command</u>: [[WITH]] (See: [[Syntax#DO WITH]])
#### Using an Already-Set Variable
If a Variable has been set by name, you can use this Variable name as a placeholder for the Variable's value anywhere in the code. You should not surround this name with quotes:
```
SET X = "Variable Value"
SAY:X
# This will say "Variable Value", as that is what is stored in X.
```
#### Un-Setting a Variable
To un-set a Variable, you use the same syntax and equivalent [[UN-SET- Commands|UN-SET- Command]] to the [[SET- Commands|SET- Command]] used to create the variable initially.

Unsetting a Variable in it's [[Variable Scope|Scope]] makes it so that the Variable name behaves as if the Variable was never set when trying to access it. The [[Program]] stops tracking this Variable and it's name. Unsetting a Variable will cause the program to throw an error if you try to access the variable by this name in the specified scope again.

The [[-EXISTS]] operator can be used to check if a variable is set, or un-set.
### Literals
#### Strings
[[String]] literals are written using double quotes.
```
SET: text = "THIS IS A STRING Literal"
```
The contents of String literals are case sensitive.
##### Variable Replacement Closures
You can use curly bracets to specify a part of a string to replace with another value, such as a [[Variable]] the result of a [[Command]].
```
SET: text = "THIS IS A STRING WITH ANOTHER {"variable inside"}"
# OR:
SET: character = get-closest-character
SET: greeting = "Hello {character.name}!"
```
To prevent this from happening, simply add an `\`(backslash) character before the opening curly bracket in order to escape the Variable closure. This 'escaping' would result in "Hello {character.name}!" instead of "Hello Tiffany!", etc.
#### Numbers
[[Number]] literals are written starting with a Numeric value between 0 and 9.
```
SET: number = 100
# OR:
SET: number to 10.5
# OR:
SET: number TO 0.004
```
Numbers may contain a single decimal point, and no spaces.
### Collections
[[Collections]] in OWS are usually Weakly Typed, though some [[Command]]s require Collections who's elements all have the same [[Data Types|Data Type]]. 

Collections are declared using square brackets, surrounding entries seperated by one of the following case insensitive symbols or text operators: `AND`, `&`, and/or `,`. 
If a Command speifices one of it's [[Parameter]]s will be a Collection, you can exclude the square brakets when declaring that parameter: 
```
FOR-EACH:["Hello", "How are you?", "Goodbye"]:SAY:LOOP-OBJECT
# OR:
FOR-EACH:"Hello", "How are you?", "Goodbye":SAY:LOOP-OBJECT
# OR:
FOR-EACH:"Hello":SAY:LOOP-OBJECT
# OR:
FOR-EACH:Get-All-Characters:SAY:"Hello {LOOP-OBJECT.NAME}!"
```
## Operations
[[Operatons]] are [[Commands]] with special syntax that use a symbol or phrase for invocation. They usually take 2 [[Parameter]]s and return a value of the same type as the Parameters they took in.
An example of an Operation is: `5 + 5`.
### String, Number, and Collection Operations
Information on [[String]], [[Collection]], and [[Number]] Operations and Operator symbols, such as `+`, and `DIVIDED-BY` can be found here:  [[Operations]]
### Conditions and Boolean Operations
#### NOT-
[[NOT-]] is a special [[Operations#Boolean Operations|Conditional Boolean Operator]] that only has a Right-Hand [[Parameter]]:
```
SET F = NOT-TRUE
# OR: 
SET T = NOT-GET-SOME-FALSE-RESULT
```
#### IF:: and IF::ELSE:
[[IF]] and [[IF-NOT]] are [[Command]]s with some special Syntax that can be used with an [[Operations#Boolean Operations|Conditional Operator]] in order to perform a command.
The default Syntax for IF is rather straitforward. The Command executes the provided Command(Second [[Parameter]]) if the Conditional(First Parameter) it is true:
```
SET:TEST = 4;
IF:TRUE:SET:TEST=5; #Test is set to 5 here
# OR:
IF:FALSE:RETURN:4
RETURN:5
# OR:
SET X TO 1
IF:X = 1:RETURN:4;RETURN:5
# OR:
IF:TRUE:GOTO:true_label
RETURN:FALSE
[true_label]: END-AND-RETURN:TRUE
```

The special Syntax comes in with the [[ELSE]] keyword. This keyword turns the [[IF]] Command into an effective Ternary Statement that returns one of two values, depending on the result of the provided Condition. Both a `:` and then the `ELSE` Syntax must be added, with a following Condition to execute and return if the Condition turns out to be [[FALSE]].
```
SET X = IF:TRUE:4
...:ELSE:5
# OR:
SET X = IF:TRUE:4:ELSE:5
# OR:
SET X = IF:TRUE EQUALS TRUE:4:ELSE:5
# OR:
IF:X = 1:RETURN:TRUE:ELSE:DO:Line_Label WITH [Var as X]
```
#### Other Conditional Operations
Information on most Boolean Conditional operations such as [[AND]] and [[OR]] can be found here: [[Operations#Boolean Operations|Boolean Operator]]
## Loops
Loops in [[Overworld Scripting Language (OWS)|OWS]] are used to execute a [[Command]] multiple times in succession.
Loops are limited by a [[Operations#Boolean Operations|Condition]] , [[Collection]]'s size, or a provided Numeric Index value to use as a counter.

Some types of Loops provide a special [[LOOP-INDEX]] or [[LOOP-OBJECT]] [[Variable]] that can be placed in the [[Command]] or [[WITH]] statement in place of the current Index counter value of the loop.

Loops in OWS work well in conjucntion with [[#Jump Commands]] such as [[DO]].
```
SET X = 1
WHILE:X > 10:SET: X TO X + 2
# OR: 
SET:count=0;
SET:bonus=1;
SET:base=1;

COUNT-UP-WITH:count:100:
... DO: Label_Name 
... WITH [
...   Base as 4
...   AND Bonus is 2
... ]
```
### Built In Types
Built In Types of Loop Commands Include:
- [[ALWAYS]]: Execute a Command for as long as the program is enabled.
- [[WHILE]]: Execute a Command while a Condition is true.
- [[UNTIL]]: Execute a Command while a Condition is false.
- [[COUNTDOWN]]: Execute a Command X times while counting down from an initial [[LOOP-INDEX]].
- [[COUNT-UP]]: Execute a Command X times while counting up from to a final provided [[LOOP-INDEX]] value from 0.
- [[COUNTDOWN-WITH]]: Execute a Command X times while counting down with a provided variable as the [[LOOP-INDEX]].
- [[COUNT-UP-WITH]]: Execute a Command X times while counting up from 0 to another [[Number]] with a provided variable as the [[LOOP-INDEX]].
- [[FOR]]: Most similar to a traditional C-Like For Loop.
- [[FOR-EACH]]: Itterate though a collection of Objects