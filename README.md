MarkImplicitlyUsedStep
======================

This step allows the use of the "UsedImplicitly" Attribute (in
JetBrains.Annotations) to prevent the stripping of types, methods or
fields by monolinker.

Usage
-----

Compile to ImplicitlyUsed.dll and put with monolinker or in your path

Run - `monolinker -s
ImplicityUsed.MarkImplicitlyUsedStep,ImplicityUsed:MarkStep <other
monolinker options>`

It must be run before the normal MarkStep

