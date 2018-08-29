using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("CoreEA")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("XLAG")]
[assembly: AssemblyProduct("CoreEA")]
[assembly: AssemblyCopyright("Copyright © XLAG 2007")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("16f6222b-5e3c-4425-8761-e5eb54b27359")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision

//Version 0.1.0.1
//--1) with key 

//Version 1.1.0.1
//--1)Update new mysql provider 

//Version 1.1.0.2
//->1)Remove XLCS Depen

//Version 1.2.0.1
//->Add DbHandlerBase
//->Add DbParameters ways
//->Add some implements in some Eas
//->Refactor some methods refer to EntLib

//Version 1.2.0.2
//->Add more method in SqlCommand
//->Delete ODBC method and ConnectMode in Access and CoreEA
//->Change namespace of Sqlce(Current version is 3.5)
//->Fix bugs in SqlCE

//version 1.2.0.3
//->Add SqlInjection Detect in IValidationBase
//->Add other method 

//version 1.2.0.4
//->Add EnmuSqlServerInstance
//->Add ExtensionMethod Base
//->Refactor some features
//->Refactor Connection string (Espeical in Sql Server Express)

//version 1.2.0.5
//->Change Access Type to Oledb(Thus, we support Excel...etc...)
//->Refactor Connection String class

//version 1.2.0.6
//->Refactor CoreEA
//->Add Functions in Proecssing Exception
//->Upgrade to sqlce 3.5sp1 (dll)
//->Add SetConnection Method , which sqlce need it if can't create it . 

//version 1.2.0.7
//->Remove necessary Namespace
//->Add Property in Interface
//->Add Some new method in base class

//version 1.2.0.8
//->Big Refactor in Whole App
//->Upgrade to .net 3.5 

//version 1.3.0.1
//->Refactor some sql command execute method
//->Add some SSCE has implemented method

//version 1.4.0.2
//->Add Modes (Columns,Indexes,Keys,Constraints)

//version 1.5.1.0
//->Huge refactor
//->Add Excel,CSV support

//version 1.6.1.0
//->Add Support for password Access
//->Add Support for Oracle Database

//version 1.6.2.0
//->Add Generate Create Table Script in SqlCE and SqlServer
//->Made some modifications on create index in sqlce and sql server

//version 1.7.2.0
//->Upgrade to .net framework 4.0 
//->Fix some bugs
//->Add Oracle dll from .net framework4.0(remove old odp.net dll dependency)
//->Refactor

//version 1.8.2.0
//1)Support Effiproz database type

[assembly:SuppressIldasm()]
[assembly: AssemblyVersion("1.8.2.0")]
[assembly: AssemblyFileVersion("1.8.2.0")]


//[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SSCEViewer_UnitTest,PublicKey=0024000004800000540000000602000000240000525341310002000001000100b34d8d0b3b7f20c61d0760d531bd77b7112fd8d4512c3de51ac217401e04bc53739bfc4218c480b9fc06de87deb313b3d4ec7732156d511eb4b6aa50899d8da3")]