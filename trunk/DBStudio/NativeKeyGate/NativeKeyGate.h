// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the NATIVEKEYGATE_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// NATIVEKEYGATE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef NATIVEKEYGATE_EXPORTS
#define NATIVEKEYGATE_API __declspec(dllexport)
#else
#define NATIVEKEYGATE_API __declspec(dllimport)
#endif

// This class is exported from the NativeKeyGate.dll
class NATIVEKEYGATE_API CNativeKeyGate {
public:
	CNativeKeyGate(void);
	// TODO: add your methods here.
};

extern NATIVEKEYGATE_API int nNativeKeyGate;

NATIVEKEYGATE_API int fnNativeKeyGate(void);
