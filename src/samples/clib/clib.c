#include "clib.h"

int x;

__attribute__((visibility("default")))
int clib(void)
{
    // Triggers -Wparentheses.
    if (x = 42)
        return x;

    return 0;
}
