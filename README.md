# How to Add a New Extension
1) Download and set up Forge from https://fire.ly/forge (it's a FHIR profile editor)
2) Clone and check out the FHIR-Profiles repo from our older Git org
3) Open forge
4) Find and open one of the XML files for an existing extension (in that repo) - so you can compare against something
5) Create a new extension (you need to click on the "session" node before the "new" button is enabled)
6) Select the Properties tab
7) Paste your extension URL in the URL field (e.g. http://endeavourhealth.org/fhir/StructureDefinition/primarycare-appointment-dna-reason-extension)
8) Copy and paste the last part of the extension URL into the Resource ID field (e.g. primarycare-appointment-dna-reason-extension)
9) Copy and paste the last part of the extension URL into the Name field and change to start words with caps (e.g. PrimaryCare-AppointmentDnaReason-Extension)
10) Enter something in the Description box
11) Set the Publishing Date to today
12) Change Context Type to "Resource"
13) Click the + button next to the Context field and select the resource type(s) your extension applies to
14) Select the Element Tree tab
15) Click on "Value[x]"
16) On the right, uncheck all the options and then just check the option(s) for the data types your extension will store
17) Click "Save" to save your extension to XML, name it the same as the Resource ID (e.g. primarycare-appointment-dna-reason-extension.xml)
18) Find and open the Resource profile for the resource your extension goes on
19) Select the Element Tree tab
20) Click the top-most node in the tree below the tab
21) Click Extend to add a new extension to the resource, which you should see appear at the bottom
22) Click the new extension
23) Click the top-right drop down (extension) combo and you should see your new extension listed (any open extension files are shown here) - select it
24) Select the appropriate cardinality for your extension
25) Save the resource XML file
26) Close Forge
27) Commit and push your changes to Git.

