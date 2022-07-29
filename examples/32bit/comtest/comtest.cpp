#include <atlbase.h>
#include <iostream>

#import "..\comtestdotnet\comtestdotnet.tlb"

int main()
{

    #ifdef _WIN64
        std::cout << "Hello from C++! I am a 64 bit process" << std::endl;
    #elif _WIN32
        std::cout << "Hello from C++! I am a 32 bit process" << std::endl;
    #endif

    HRESULT hr = CoInitializeEx(NULL, COINITBASE_MULTITHREADED);
    if (FAILED(hr)) {
        std::cout << "CoInitializeEx failure";
        return EXIT_FAILURE;
    }

    comtestdotnet::IMyDemoInterfacePtr myInterfacePtr;
    hr = myInterfacePtr.CreateInstance(__uuidof(comtestdotnet::MyDemoClass));
    if (FAILED(hr)) {
        std::cout << "CreateInstance failure";
        return EXIT_FAILURE;
    }

    std::cout << myInterfacePtr->HelloWorld();

    CoUninitialize();
}
