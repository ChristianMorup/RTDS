#pragma once
#ifndef RTKWRAPPER_H
#define RTKWRAPPER_H

#ifdef RTKWRAPPER_EXPORTS
#define RTKWRAPPER_API __declspec(dllexport)
#else
#define RTKWRAPPER_API __declspec(dllimport)
#endif


extern "C" RTKWRAPPER_API int ProcessFiles(char* file_in1, char* file_in2, char* file_in3, char* file_out); 

#endif