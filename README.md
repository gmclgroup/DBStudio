# DBStudio
DBStudio was originally intended to help end users or developers manage the Microsoft Sql
Server Compact Edition 3.5 or up. But later, after made its’ architecture
support more widely used database, now it support manage a lot of common used
databases. All current supported databases are listed in this section.


![image](https://github.com/fuqifacai/DBStudio/tree/master/readme_images/1.png)

 





*[Skin section] allow user select the skin
(theme) which he liked. Currently we list many skins, included system default skins
and customized skins.



The
system default themes are shown as Text item and the customized themes shown as
an icon. 



 





*[Shortcut section]



Shortcut
section provides some quick linker to some useful and common functions. Such as
“documenting database”, “data transfer”, “connection string help linker”,etc.



 



Documenting
database: help user exporting the schema of database to XML or EXCEL format.  



Data transfer:
Many end users like this feature. It can transfer the data from Sql Server, Sql
CE, MySql, Excel, Sqlite, Firebird/Interbase, Access, and CSV to SqlCE. (And
now support to CSV, too). 



 



Example:
 



Here
we navigate to the SqlCE management page.





[Open latest file], open the
latest opened SqlCE file, if the file and password all ok, it will go to the
management page. 



[Open history], show a
window which list the entire opened SqlCE file; allow you select one to open
via double click that item. 



[Create], allow you
create an empty SqlCE file, make some settings in that SqlCE file. Such as:
Password, Encryption Mode, etc. 



[Sync], it is Data
transfer functionality. 



[Change
Password], you can input the SqlCE file name in the database textbox, and input
the old password (if has), then press this button will allow change your
password of that SqlCE file . 



[V], detects
the current selected SqlCE database file version. It is very useful because SqlCE
has some major differences in its’ each version.



[Open Mode], select mode to open
selected SqlCE database file, such as read, write, or read-only, etc. 



 



 



 



 



 



 



 



Main Management UI : 





Top
is the Ribbon Menu: 



It
has some basic functions according to the selected database type. 



{General Tab}



Open Script:
open a Sql Script file and display it in the Sql editor, you can run the script
file.

Save Script:
Save the Sql command in the Sql Editor to a script file. 

Close:
Close current management page, and back to the login in page. 

Exit: exit
the whole application right now. 

Execute Sql
Command: It will execute the Sql command which user input in the Sql Editor. 

Save Sql
query result : it will save the data which display in the bottom data grid
.(These data are often queried by the Sql command ) 

Next
Command/Last Command: show the previous or next command text.  















 



 



{Data Exchange Tab}





1)



Import
From Sql server

Import From Access

Import From Excel

Import From CSV

Import From MySql

These functions are similar with Data Transfer .But they has some differences
.Data Transfer like an upgrade version of these functions . Because they are
based on the different code and architect, so the result data may be different.




 



2)
Generate Linq O/R Map



This
function help user generating the Linq to Sql O/R mapping file, because SqlCE
Support Linq to Sql. And the target file type can be both c sharp (C#) and
visual basic (VB.Net). 



 



3)
Generate business entity 



This
function generates the business entity according to the table schema. 



{Tool Tab}





Download DB
File : Support Download a database file or other type file from internet 

Reset DB:
Reset (Delete/Clear) all the data of all tables in the database.

Get Current
Opened Database information: Display all the basic information of current
opened database. 

Change
Database Password: This function is same to the function which placed in the
log in page. 

Option:
Settings of Main Management UI.

 

{Special Tab}

 



















Export selected
table schema: display current table schema information .Display the information
in the Sql Editor. User can copy/paste to other tools to do something he wants.


Generate column
schema: generate the column schema information to both Xml and UI. It help user
see detail info of selected column. 

 

The
following is Management Detail UI: 









Tree List
(UI Command Center)

See below .You can
right click each item and see some embedded functions (It save your time to
input the Sql command directly). Each item will pop up relevant Context Menu
when it clicked. 

(If you open a SqlCE
database file, it will also support a [System View] item allow user see detail
information about this database system embedded views.  )

 

Sql Command
Area













You can input Sql command
in each tab of Command Area. Each content is standalone. Here you can input
Standard Sql Command ,like “Select “, “Insert”,”Alert”,”Update”,..etc . 





Sql result
section

This section displays
the result of your executed Sql command. 

You can create a new
tab to view many results. (Here we show 2 results in 2 tabs)











