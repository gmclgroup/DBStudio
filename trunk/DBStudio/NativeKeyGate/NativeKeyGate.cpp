// NativeKeyGate.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "NativeKeyGate.h"

using namespace System;
using namespace System::Runtime::InteropServices;

// This is an example of an exported variable
NATIVEKEYGATE_API int nNativeKeyGate=0;

// This is an example of an exported function.
NATIVEKEYGATE_API int fnNativeKeyGate(void)
{
	return 42;
}

// This is the constructor of a class that has been exported.
// see NativeKeyGate.h for the class definition
CNativeKeyGate::CNativeKeyGate()
{
	return;
}

namespace NativeGate {
public ref class Util
	{
	private :
		static unsigned short Key_Length=18;

		// TODO: Add your methods for this class here.
	public :static char* GetNativeStringFromManaged(System::String^ strSource)
			{
				IntPtr h=Marshal::StringToHGlobalAnsi(strSource);
				char* nativeString=(char*)(void*)h;
				int length=::strlen(nativeString);

				char* result=new char[length+1];

				strcpy_s(result,length+1,nativeString);

				Marshal::FreeHGlobal(h);

				return result;
			}

			//Validation key
	public :static bool LicenseKeySimplestValidation(System::String^ key)
			{
				bool result=false;
				try

				{
					char* lKey=GetNativeStringFromManaged(key);
					if(strlen(lKey)==Key_Length)
					{
						result=true;
					}

				}
				catch(...)
				{
					return false;
				}

				return result;
			}


	};
}