const std = @import("std");
const assert = std.debug.assert;

pub export fn ziglib() i32 {
    return 42;
}

test "test ziglib fn" {
    assert(ziglib() == 42);
}
