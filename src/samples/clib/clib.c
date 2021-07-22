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

__attribute__((visibility("default")))
int clib(void)
{
    // Triggers -Wparentheses.
    if (x = 42)
        return x;

    return 0;
}
