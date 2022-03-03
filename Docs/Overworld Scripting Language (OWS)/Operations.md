# Operations
Operations in [[Overworld Scripting Language (OWS)|OWS]] are [[Command]]s with special Syntax. These commands use symbols such as `+`,`-`,`AND`,`%`,`DIVIDED-BY`,`&`, etc to combine, do math on, and modify one or two values or [[Variable]]s in human readable ways.

## General Operation Syntax
Most Operations have a "Left Hand" and "Right Hand" [[Parameter]], as well as an Operator, which is either a peice of case insensitive text, or a symbol. Most types of Operator have both symbol and text options for invocation. When using the text invocation option, you must include spaces between the phrases and the Left and Right hand Parameters. You do not need to do the same for symbol based Operators; For example:
```
# Does not require spaces, because it uses symbols:
5+5
# Requires spaces because it uses phrases/word based operator:
5 PLUS 5
```

**Note:** [[-EXISTS]], [[NOT-]] and [[IDENTITY]] are the exceptions to many of these rules, and only have one Parameter each. Identity also uses no Operator symbol or text. [[GREATER-THAN]] and [[LESS-THAN]] are also exceptions, in that they don't return the same types they take in.

The general Operator use syntax is:
```
SET: Value = Left-Hand {Operator} Right-Hand
```
where: {Operator} is replaced with the Operator symbol or text of the Operation.

## Number Operations
Number Operations are also sometimes called Math Operations. In C# They are represented by the MathOperation class. These Operators preform mathmatical opperations on their [[Parameters]] as you'd expect them to.
### Addition
**Operators**: `+` and `PLUS`
Example: 
```
SET: TEN TO 6 PLUS 4
# OR
SET: TEN = 5+5
```
### Subtraction
**Operators**: `-` and `MINUS`
Example: 
```
SET TEN TO 16 MINUS 6
# OR
SET: TEN = 15 - 5
```
### Multiplication
**Operators**: `*`, `X`, `x`, and `TIMES`
Example: 
```
SET: TEN TO 2 TIMES 5
# OR
SET: TEN = 2*5
# OR
SET: TEN = 5 X 2
# OR
SET: TEN = 5x2
```
### Division
**Operators**: `/`, `÷` and `DIVIDED-BY`
Example: 
```
SET: TEN TO 60 DIVIDED-BY 6
# OR
SET: TEN = 60÷6
# OR
SET: TEN = 60/6
```
### Exponential Functions
**Operators**: `^` and `TO-THE-POWER-OF`
Example: 
```
SET: One-Hundred TO 10 To-the-power-of 2
# OR
SET: One-Hundred TO 10^2
```
### IS-GREATER-THAN
**Operators**: `>` and `IS-GREATER-THAN`
[[GREATER-THAN]] takes in two [[Number]] values, but returns a [[Boolean]] value.
Example: 
```
SET: t-value TO 5 < 3
# OR
SET: t-value TO true&true
```
### IS-LESS-THAN
**Operators**: `<` and `IS-LESS-THAN`
[[LESS-THAN]] than takes in two [[Number]] values, but returns a [[Boolean]] value.
Example: 
```
SET: t-value TO true AND true
# OR
SET: t-value TO true&true
```

## Boolean Operations
Boolean Opperators are also sometimes called Conditions or Conditional Operators.
In C# They are represented by the Condition class.
### AND
**Operators**: `&` and `AND`
Example: 
```
SET: t-value TO true AND true
# OR
SET: t-value TO true&true
```
### OR
**Operators**: `|` and `AND`
Example: 
```
SET: t-value TO true AND true
# OR
SET: t-value TO true&true
```
### NOT
**Operators**: `!` and `NOT-`
Example: 
```
SET: t-value TO !false
# OR
SET: t-value TO not-false
```
### EQUALS
**Operators**: `=` and `EQUALS`
Example: 
```
SET: t-value TO 4 EQUALS 4
# OR
SET: t-value TO true = true
```
### IDENTITY
The identity Operator is used internally only and has no symbols or phrases to invoke it. 
### EQUALS
**Operators**: `&` and `AND`
Example: 
```
SET: t-value TO true AND true
# OR
SET: t-value TO true&true
```
## String Operations
### Concatinators
**Operators**:  `+`, `PLUS`, `AND`
The [[String]] Concatinator is the only String Operator in OWS. It can be used to combine multiple strings together into one.
```
set: text = "SOME " + "TEXT THA" AND "T IS ALL SPLI" PLUS "T UP"
```
## Collection Operations
### Concatinators
**Operators**:  `+`, `PLUS`, `AND`, `WITH`, and `&`
The [[Collection]] Concatinator can be used to combine multiple collections together into one, or to append an element to the current collection.
```
set: letters = ["a", "b" and "c"] WITH "d";
#OR:
set: letters = ["a", "b" and "c"] WITH ["d" AND "e"];
#OR:
set: letters = ["a", "b" and "c"] PLUS ["d" AND "e"];
#OR:
set: letters = ["a", "b" and "c"] + ["d" AND "e"];
#OR:
set: letters = ["a", "b" and "c"] + "d" WITH ["e" AND "f"];
#OR:
FOR-EACH:["a", "b" and "c"] + "d" WITH ["e" AND "f"]:SAY:"the letter is {loop-object}"
```
### Without
**Operators**:  `+`, `PLUS`, `AND`, `WITH`, and `&`
The [[Collection]] Without Operator can be used to remove items from a collection. It can be used with a collection, or a single item.
```
set: lettersAB = ["a", "b" and "c"] WITHOUT "c";
#OR:
set: lettersA = ["a", "b" and "c"] WITHOUT ["c" AND "b"];
#OR:
FOR-EACH:["a", "b" and "c", "d"] WITHOUT "d" WITHOUT ["a" AND "b"]:SAY:"the letter is {loop-object}"
```
## Object Operators
### Exists
**Operators**:  `-EXISTS` , `~` ...
The Exists Operator is used to check if a variable has been Set/Exists anywhere. There are several variants of the Operator for the different [[Variable Scope]]s:
- `-EXISTS` or `~`
- `-EXISTS-LOCALLY` or `~~`
- `-EXISTS-FOR-PROGRAM` or `~^`
- `-EXISTS-FOR-WORLD` or `~@`
- `-EXISTS-ANYWHERE` or `~!`
- `-EXISTS-FOR-{CHARACTER-NAME}` or `~{CHARACTER-NAME}`\* 
- `-EXISTS-LOCALLY-FOR-{CHARACTER-NAME}` OR `~~{CHARACTER-NAME}`\* 

\* : `{CHARACTER-NAME}` should be replaced with the identifier of the [[Character]] you want the Variable Scope information for.