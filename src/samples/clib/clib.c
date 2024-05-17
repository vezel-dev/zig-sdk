// SPDX-License-Identifier: 0BSD

#include "clib.h"

typedef struct
{
    int y;
} test1;

typedef struct
{
    test1;
} test2;

int x;

#if defined(ZIG_OS_WINDOWS)

void undefined()
{
    // Testing this scenario on Windows would require an import library.
}

#else

void undefined();

#endif

__attribute__((visibility("default")))
int clib(void)
{
    // Exercise AllowUndefinedSymbols.
    undefined();

    // Triggers -Wparentheses.
    if (x = 42)
        return x;

    return 0;
}
