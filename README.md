# XLIFF 2.0 Object Model
The XLIFF 2.0 Object Model contains classes and methods for generating 
and manipulating XLIFF 2.0 documents as described in the XLIFF 2.0
Standard. The library is built using the Portable Class Library enabling
developers to generate XLIFF documents using various platforms. 

## Goals for this project
The XLIFF 2.0 Object Model allows a developer to build up an XLF document
in memory and change various properties on the elements before writing
the file. This is intended to give developers a head-start in building 
localization tools, platforms, and engineering systems that take advantage
of the newest open localization standard.

## What this project provides 
The library includes classes for all the Core elements as well as all the 
module elements as described in the standard including: 

Core Elements (xliff, file, group, etc)
Change Tracking Module
Format Style Module
Glossary Module
Metadata Module
Resource Data Module
Size and Length Restriction Module
Translation Candidates Module
Validation Module

For more information, please take a look at the XLIFF 2.0 Class Guide
documentation provided with this project.

### Constraint Validation
This initial drop allows developers to read and write Core XLIFF 2.0 and
all associated modules as required by the XLIFF 2.0 Standard. However, full
validation for constraints defined in the standard is only available for
Core, Metadata, Glossary, and Translation Candidates Modules.  

##Contributing
Please help us improve the XLIFF 2.0 Object Model by filing
bugs or feature requests on this repo. You are encouraged to fork
and contribute a fix via a pull request.

###Bug Fixes
If you believe you've found a bug, you're encouraged to file an issue
on this repo.

## Licensing
The XLIFF 2.0 Object Model is licensed under the MIT License.
